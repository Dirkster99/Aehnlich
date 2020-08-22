namespace FsDataLib.Interfaces.Dir
{
	public interface IDirectoryInfo : IFileSystemInfo
	{
		/// <summary>Gets a value indicating whether this DIRECTORY exists.</summary>
		/// <returns>true if the directory exists; otherwise, false.</returns>
		bool DirectoryExists { get; }

		#region methods
		/// <summary>
		/// Returns the subdirectories of the current directory.
		///
		/// Exceptions:
		///   T:System.IO.DirectoryNotFoundException:
		///     The path encapsulated in the System.IO.DirectoryInfo object is invalid, such
		///     as being on an unmapped drive.
		///
		///   T:System.Security.SecurityException:
		///     The caller does not have the required permission.
		///
		///   T:System.UnauthorizedAccessException:
		///     The caller does not have the required permission.
		/// </summary>
		/// <returns>An array of <see cref="IDirectoryInfo"/> objects.</returns>
		IDirectoryInfo[] GetDirectories();

		/// <summary>
		///     Returns an array of directories in the current System.IO.DirectoryInfo matching
		///     the given search criteria.
		///     
		/// Exceptions:
		///   T:System.ArgumentException:
		///     searchPattern contains one or more invalid characters defined by the System.IO.Path.GetInvalidPathChars
		///     method.
		///
		///   T:System.ArgumentNullException:
		///     searchPattern is null.
		///
		///   T:System.IO.DirectoryNotFoundException:
		///     The path encapsulated in the DirectoryInfo object is invalid (for example, it
		///     is on an unmapped drive).
		///
		///   T:System.UnauthorizedAccessException:
		///     The caller does not have the required permission.
		/// </summary>
		/// <param name="searchPattern">
		///     The search string to match against the names of directories. This parameter can
		///     contain a combination of valid literal path and wildcard (* and ?) characters
		///     (see Remarks), but doesn't support regular expressions. The default pattern is
		///     "*", which returns all files.
		/// </param>
		/// <returns>An array of type DirectoryInfo matching searchPattern.</returns>
		IDirectoryInfo[] GetDirectories(string searchPattern);

		/// <summary>
		/// Returns a file list from the current directory matching the given search pattern.
		///
		/// Exceptions:
		///   T:System.ArgumentException:
		///     searchPattern contains one or more invalid characters defined by the System.IO.Path.GetInvalidPathChars
		///     method.
		///
		///   T:System.ArgumentNullException:
		///     searchPattern is null.
		///
		///   T:System.IO.DirectoryNotFoundException:
		///     The path is invalid (for example, it is on an unmapped drive).
		///
		///   T:System.Security.SecurityException:
		///     The caller does not have the required permission.
		/// </summary>
		/// <returns>An array of type <see cref="IFileInfo"/>.</returns>
		IFileInfo[] GetFiles();

		/// <summary>
		/// Returns a file list from the current directory matching the given search pattern.
		///
		/// Exceptions:
		///   T:System.ArgumentException:
		///     searchPattern contains one or more invalid characters defined by the System.IO.Path.GetInvalidPathChars
		///     method.
		///
		///   T:System.ArgumentNullException:
		///     searchPattern is null.
		///
		///   T:System.IO.DirectoryNotFoundException:
		///     The path is invalid (for example, it is on an unmapped drive).
		///
		///   T:System.Security.SecurityException:
		///     The caller does not have the required permission.
		/// </summary>
		/// <param name="searchPattern">
		/// The search string to match against the names of files. This parameter can contain
		/// a combination of valid literal path and wildcard (* and ?) characters (see Remarks),
		/// but doesn't support regular expressions. The default pattern is "*", which returns
		/// all files.
		/// </param>
		/// <returns>An array of type <see cref="IFileInfo"/>.</returns>
		IFileInfo[] GetFiles(string searchPattern);
		#endregion methods
	}
}
