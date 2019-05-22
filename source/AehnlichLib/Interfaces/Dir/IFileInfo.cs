namespace AehnlichLib.Interfaces.Dir
{
    public interface IFileInfo : IFileSystemInfo
    {
        #region properties
        /// <summary>
        /// Gets the size, in bytes, of the current file.
        /// </summary>
        long Length { get; }
        #endregion properties

        #region methods

        #endregion methods
    }
}
