namespace AehnlichDirViewModelLib.Interfaces
{
	using AehnlichLib.Enums;

	/// <summary>
	/// Implements an interface to a viewmodel for a directory comparison mode that is used
	/// to compare one directory and its contents to another directory.
	/// 
	/// Each mode is based on (combination) of a set of properties:
	/// 1- File Length
	/// 2- Modification Date, and or
	/// 3- Byte-by-Byte Comparison
	/// 
	/// The first two modes (or a combination of them) is usually very fast
	/// and yields the same results as the last mode (or a combination with the last mode).
	/// 
	/// But the last mode is very slow by comparison which is why it makes sense to offer
	/// these as a user controlled option.
	/// </summary>
	public interface IDiffFileModeItemViewModel
	{
		/// <summary>
		/// Gets a display name of this item.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a diescription of this item.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets a <see cref="DiffDirFileMode"/> based a key value
		/// that is unique for the combination of file comparison modes defined in this item.
		/// </summary>
		DiffDirFileMode ModeKey { get; }

		/// <summary>
		/// Gets an <see cref="uint"/> based key value that is unique
		/// for the combination of file comparison modes defined in this item.
		/// </summary>
		uint Key { get; }

	}
}
