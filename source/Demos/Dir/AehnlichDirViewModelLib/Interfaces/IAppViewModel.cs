namespace AehnlichDirViewModelLib.Interfaces
{
	using AehnlichLib.Interfaces;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Windows.Input;

	/// <summary>
	/// Defines the interface for the viewmodel that implements the main application viewmodel.
	/// </summary>
	public interface IAppViewModel : INotifyPropertyChanged, System.IDisposable
	{
		#region properties
		/// <summary>
		/// Gets the viewmodel for the document that contains the diff information
		/// on a left directory (A) and a right directory (B) and its contents.
		/// </summary>
		IDirDiffDocViewModel DirDiffDoc { get; }

		#region Diff File Mode Selection
		/// <summary>
		/// Gets a viewmodel that defines a comparison strategy for files
		/// (using lastupdate, size in bytes, and/or byte-by-byte comparison)
		/// </summary>
		IFileDiffModeViewModel FileDiffMode { get; }
		#endregion

		#region CompareCommand
		/// <summary>
		/// Gets a command that refreshs (reloads) the comparison of
		/// two directories (sub-directories) and their files.
		/// </summary>
		ICommand CompareDirectoriesCommand { get; }

		/// <summary>
		/// Gets a command that can be used to cancel the directory comparison
		/// currently being processed (if any).
		/// </summary>
		ICommand CancelCompareCommand { get; }

		#endregion CompareCommand

		/// <summary>
		/// Gets the left directory path.
		/// </summary>
		string LeftDirPath { get; set; }

		/// <summary>
		/// Gets the right directory path.
		/// </summary>
		string RightDirPath { get; }

		#region File DiffMode
		/// <summary>
		/// Gets a list of view modes by which the results of the
		/// directory and file comparison can be viewed
		/// (eg.: directories and files or files only).
		/// </summary>
		IReadOnlyList<IListItemViewModel> DiffViewModes { get; }

		/// <summary>
		/// Gets the currently selected view mode (Files and Directories or Files Only)
		/// for the display of diff results.
		/// </summary>
		IListItemViewModel DiffViewModeSelected { get; }

		/// <summary>
		/// Gets a command that can be used to change the
		/// currently selected view mode for displaying diff results.
		/// </summary>
		ICommand DiffViewModeSelectCommand { get; }
		#endregion File DiffMode

		/// <summary>
		/// Gets a viewmodel that manages progress display in terms of min, value, max or
		/// indeterminate progress display.
		/// </summary>
		IDiffProgress DiffProgress { get; }
		#endregion properties

		#region methods
		/// <summary>
		/// Initializes the left and right dir from the last application session (if any)
		/// </summary>
		/// <param name="leftDirPath"></param>
		/// <param name="rightDirPath"></param>
		void Initialize(string leftDirPath, string rightDirPath);
		#endregion methods
	}
}
