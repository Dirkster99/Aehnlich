namespace AehnlichLib.Binaries
{
	using AehnlichLib.Interfaces;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;
	using System.Text;

	/// <summary>
	/// This class provides a way to get a set of text lines that
	/// can be diffed to display the difference between two binary
	/// files.
	/// </summary>
	public sealed class BinaryDiffLines
	{
		#region Public Constants

		// This magic number comes from the 8 hex-digit position marker
		// and the 4 whitespace characters at the beginning of each line.
		public const int PrefixLength = 12;

		#endregion

		#region Private Data Members

		private readonly IList<string> baseLines = new List<string>();
		private readonly int basePosition;
		private readonly int bytesPerLine;
		private readonly IList<string> versionLines = new List<string>();
		private readonly int versionPosition;

		#endregion

		#region Constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="baseFile"></param>
		/// <param name="list"></param>
		/// <param name="bytesPerLine"Number of bytes to display per line.></param>
		public BinaryDiffLines(Stream baseFile, AddCopyCollection list, int bytesPerLine)
		{
			this.bytesPerLine = bytesPerLine;

			baseFile.Seek(0, SeekOrigin.Begin);

			foreach (IAddCopy entry in list)
			{
				if (entry.IsAdd)
				{
					// Add A's bytes to VerLines
					Addition add = (Addition)entry;
					byte[] bytes = add.GetBytes();
					int length = bytes.Length;
					using (MemoryStream stream = new MemoryStream(bytes, false))
					{
						this.AddBytesFromStream(stream, 0, length, false, true);
					}

					// Move the ver position.
					this.versionPosition += length;
				}
				else
				{
					Copy copy = (Copy)entry;

					if (this.basePosition < copy.BaseOffset)
					{
						// Add bytes to BaseLines from this.iBasePos to C.iBaseOffset-1
						int length = copy.BaseOffset - this.basePosition;
						this.AddBytesFromStream(baseFile, this.basePosition, length, true, false);
						this.basePosition += length;
					}

					// Add copied bytes to both sets of lines.
					this.AddBytesFromStream(baseFile, copy.BaseOffset, copy.Length, true, true);

					// Move the base and version positions.
					this.basePosition = copy.BaseOffset + copy.Length;
					this.versionPosition += copy.Length;
				}
			}

			int baseLength = (int)baseFile.Length;
			if (this.basePosition < baseLength)
			{
				// Add bytes to BaseLines from this.iBasePos to this.Base.Length
				this.AddBytesFromStream(baseFile, this.basePosition, baseLength - this.basePosition, true, false);
			}
		}

		#endregion

		#region Public Properties

		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BaseLines",
			Justification = "This does not refer to graphical 'baselines'.")]
		public IList<string> BaseLines => this.baseLines;

		public IList<string> VersionLines => this.versionLines;

		#endregion

		#region Private Methods

		private static string GetPositionString(int position) => string.Format("{0:X8}", position);

		private void AddBytesFromStream(Stream stream, int position, int length, bool addToBase, bool addToVer)
		{
			stream.Seek(position, SeekOrigin.Begin);

			// Figure out the number of lines we'll have
			int numLines = length / this.bytesPerLine;
			if (length % this.bytesPerLine != 0)
			{
				numLines++;
			}

			// Keep up with different positions for each line type
			int baseLinePos = this.basePosition;
			int verLinePos = this.versionPosition;

			// Build each line and add it to the appropriate collections.
			int remainingLength = length;
			for (int lineIndex = 0; lineIndex < numLines; lineIndex++)
			{
				// Get the line text.
				int lineLength = Math.Min(remainingLength, this.bytesPerLine);
				string line = this.GetLineString(stream, lineLength);
				remainingLength -= lineLength;

				// Add it to the BaseLines
				if (addToBase)
				{
					this.baseLines.Add(GetPositionString(baseLinePos) + line);
					baseLinePos += lineLength;
				}

				// Add it to the VerLines
				if (addToVer)
				{
					this.versionLines.Add(GetPositionString(verLinePos) + line);
					verLinePos += lineLength;
				}
			}
		}

		private string GetLineString(Stream stream, int length)
		{
			// The magic number 3 appears in this method because each
			// byte takes two hex characters plus a space after it.
			StringBuilder hexBuilder = new StringBuilder(length * 3);
			StringBuilder charBuilder = new StringBuilder(length);

			for (int i = 0; i < length; i++)
			{
				int byteValue = stream.ReadByte();
				if (byteValue == -1)
				{
					hexBuilder.Append("   ");
				}
				else
				{
					byte by = (byte)byteValue;
					hexBuilder.AppendFormat("{0:X2} ", by);
					char ch = (char)by;
					charBuilder.Append(char.IsControl(ch) ? '.' : ch);
				}
			}

			while (hexBuilder.Length < 3 * this.bytesPerLine)
			{
				hexBuilder.Append("   ");
			}

			return string.Concat("    ", hexBuilder.ToString(), "   ", charBuilder.ToString());
		}

		#endregion
	}
}
