using System;

namespace AehnlichLib.Interfaces.Dir
{
    internal interface IFileSystemInfo
    {
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
        /// Gets a value indicating whether the directory exists.
        /// </summary>
        /// <returns>true if the directory exists; otherwise, false.</returns>
        bool Exists { get; }

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
        DateTime LastWriteTime { get; }
        #endregion properties

        #region methods

        #endregion methods
    }
}
