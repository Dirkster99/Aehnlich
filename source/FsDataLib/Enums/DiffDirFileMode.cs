namespace FsDataLib.Enums
{
	/// <summary>
	/// Determines the mode of matching that is implemented to determine whether two files
	/// in two different directories are equal or not.
	/// 
	/// This comparison mode is a mix of different permutations where those modes with
	/// <see cref="DiffDirFileMode.AllBytes"/> are the most precise but slowest methods
	/// while
	/// <see cref="DiffDirFileMode.ByteLength_LastUpdate"/> is very fast
	/// but will miss the UNLIKELY event where files that have (somehow) been changed
	/// in their sequence of bytes
	///  1) without changing the last access time in Windows and
	///  2) without changing the total file length in bytes.
	/// </summary>
	public enum DiffDirFileMode : uint
	{
		/// <summary>
		/// Two files in the same relative location A (left dir tree) and B (right dir tree)
		/// are considered equal if:
		/// 0) their file name is the same and
		/// 1) their byte length is equal. 
		/// </summary>
		ByteLength = 1,

		/// <summary>
		/// Two files in the same relative location A (left dir tree) and B (right dir tree)
		/// are considered equal if:
		/// 0) their file name is the same and
		/// 1) their last change time is the same. 
		/// </summary>
		LastUpdate = 2,

		/// <summary>
		/// Two files in the same relative location A (left dir tree) and B (right dir tree)
		/// are considered equal if:
		/// 0) their file name is the same and
		/// 1) their byte length is equal and
		/// 2) their byte by byte sequence is equal. 
		/// </summary>
		AllBytes = 4,

		/// <summary>
		/// Two files in the same relative location A (left dir tree) and B (right dir tree)
		/// are considered equal if:
		/// 0) their file name is the same and
		/// 1) their byte length is equal and
		/// 2) their date and time of last change is equal.
		/// </summary>
		ByteLength_LastUpdate = (ByteLength | LastUpdate),

//// This is technically a duplicate with AllBytes
////		/// <summary>
////		/// Two files in the same relative location A (left dir tree) and B (right dir tree)
////		/// are considered equal if:
////		/// 0) their file name is the same and
////		/// 1) their byte length is equal and
////		/// 2) their byte by byte sequence is equal.
////		/// </summary>
////		ByteLength_AllBytes = (ByteLength | AllBytes),

		/// <summary>Text file matching treads Cr, Lf and LfCr bytes as equal sequences.</summary>
		IgnoreLf = 0x08,

		IgnoreWSP = 0x10,

		/// <summary>
		/// Two files in the same relative location A (left dir tree) and B (right dir tree)
		/// are considered equal if:
		/// 0) their file name is the same and
		/// 1) The file is binary (image, sound, video):
		///    1.1) their byte length is equal and
		///    1.2) their byte by byte sequence is equal.
		///
		/// 1) The file is text (C# source, mark down, txt):
		///    1.1) their byte by byte sequence is equal
		///         matching Cr, Lf and LfCr bytes as equal sequences
		/// </summary>
		ByteLength_AllBytes_IgnoreLf = (AllBytes | IgnoreLf),

		ByteLength_AllBytes_IgnoreLf_WSP = (AllBytes | IgnoreLf | IgnoreWSP),

		/// <summary>
		/// Two files in the same relative location A (left dir tree) and B (right dir tree)
		/// are considered equal:
		/// 0) if their file name is the same and
		/// 1) their byte by byte sequence is equal and
		/// 2) their date and time of last change is equal.
		/// </summary>
		LastUpdate_AllBytes = (LastUpdate | AllBytes),

		/// <summary>
		/// Two files in the same relative location A (left dir tree) and B (right dir tree)
		/// are considered equal:
		/// 0) if their file name is the same and
		/// 1) their byte length is equal and
		/// 2) their date and time of last change is equal and
		/// 3) their byte by byte sequence is equal.
		/// </summary>
		ByteLength_LastUpdate_AllBytes = (ByteLength | LastUpdate | AllBytes)
	}
}
