namespace FsDataLib.Interfaces.Dir
{
	/// <summary>Defines common methods/properties of an object representing a file or directory.</summary>
	public interface IFileSystemInfo
	{
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
		string FullName { get; }

		/// <summary>
		/// For files, gets the name of the file. For directories, gets the name of the last
		/// directory in the hierarchy if a hierarchy exists. Otherwise, the Name property
		/// gets the name of the directory.
		/// </summary>
		/// <returns>
		/// A string that is the name of the parent directory, the name of the last directory
		/// in the hierarchy, or the name of a file, including the file name extension.
		/// </returns>
		string Name { get; }

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
		System.DateTime LastWriteTime { get; }
	}
}
