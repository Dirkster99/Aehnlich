namespace FsDataLib.Enums
{
	/// <summary>Enumerates types that can be applicable to a given file.</summary>
	public enum FileType
	{
		/// <summary>The type of file is not known, yet.
		/// This is a default that should normally never be encountered in real life.</summary>
		Unknown,

		/// <summary>The file does not exist in file system and can those not be described in more detail.</summary>
		NotExisting,

		/// <summary>The file is a binary file (e.: image, sound, video).</summary>
		Binary,

		/// <summary>The file is a text file (e.: C# code, mark down, txt).</summary>
		Text,

		/// <summary>The file is an XML file.</summary>
		Xml
	}
}
