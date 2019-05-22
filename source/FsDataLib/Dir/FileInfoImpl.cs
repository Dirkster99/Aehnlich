namespace FsDataLib.Dir
{
    using AehnlichLib.Interfaces.Dir;

    internal class FileInfoImpl : FileSystemInfoImpl, IFileInfo
    {
        #region fields

        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public FileInfoImpl(string path)
            : base(path)
        {
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        private FileInfoImpl()
        {
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets the size, in bytes, of the current file.
        /// </summary>
        public long Length
        {
            get
            {
                var info = new System.IO.FileInfo(_path);
                return info.Length;
            }
        }
        #endregion properties

        #region methods

        #endregion methods
    }
}
