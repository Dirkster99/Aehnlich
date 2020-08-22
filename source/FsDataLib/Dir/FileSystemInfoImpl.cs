namespace FsDataLib.Dir
{
	using FsDataLib.Interfaces.Dir;
	using System;

	/// <summary>Serves as a base class for common properties and methods of classes that
	/// model files and directories.</summary>
	internal class FileSystemInfoImpl : IFileSystemInfo
	{
		#region fields
		protected readonly string _path;
		#endregion fields

		#region ctors
		/// <summary>Class constructor</summary>
		/// <param name="path"></param>
		public FileSystemInfoImpl(string path)
			: this()
		{
			_path = path;
		}

		/// <summary>Class constructor</summary>
		protected FileSystemInfoImpl()
		{
		}
		#endregion ctors

		#region properties
		/// <summary>
		/// Gets the full path of the directory or file.
		///
		/// Exceptions:
		///   T:System.IO.PathTooLongException:
		///     The fully qualified path and file name is 260 or more characters.
		///
		///   T:System.Security.SecurityException:
		///     The caller does not have the required permission.
		/// </summary>
		/// <returns>A string containing the full path.</returns>
		public string FullName { get { return _path; } }

		/// <summary>
		/// For files, gets the name of the file. For directories, gets the name of the last
		/// directory in the hierarchy if a hierarchy exists. Otherwise, the Name property
		/// gets the name of the directory.
		/// </summary>
		/// <returns>
		/// A string that is the name of the parent directory, the name of the last directory
		/// in the hierarchy, or the name of a file, including the file name extension.
		/// </returns>
		public string Name
		{
			get
			{
				return System.IO.Path.GetFileName(_path);
			}
		}

		/// <summary>
		/// Gets the time when the current file or directory was last written to.
		///
		/// Returns:
		///     The time the current file was last written.
		///
		/// Exceptions:
		///   T:System.IO.IOException:
		///     System.IO.FileSystemInfo.Refresh cannot initialize the data.
		///
		///   T:System.PlatformNotSupportedException:
		///     The current operating system is not Windows NT or later.
		///
		///   T:System.ArgumentOutOfRangeException:
		///     The caller attempts to set an invalid write time.
		/// </summary>
		public DateTime LastWriteTime
		{
			get
			{
				var info = new System.IO.DirectoryInfo(_path);
				return info.LastWriteTime;
			}
		}
		#endregion properties

		#region methods
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
		public static string Combine(string a, string b)
		{
			return System.IO.Path.Combine(a, b);
		}

		protected System.IO.DirectoryInfo GetDirInfo()
		{
			return new System.IO.DirectoryInfo(_path);
		}
		#endregion methods
	}
}
