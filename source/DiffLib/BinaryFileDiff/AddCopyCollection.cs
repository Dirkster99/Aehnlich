namespace DiffLib.BinaryFileDiff
{
    using DiffLib.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;

    public sealed class AddCopyCollection : ReadOnlyCollection<IAddCopy>
	{
		#region Constructors

		internal AddCopyCollection(IList<IAddCopy> list)
			: base(list)
		{
		}

		#endregion

		#region Public Members

		// Gets the total byte length of all of the adds and copies.
		// This should equal the file size of the output file.
		public int TotalByteLength
		{
			get
			{
				int total = 0;
				foreach (IAddCopy addCopy in this)
				{
					if (addCopy.IsAdd)
					{
						Addition addition = (Addition)addCopy;
						total += addition.GetBytes().Length;
					}
					else
					{
						Copy copy = (Copy)addCopy;
						total += copy.Length;
					}
				}

				return total;
			}
		}

		/// <summary>
		/// This outputs the Add/Copy info to a stream in GDIFF format.
		/// </summary>
		/// <param name="diff">The stream to dump the diff info to.  It must support at least forward-only writing.</param>
		public void GDiff(Stream diff)
		{
			// http://www.w3.org/TR/NOTE-gdiff-19970825.html
			//
			// The GDIFF format is a binary format. The mime type of a GDIFF file is "application/gdiff".
			// All binary numbers in a GDIFF file are stored in big endian format (most significant byte first).
			// Each diff stream starts with the 4-byte magic number (value 0xd1ffd1ff), followed by a 1-byte
			// version number (value 4). The version number is followed by a sequence of 1 byte commands which
			// are interpreted in order. The last command in the stream is the end-of-file command (value 0).
			//
			// byte - 8 bit signed
			// ubyte - 8 bit unsigned
			// ushort - 16 bit unsigned, most significant byte first
			// int - 32 bit signed, most significant byte first
			// long - 64 bit signed, most significant byte first

			// Write the magic number 0xd1ffd1ff ("diff diff")
			for (int i = 0; i < 2; i++)
			{
				diff.WriteByte(0xd1);
				diff.WriteByte(0xff);
			}

			// Write the version
			diff.WriteByte(0x04);

			// Write the data
			foreach (IAddCopy addCopy in this)
			{
				if (addCopy.IsAdd)
				{
					GDiffAdd(diff, (Addition)addCopy);
				}
				else
				{
					GDiffCopy(diff, (Copy)addCopy);
				}
			}

			// Write the end-of-file command
			diff.WriteByte(0x00);
		}

		#endregion

		#region Private Members

		private static void GDiffAdd(Stream diff, Addition add)
		{
			// Name	Cmd		Followed By			Action
			// -----------------------------------------------------------
			// DATA	1		1 byte				append 1 data byte
			// DATA	2		2 bytes				append 2 data bytes
			// DATA	<n>		<n> bytes			append <n> data bytes
			// DATA	246		246 bytes			append 246 data bytes
			// DATA	247		ushort, <n> bytes	append <n> data bytes
			// DATA	248		int, <n> bytes		append <n> data bytes
			int length = add.GetBytes().Length;
			if (length <= 246)
			{
				diff.WriteByte((byte)length);
				diff.Write(add.GetBytes(), 0, length);
			}
			else if (length <= ushort.MaxValue)
			{
				diff.WriteByte(247);
				WriteBigEndian(diff, (ushort)length);
				diff.Write(add.GetBytes(), 0, length);
			}
			else
			{
				diff.WriteByte(248);
				WriteBigEndian(diff, length);
				diff.Write(add.GetBytes(), 0, length);
			}
		}

		private static void GDiffCopy(Stream diff, Copy copy)
		{
			// Name	Cmd		Followed By			Action
			// -----------------------------------------------------------
			// COPY	249		ushort, ubyte		copy <position>, <length>
			// COPY	250		ushort, ushort		copy <position>, <length>
			// COPY	251		ushort, int			copy <position>, <length>
			// COPY	252		int, ubyte			copy <position>, <length>
			// COPY	253		int, ushort			copy <position>, <length>
			// COPY	254		int, int			copy <position>, <length>
			// COPY	255		long, int			copy <position>, <length>
			if (copy.BaseOffset <= ushort.MaxValue)
			{
				if (copy.Length <= byte.MaxValue)
				{
					diff.WriteByte(249);
					WriteBigEndian(diff, (ushort)copy.BaseOffset);
					diff.WriteByte((byte)copy.Length);
				}
				else if (copy.Length <= ushort.MaxValue)
				{
					diff.WriteByte(250);
					WriteBigEndian(diff, (ushort)copy.BaseOffset);
					WriteBigEndian(diff, (ushort)copy.Length);
				}
				else
				{
					diff.WriteByte(251);
					WriteBigEndian(diff, (ushort)copy.BaseOffset);
					WriteBigEndian(diff, copy.Length);
				}
			}
			else
			{
				if (copy.Length <= byte.MaxValue)
				{
					diff.WriteByte(252);
					WriteBigEndian(diff, copy.BaseOffset);
					diff.WriteByte((byte)copy.Length);
				}
				else if (copy.Length <= ushort.MaxValue)
				{
					diff.WriteByte(253);
					WriteBigEndian(diff, copy.BaseOffset);
					WriteBigEndian(diff, (ushort)copy.Length);
				}
				else
				{
					diff.WriteByte(254);
					WriteBigEndian(diff, copy.BaseOffset);
					WriteBigEndian(diff, copy.Length);
				}
			}
		}

		private static void WriteBigEndian(Stream diff, ushort value)
		{
			WriteBigEndian(diff, BitConverter.GetBytes(value));
		}

		private static void WriteBigEndian(Stream diff, int value)
		{
			WriteBigEndian(diff, BitConverter.GetBytes(value));
		}

		private static void WriteBigEndian(Stream diff, byte[] bytes)
		{
			if (BitConverter.IsLittleEndian)
			{
				for (int i = bytes.Length - 1; i >= 0; i--)
				{
					diff.WriteByte(bytes[i]);
				}
			}
			else
			{
				diff.Write(bytes, 0, bytes.Length);
			}
		}

		#endregion
	}
}
