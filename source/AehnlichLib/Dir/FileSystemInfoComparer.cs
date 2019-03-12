namespace AehnlichLib.Dir
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Implements an internal class with static comparers for files and directories
    /// to determine whehter files or directories are equal or not (based on their case-insensitive name).
    /// </summary>
    internal class FileSystemInfoComparer : IComparer<FileSystemInfo>, IComparer<FileInfo>, IComparer<DirectoryInfo>
	{
        #region Public Fields
        /// <summary>
        /// Determines whether 2 <see cref="FileSystemInfo"/> objects are equal based
        /// on comparing their names with case-insensitivity.
        /// </summary>
        public static readonly FileSystemInfoComparer Comparer = new FileSystemInfoComparer();

        /// <summary>
        /// Determines whether 2 <see cref="FileInfo"/> objects are equal based
        /// on comparing their names with case-insensitivity.
        /// </summary>
		public static readonly IComparer<FileInfo> FileComparer = Comparer;

        /// <summary>
        /// Determines whether 2 <see cref="DirectoryInfo"/> objects are equal based
        /// on comparing their names with case-insensitivity.
        /// </summary>
		public static readonly IComparer<DirectoryInfo> DirectoryComparer = Comparer;
		#endregion

		#region Public Methods
        /// <summary>
        /// Determines whether 2 <see cref="FileSystemInfo"/> objects are equal based
        /// on comparing their names with case-insensitivity.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
		public int Compare(FileSystemInfo x, FileSystemInfo y) => CompareInfo(x, y);

        /// <summary>
        /// Determines whether 2 <see cref="FileInfo"/> objects are equal based
        /// on comparing their names with case-insensitivity.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
		public int Compare(FileInfo x, FileInfo y) => CompareInfo(x, y);

        /// <summary>
        /// Determines whether 2 <see cref="DirectoryInfo"/> objects are equal based
        /// on comparing their names with case-insensitivity.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
		public int Compare(DirectoryInfo x, DirectoryInfo y) => CompareInfo(x, y);

        #endregion

        #region Private Methods
        /// <summary>
        /// Method can be used to compare objects that inherate from <see cref="FileSystemInfo"/>
        /// based on their case insensitive name.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static int CompareInfo(FileSystemInfo x, FileSystemInfo y) => string.Compare(x.Name, y.Name, true);

		#endregion
	}
}
