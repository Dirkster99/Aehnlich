namespace AehnlichLib.Dir
{
	using FsDataLib.Interfaces.Dir;
	using System.Collections.Generic;

	/// <summary>
	/// Implements an internal class with static comparers for files and directories
	/// to determine whether files or directories are equal or not (based on their case-insensitive name).
	/// </summary>
	internal class FileSystemInfoComparer : IComparer<IFileSystemInfo>, IComparer<IFileInfo>, IComparer<IDirectoryInfo>
	{
		#region Public Fields
		/// <summary>
		/// Determines whether 2 <see cref="IFileSystemInfo"/> objects are equal based
		/// on comparing their names with case-insensitivity.
		/// </summary>
		public static readonly FileSystemInfoComparer Comparer = new FileSystemInfoComparer();

		/// <summary>
		/// Determines whether 2 <see cref="IFileInfo"/> objects are equal based
		/// on comparing their names with case-insensitivity.
		/// </summary>
		public static readonly IComparer<IFileInfo> FileComparer = Comparer;

		/// <summary>
		/// Determines whether 2 <see cref="IDirectoryInfo"/> objects are equal based
		/// on comparing their names with case-insensitivity.
		/// </summary>
		public static readonly IComparer<IDirectoryInfo> DirectoryComparer = Comparer;
		#endregion

		#region Public Methods
		/// <summary>
		/// Determines whether 2 <see cref="FileSystemInfo"/> objects are equal based
		/// on comparing their names with case-insensitivity.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(IFileSystemInfo x, IFileSystemInfo y)
		{
			return CompareInfo(x, y);
		}

		/// <summary>
		/// Determines whether 2 <see cref="IFileInfo"/> objects are equal based
		/// on comparing their names with case-insensitivity.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(IFileInfo x, IFileInfo y)
		{
			return CompareInfo(x, y);
		}

		/// <summary>
		/// Determines whether 2 <see cref="IDirectoryInfo"/> objects are equal based
		/// on comparing their names with case-insensitivity.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(IDirectoryInfo x, IDirectoryInfo y)
		{
			return CompareInfo(x, y);
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Method can be used to compare objects that inherate from <see cref="FileSystemInfo"/>
		/// based on their case insensitive name.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private static int CompareInfo(IFileSystemInfo x, IFileSystemInfo y)
		{
			return string.Compare(x.Name, y.Name, true);
		}
		#endregion
	}
}
