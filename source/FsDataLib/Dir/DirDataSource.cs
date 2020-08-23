namespace FsDataLib.Dir
{
	using FsDataLib.Enums;
	using FsDataLib.Interfaces.Dir;
	using System;
	using System.Diagnostics;
	using System.IO;

	/// <summary>
	/// Provides routines and objects for working with data objects that refers to directories and files.
	/// </summary>
	internal sealed class DirDataSource : IDataSource
	{
		#region ctors
		/// <summary>
		/// Class constructor.
		/// </summary>
		public DirDataSource()
		{
		}
		#endregion ctors

		#region methods
		/// <summary>
		/// Gets a normalized path to a directory if it exists or null.
		/// </summary>
		/// <param name="leftDir"></param>
		/// <returns></returns>
		public string GetPathIfDirExists(string leftDir)
		{
			if (string.IsNullOrEmpty(leftDir) == true)
				return null;

			if (leftDir.Length < 2)
				return null;

			try
			{
				var leftDirInfo = new DirectoryInfo(leftDir);

				// Return normalized path notation :-)
				if (leftDirInfo.Exists == true)
					return NormalizePath(leftDirInfo.FullName);
			}
			catch (Exception)
			{
				return null;
			}

			return null;
		}

		/// <summary>
		/// Gets a normalized path to a directory
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public string NormalizePath(string path)
		{
			try
			{
				if (string.IsNullOrEmpty(path) == false)
				{
					return Path.GetFullPath(new Uri(path).LocalPath)
							   //.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
							   .ToUpperInvariant();
				}
				else
					return path;

			}
			catch
			{
				return path;
			}
		}

		/// <summary>
		/// Creates an <see cref="IDirectoryInfo"/> object from the given path and returns it.
		/// </summary>
		/// <param name="path">The path to create a directory data source object for.</param>
		/// <returns>
		/// The created <see cref="IDirectoryInfo"/> object.
		/// </returns>
		public IDirectoryInfo CreateDirectory(string path)
		{
			return new DirectoryInfoImpl(path);
		}

		/// <summary>
		/// Creates an <see cref="IFileInfo"/> object from the given path and returns it.
		/// </summary>
		/// <param name="path">The path to create a file data source object for.</param>
		/// <returns>
		/// The created <see cref="IFileInfo"/> object.
		/// </returns>
		public IFileInfo CreateFile(string path)
		{
			return new FileInfoImpl(path);
		}

		/// <summary>
		/// Combines two strings into a path.
		/// </summary>
		/// <param name="a">The first path to combine.</param>
		/// <param name="b">The second path to combine.</param>
		/// <returns>
		/// The combined paths. If one of the specified paths is a zero-length string, this
		/// method returns the other path. If path2 contains an absolute path, this method
		/// returns path2.
		/// </returns>
		public string Combine(string a, string b)
		{
			return System.IO.Path.Combine(a, b);
		}

		/// <summary>
		/// Compares two file on a byte-by-byte sequence strategy.
		/// Either based on a:
		/// 1) byte size comparison and byte-by-byte comparison or
		/// 2) byte-by-byte comparison ignoring different line feed styles on text files.
		/// </summary>
		/// <param name="info1"></param>
		/// <param name="info2"></param>
		/// <param name="diffMode"></param>
		/// <returns>false if both files are equal and true if they differ</returns>
		public bool AreBinaryFilesDifferent(string fileName1, string fileName2, DiffDirFileMode diffMode)
		{
			return AreBinaryFilesDifferent(new FileInfoImpl(fileName1), new FileInfoImpl(fileName2), diffMode);
		}

		/// <summary>
		/// Compares two file with a byte-by-byte sequence strategy.
		/// Either based on a:
		/// 1) byte size comparison and byte-by-byte comparison or
		/// 2) byte-by-byte comparison ignoring different line feed styles on text files.
		/// </summary>
		/// <param name="ifo1"></param>
		/// <param name="ifo2"></param>
		/// <param name="diffMode"></param>
		/// <returns>false if both files are equal and true if they differ</returns>
		public bool AreBinaryFilesDifferent(IFileInfo ifo1, IFileInfo ifo2, DiffDirFileMode diffMode)
		{
			// Should we ignore different linefeeds on text files and is this type of matching applicable here?
			if ((diffMode & DiffDirFileMode.IgnoreLf) != 0)
			{
				bool IsIfo1Text = ifo1.Is == FileType.Text | ifo1.Is == FileType.Xml;
				bool IsIfo2Text = ifo2.Is == FileType.Text | ifo2.Is == FileType.Xml;

				if (IsIfo1Text && IsIfo2Text)
					return AreTextFilesIgnoringLF_Different(ifo1, ifo2);
			}

			// Before we open the files, compare the sizes.  If they are different,
			// then the files are certainly different.
			if (ifo1.Length != ifo2.Length)
				return true;

			var info1 = new FileInfo(ifo1.FullName);
			var info2 = new FileInfo(ifo2.FullName);

			using (FileStream stream1 = info1.OpenRead())
			using (FileStream stream2 = info2.OpenRead())
			{
				// The previous length check should ensure these are equal.
				Debug.Assert(stream1.Length == stream2.Length, "The streams' lengths must be the same.");

				// They have the same lengths, so we have to check byte-by-byte.  As soon as we find a difference, we can quit.
				int byte1, byte2;
				do
				{
					byte1 = stream1.ReadByte();
					byte2 = stream2.ReadByte();

					Debug.Assert(byte1 <= 255, "Byte1 size is larger than 8 bit.");
					Debug.Assert(byte2 <= 255, "Byte2 size is larger than 8 bit.");

					if (byte1 != byte2)
					{
						return true;  // The files are different
					}
				}
				while (byte1 >= 0 && byte2 >= 0);

				return false; // The files are byte-by-byte equal.
			}
		}

		/// <summary>Compares two text files with a byte-by-byte sequence strategy.
		/// Based on a: byte-by-byte comparison ignoring different line feed styles on text files.</summary>
		/// <param name="ifo1"></param>
		/// <param name="ifo2"></param>
		/// <returns></returns>
		internal bool AreTextFilesIgnoringLF_Different(IFileInfo ifo1, IFileInfo ifo2)
		{
			var info1 = new FileInfo(ifo1.FullName);
			var info2 = new FileInfo(ifo2.FullName);

			using (FileStream stream1 = info1.OpenRead())
			using (FileStream stream2 = info2.OpenRead())
			{
				// We have to check byte-by-byte.  As soon as we find a significant difference, we can quit.
				int byte1, byte2;
				byte1 = stream1.ReadByte();
				byte2 = stream2.ReadByte();

				while (byte1 >= 0 && byte2 >= 0)
				{
					Debug.Assert(byte1 <= 255, "Byte1 size is larger than 8 bit.");
					Debug.Assert(byte2 <= 255, "Byte2 size is larger than 8 bit.");

					if (byte1 != byte2)
					{
						// Advance both sequences beyond known LineFeed bytes (any LineFeed byte (eg 0x0A) is considered equal any other (eg 0x0D))
						if ((byte1 == 0x0a || byte1 == 0x0d) && (byte2 == 0x0a || byte2 == 0x0d))
						{
							while (byte1 == 0x0a || byte1 == 0x0d)
								byte1 = stream1.ReadByte();

							while (byte2 == 0x0a || byte1 == 0x0d)
								byte2 = stream2.ReadByte();
						}
						else
						{
							return true;  // Files are not equal
						}
					}
					else
					{
						byte1 = stream1.ReadByte();
						byte2 = stream2.ReadByte();
					}
				}
				
				return false; // The files are considered equal.
			}
		}
		#endregion methods
	}
}
