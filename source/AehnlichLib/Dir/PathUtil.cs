namespace AehnlichLib.Dir
{
    using System;
    using System.IO;

    /// <summary>
    /// Path utility helper class.
    /// </summary>
    public static class PathUtil
    {
        /// <summary>
        /// Gets a normalized path to a directory if it exists or null.
        /// </summary>
        /// <param name="leftDir"></param>
        /// <returns></returns>
        public static string GetPathIfDirExists(string leftDir)
        {
            if (string.IsNullOrEmpty(leftDir) == true)
                return null;

            if (leftDir.Length < 2)
                return null;

            var leftDirInfo = new DirectoryInfo(leftDir);

            // Return normalized path notation :-)
            if (leftDirInfo.Exists == true)
                return NormalizePath(leftDirInfo.FullName);

            return null;
        }

        /// <summary>
        /// Gets a normalized path to a directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }
    }
}
