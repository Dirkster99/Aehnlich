namespace AehnlichDirViewModelLib.ViewModels
{
	using AehnlichDirViewModelLib.Interfaces;
	using AehnlichLib.Enums;

	/// <summary>
	/// Implements a viewmodel for a directory comparison mode that is used
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
	internal class DiffFileModeItemViewModel : IDiffFileModeItemViewModel
	{
		#region ctors
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <param name="modeKey"></param>
		public DiffFileModeItemViewModel(string name,
										 string description,
										 DiffDirFileMode modeKey)
			: this()
		{
			this.Name = name;
			this.Description = description;
			this.ModeKey = modeKey;
		}

		/// <summary>
		/// Hidden class constructor.
		/// </summary>
		protected DiffFileModeItemViewModel()
		{
		}
		#endregion ctors

		#region properties
		/// <summary>
		/// Gets a display name of this item.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets a diescription of this item.
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Gets a <see cref="DiffDirFileMode"/> based a key value
		/// that is unique for the combination of file comparison modes defined in this item.
		/// </summary>
		public DiffDirFileMode ModeKey { get; }

		/// <summary>
		/// Gets an <see cref="uint"/> based key value that is unique
		/// for the combination of file comparison modes defined in this item.
		/// </summary>
		public uint Key { get { return (uint)ModeKey; } }
		#endregion properties
	}
}
