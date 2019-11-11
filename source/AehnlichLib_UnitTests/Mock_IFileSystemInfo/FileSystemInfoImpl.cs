namespace AehnlichLib_UnitTests.Mock_IFileSystemInfo
{
    using AehnlichLib.Interfaces.Dir;
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class FileSystemInfoImpl : IFileSystemInfo
    {
        #region fields
        private string _path;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="path"></param>
        public FileSystemInfoImpl(string path)
            : this()
        {
            _path = path;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        protected FileSystemInfoImpl()
        {
        }
        #endregion ctors

        #region properties

        public string FullName => _path;

        public string Name => throw new NotImplementedException();

        public bool Exists => throw new NotImplementedException();

        public DateTime LastWriteTime => throw new NotImplementedException();
        #endregion properties

        #region methods
        #endregion methods
    }
}
