namespace DiffLib.Dir
{
    #region Using Directives

    using System.Collections.Generic;
    using System.IO;

    #endregion

    internal class FileSystemInfoComparer : IComparer<FileSystemInfo>, IComparer<FileInfo>, IComparer<DirectoryInfo>
	{
		#region Public Fields

		public static readonly FileSystemInfoComparer Comparer = new FileSystemInfoComparer();

		public static readonly IComparer<DirectoryInfo> DirectoryComparer = Comparer;

		public static readonly IComparer<FileInfo> FileComparer = Comparer;

		#endregion

		#region Public Methods

		public int Compare(FileSystemInfo x, FileSystemInfo y) => CompareInfo(x, y);

		public int Compare(FileInfo x, FileInfo y) => CompareInfo(x, y);

		public int Compare(DirectoryInfo x, DirectoryInfo y) => CompareInfo(x, y);

		#endregion

		#region Private Methods

		private static int CompareInfo(FileSystemInfo x, FileSystemInfo y) => string.Compare(x.Name, y.Name, true);

		#endregion
	}
}
