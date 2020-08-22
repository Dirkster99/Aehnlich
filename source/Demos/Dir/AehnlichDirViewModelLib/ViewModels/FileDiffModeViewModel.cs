namespace AehnlichDirViewModelLib.ViewModels
{
	using AehnlichDirViewModelLib.Interfaces;
	using FsDataLib.Enums;
	using System.Collections.Generic;

	/// <summary>
	/// Implements a viewmodel that manages a list of directory diff modes
	/// (eg.: 'File Length', 'Last Change', 'File Length + Last Change', ... 'All Bytes')
	/// and their strings for binding with ItemControls, such as, ComboBox etc.
	/// 
	/// The selected diff mode from this viewmodel determines the diff strategy/properties used
	/// when each left file is compared with its right file in the directory diff to determine
	/// whether two files existing on both sides are flagged different or not.
	/// </summary>
	internal class FileDiffModeViewModel : Base.ViewModelBase, IFileDiffModeViewModel
	{
		#region fields
		private readonly List<IDiffFileModeItemViewModel> _DiffFileModes;
		private IDiffFileModeItemViewModel _DiffFileModeSelected;
		#endregion fields

		#region ctors
		/// <summary>
		/// Class constructor
		/// </summary>
		public FileDiffModeViewModel()
		{
			_DiffFileModes = new List<IDiffFileModeItemViewModel>();
			_DiffFileModeSelected = CreateCompareFileModes(_DiffFileModes);
		}
		#endregion ctors

		#region properties
		/// <summary>
		/// Gets a list of modies that can be used to compare one directory
		/// and its contents, to the other directory.
		/// </summary>
		public List<IDiffFileModeItemViewModel> DiffFileModes
		{
			get
			{
				return _DiffFileModes;
			}
		}

		/// <summary>
		/// Gets/sets the mode that is currently used to compare one directory
		/// and its contents with the other directory.
		/// </summary>
		public IDiffFileModeItemViewModel DiffFileModeSelected
		{
			get
			{
				return _DiffFileModeSelected;
			}

			set
			{
				if (_DiffFileModeSelected != value)
				{
					_DiffFileModeSelected = value;
					NotifyPropertyChanged(() => DiffFileModeSelected);
				}
			}
		}
		#endregion properties

		#region methods

		private static IDiffFileModeItemViewModel CreateCompareFileModes(
			IList<IDiffFileModeItemViewModel> diffFileModes)
		{
			DiffFileModeItemViewModel defaultItem = null;

			diffFileModes.Add(
				new DiffFileModeItemViewModel("File Length",
				"Compare the byte length of each file",
				DiffDirFileMode.ByteLength));

			diffFileModes.Add(
				new DiffFileModeItemViewModel("Last Change",
				"Compare last modification time of change of each file",
				DiffDirFileMode.LastUpdate));

			defaultItem = new DiffFileModeItemViewModel("File Length + Last Change",
				"Compare the byte length and last modification time of each file",
				DiffDirFileMode.ByteLength_LastUpdate);

			diffFileModes.Add(defaultItem);

			diffFileModes.Add(new DiffFileModeItemViewModel("Last Change + File Length + All Bytes",
				"Compare each file by their length, last modification time, and byte-by-byte sequence",
				DiffDirFileMode.ByteLength_LastUpdate_AllBytes));

//// This is technically a duplicate with AllBytes
////			diffFileModes.Add(new DiffFileModeItemViewModel("Byte Length + All Bytes",
////				"Compare each file by their length and byte-by-byte sequence",
////				DiffDirFileMode.ByteLength_AllBytes));

			diffFileModes.Add(new DiffFileModeItemViewModel("All Bytes",
				"Compare each file by their length and byte-by-byte sequence",
				DiffDirFileMode.AllBytes));

			diffFileModes.Add(new DiffFileModeItemViewModel("All Bytes without LineFeeds",
				"Compare each file by their byte-by-byte sequence but ignoring different LineFeeds in text files.",
				DiffDirFileMode.ByteLength_AllBytes_IgnoreLf));

			return defaultItem;
		}
		#endregion methods
	}
}
