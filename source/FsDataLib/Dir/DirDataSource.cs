namespace FsDataLib.Dir
{
	using AehnlichLib.Interfaces.Dir;
	using System;
	using System.Diagnostics;
	using System.IO;

	/// <summary>
	/// Provides routines and objects for working with data objects that refers to directories and files.
	/// </summary>
	internal class DirDataSource : IDataSource
	{
		#region ctors
		/// <summary>
		/// Class constructor.
		/// </summary>
		public DirDataSource()
		{
		}
		#endregion ctors

		#region Members
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
							   //                              .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
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
		/// Returns false if both files are equal and true if they differ
		/// (based on a byte size comparison or byte by byte comparison).
		/// </summary>
		/// <param name="info1"></param>
		/// <param name="info2"></param>
		/// <returns></returns>
		public bool AreFilesDifferent(string fileName1, string fileName2)
		{
			return AreFilesDifferent(new FileInfoImpl(fileName1), new FileInfoImpl(fileName2));
		}

		/// <summary>
		/// Returns false if both files are equal and true if they differ
		/// (based on a byte size comparison or byte by byte comparison).
		/// </summary>
		/// <param name="ifo1"></param>
		/// <param name="ifo2"></param>
		/// <returns></returns>
		public bool AreFilesDifferent(IFileInfo ifo1, IFileInfo ifo2)
		{
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

					if (byte1 != byte2)
					{
						return true;
					}
				}
				while (byte1 >= 0 && byte2 >= 0);

				// The files were byte-by-byte equal.
				return false;
			}
		}
		#endregion Members
	}
}
