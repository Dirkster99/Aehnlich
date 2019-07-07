namespace AehnlichLib.Binaries
{
    using AehnlichLib.Interfaces;
    using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;

	/// <summary>
	/// This class implements a binary differencing algorithm to return a
	/// near-optimal set of differences.  It uses the algorithm from "A
	/// Linear Time, Constant Space Differencing Algorithm" by Randal C.
	/// Burns and Darrell D. E. Long.  It is based on the pseudo-code from
	/// Randal C. Burns's master thesis entitled "Differential Compression:
	/// A Generalized Solution For Binary Files".
	///
	/// Finding an optimal set of differences requires quadratic time
	/// relative to the input size, so it becomes unusable very quickly.
	/// This near-optimal linear algorithm is good enough for most cases.
	///
	/// As the Burns/Long paper suggested, this implementation uses a
	/// Karp-Rabin hashing scheme so that sequential footprints can be
	/// easily determined based on the previous hash and the next byte.
	///
	/// The base implementation of this class returns the diffs in an
	/// AddCopyCollection.  If for performance reasons you need the diffs in a
	/// different format, then inherit from BinaryDiff and provide
	/// overrides for EmitAdd and EmitCopy.  Then you can dump the diffs
	/// out however you need (e.g. to a member Stream called this.DiffFile).
	/// </summary>
	public class BinaryDiff
	{
		#region Private Data Members

		private bool favorLastMatch;
		private int footprintLength = 8;
		private int tableSize = 1009;
		private uint powerD = 128;

		#endregion

		#region Constructors

		public BinaryDiff()
		{
		}

		#endregion

		#region Public Methods and Properties

		/// <summary>
		/// Whether the first or last match is favored if two segments
		/// hash to the same entry in the hashtable.  This defaults to
		/// false.
		/// </summary>
		public bool FavorLastMatch
		{
			get
			{
				return this.favorLastMatch;
			}

			set
			{
				this.favorLastMatch = value;
			}
		}

		/// <summary>
		/// The length of bytes to hash together.  This defaults to 8
		/// and must be between 1 and 31.
		/// </summary>
		public int FootprintLength
		{
			get
			{
				return this.footprintLength;
			}

			set
			{
				if (value >= 1 && value <= 31)
				{
					this.footprintLength = value;

					// Computes d = 2^(m-1) with the left-shift operator
					this.powerD = 1;
					for (int i = 1; i < this.footprintLength; i++)
					{
						this.powerD = this.powerD << 1;
					}
				}
				else
				{
					throw new ArgumentOutOfRangeException("value", value, "The value must be between 1 and 31.");
				}
			}
		}

		/// <summary>
		/// Sets the size of the hashtable to use.  This defaults to 1009.
		/// </summary>
		/// <remarks>
		/// The hash table size should be a prime number.
		/// </remarks>
		public int TableSize
		{
			get
			{
				return this.tableSize;
			}

			set
			{
				if (value >= 1)
				{
					this.tableSize = value;
				}
				else
				{
					throw new ArgumentOutOfRangeException("value", value, "The value must be greater than or equal to one.");
				}
			}
		}

		/// <summary>
		/// Does a binary diff on the two streams and returns an <see cref="AddCopyCollection"/>
		/// of the differences.
		/// </summary>
		/// <param name="baseFile">The base file.</param>
		/// <param name="versionFile">The version file.</param>
		/// <returns>An AddCopyCollection that can be used later to construct the version file from the base file.</returns>
		public AddCopyCollection Execute(Stream baseFile, Stream versionFile,
                                         IDiffProgress progress)
		{
			if (!baseFile.CanSeek || !versionFile.CanSeek)
			{
				throw new ArgumentException("The Base and Version streams must support seeking.");
			}

			TableEntry[] table = new TableEntry[this.tableSize];
			List<IAddCopy> list = new List<IAddCopy>();
			AddCopyCollection result = new AddCopyCollection(list);

			baseFile.Seek(0, SeekOrigin.Begin);
			versionFile.Seek(0, SeekOrigin.End);

			int verPos = 0;
			int basePos = 0;
			int verStart = 0;
			bool isBaseActive = true;
			uint verHash = 0;
			uint baseHash = 0;
			int lastVerHashPos = 0;
			int lastBaseHashPos = 0;

			while (verPos <= (versionFile.Length - this.footprintLength))
			{
                progress.Token.ThrowIfCancellationRequested();

                // The GetTableEntry procedure will add the entry if it isn't already there.
                // This gives us a default behavior of favoring the first match.
                verHash = this.Footprint(versionFile, verPos, verHash, ref lastVerHashPos);
				TableEntry verEntry = GetTableEntry(table, verHash, versionFile, verPos);

				TableEntry baseEntry = null;
				if (isBaseActive)
				{
					baseHash = this.Footprint(baseFile, basePos, baseHash, ref lastBaseHashPos);
					baseEntry = GetTableEntry(table, baseHash, baseFile, basePos);
				}

				if (baseFile == verEntry.File && Verify(baseFile, verEntry.Offset, versionFile, verPos))
				{
					int length = this.EmitCodes(verEntry.Offset, verPos, verStart, baseFile, versionFile, list);
					basePos = verEntry.Offset + length;
					verPos += length;
					verStart = verPos;
					FlushTable(table);
					continue;
				}
				else if (this.favorLastMatch)
				{
					verEntry.Offset = verPos;
					verEntry.File = versionFile;
				}

				isBaseActive = isBaseActive && (basePos <= (baseFile.Length - this.footprintLength));
				if (isBaseActive)
				{
					if (versionFile == baseEntry.File && Verify(versionFile, baseEntry.Offset, baseFile, basePos)
						&& verStart <= baseEntry.Offset)
					{
						int length = this.EmitCodes(basePos, baseEntry.Offset, verStart, baseFile, versionFile, list);
						verPos = baseEntry.Offset + length;
						basePos += length;
						verStart = verPos;
						FlushTable(table);
						continue;
					}
					else if (this.favorLastMatch)
					{
						baseEntry.Offset = basePos;
						baseEntry.File = baseFile;
					}
				}

				verPos++;
				basePos++;
			}

			this.EmitCodes((int)baseFile.Length, (int)versionFile.Length, verStart, baseFile, versionFile, list);

			Debug.Assert(
				result.TotalByteLength == (int)versionFile.Length,
				"The total byte length of the AddCopyCollection MUST equal the length of the version file!");
			return result;
		}

		#endregion

		#region Protected Methods

		protected virtual void EmitAdd(int versionStart, int length, Stream versionFile, IList<IAddCopy> list)
		{
			versionFile.Seek(versionStart, SeekOrigin.Begin);
			byte[] bytes = new byte[length];
			versionFile.Read(bytes, 0, length);
			Addition add = new Addition(bytes);
			list.Add(add);
		}

		protected virtual int EmitCopy(int basePosition, int length, Stream baseFile, IList<IAddCopy> list)
		{
			Copy copy = new Copy(basePosition, length);
			list.Add(copy);
			return length;
		}

		#endregion

		#region Private Methods

		private static int ExtendMatch(Stream baseFile, int basePos, Stream verFile, int verPos)
		{
			baseFile.Seek(basePos, SeekOrigin.Begin);
			verFile.Seek(verPos, SeekOrigin.Begin);
			int length = 0;
			int byteValue = 0;
			while ((byteValue = baseFile.ReadByte()) == verFile.ReadByte() && byteValue != -1)
			{
				length++;
			}

			return length;
		}

		private static void FlushTable(TableEntry[] table)
		{
			for (int i = 0; i < table.Length; i++)
			{
				table[i] = null;
			}
		}

		private static TableEntry GetTableEntry(TableEntry[] table, uint hash, Stream file, int pos)
		{
			int index = (int)(hash % table.Length);
			TableEntry result = table[index];
			if (result == null)
			{
                result = new TableEntry
                {
                    File = file,
                    Offset = pos
                };
                table[index] = result;
			}

			return result;
		}

		private static bool Verify(Stream baseFile, int basePos, Stream verFile, int verPos)
		{
			baseFile.Seek(basePos, SeekOrigin.Begin);
			verFile.Seek(verPos, SeekOrigin.Begin);
			return baseFile.ReadByte() == verFile.ReadByte();
		}

		private int EmitCodes(int basePos, int verPos, int verStart, Stream baseFile, Stream verFile, IList<IAddCopy> list)
		{
			if (verPos > verStart)
			{
				this.EmitAdd(verStart, verPos - verStart, verFile, list);
			}

			int copyLength = ExtendMatch(baseFile, basePos, verFile, verPos);
			if (copyLength > 0)
			{
				this.EmitCopy(basePos, copyLength, baseFile, list);
			}

			return copyLength;
		}

		private uint Footprint(Stream file, int pos, uint lastHash, ref int lastPos)
		{
			uint hash = 0;

			// We must allow rollovers
			unchecked
			{
				if (pos == lastPos + 1)
				{
					// Rehash using a Karp-Rabin rehashing scheme.
					file.Seek(lastPos, SeekOrigin.Begin);
					int prevByte = file.ReadByte();
					file.Seek(pos + this.footprintLength - 1, SeekOrigin.Begin);
					int nextByte = file.ReadByte();
					return (uint)(((lastHash - (prevByte * this.powerD)) << 1) + nextByte);
				}
				else
				{
					// Generate a new hash
					file.Seek(pos, SeekOrigin.Begin);
					for (int i = 0; i < this.footprintLength; i++)
					{
						hash = (uint)((hash << 1) + file.ReadByte());
					}
				}
			}

			lastPos = pos;
			return hash;
		}

		#endregion

		#region Helper Classes

		private class TableEntry
		{
			public Stream File { get; set; }

			public int Offset { get; set; }
		}

		#endregion
	}
}
