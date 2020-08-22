namespace FsDataLib.Interfaces.Dir
{
	public enum FileType
	{
		Unknown,
		NotExisting,
		Binary,
		Text,
		Xml
	}

	public interface IFileInfo : IFileSystemInfo
	{
		#region properties
		/// <summary>
		/// Gets the size, in bytes, of the current file.
		/// </summary>
		long Length { get; }

		FileType Is { get; }

		/// <summary>Gets a value indicating whether this FILE exists.</summary>
		/// <returns>true if the FILE exists; otherwise, false.</returns>
		bool FileExists { get; }
		#endregion properties

		#region methods

		#endregion methods
	}
}
