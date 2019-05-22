namespace FsDataLib.Dir
{
    using AehnlichLib.Interfaces.Dir;
    using System.Collections.Generic;

    internal class DirectoryInfoImpl : FileSystemInfoImpl, IDirectoryInfo
    {
        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DirectoryInfoImpl(string path)
            : base(path)
        {
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        private DirectoryInfoImpl()
        {
        }
        #endregion ctors

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
        public IDirectoryInfo[] GetDirectories()
        {
            return GetDirectories("*");
        }

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
        public IDirectoryInfo[] GetDirectories(string searchPattern)
        {
            string[] t = null;

            t = System.IO.Directory.GetDirectories(FullName, searchPattern);

            IDirectoryInfo[] ret = new DirectoryInfoImpl[t.Length];

            for (int i = 0; i < t.Length; i++)
            {
                ret[i] = new DirectoryInfoImpl(t[i]);
            }

            return ret;
        }

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
        public IFileInfo[] GetFiles()
        {
            return GetFiles("*");
        }

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
        public IFileInfo[] GetFiles(string searchPattern)
        {
            var info = GetDirInfo();
            List<IFileInfo> files = new List<IFileInfo>();

            foreach (var item in info.GetFiles(searchPattern))
            {
                files.Add(new FileInfoImpl(item.FullName));
            }

            return files.ToArray();
        }

        #endregion methods
    }
}
