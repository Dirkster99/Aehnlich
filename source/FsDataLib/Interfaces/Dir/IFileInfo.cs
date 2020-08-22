using FsDataLib.Enums;

namespace FsDataLib.Interfaces.Dir
{
	/// <summary>Defines methods/properties of an object representing a file.</summary>
	public interface IFileInfo : IFileSystemInfo
	{
		#region properties
		/// <summary>
		/// Gets the size, in bytes, of the current file.
		/// </summary>
		long Length { get; }

		/// <summary>Gets the type of file (binary, text, xml, not existing)</summary>
		FileType Is { get; }

		/// <summary>Gets a value indicating whether this FILE exists.</summary>
		/// <returns>true if the FILE exists; otherwise, false.</returns>
		bool FileExists { get; }
		#endregion properties

		#region methods

		#endregion methods
	}
}
