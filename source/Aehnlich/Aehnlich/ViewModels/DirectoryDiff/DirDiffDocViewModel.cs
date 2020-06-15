namespace Aehnlich.ViewModels.Documents
{
	using Aehnlich.Interfaces;
	using AehnlichLib.Interfaces.Dir;
	using System;
	using System.Windows.Input;

	/// <summary>
	/// Enumerates the stages (modes) of a <see cref="DirDiffDocViewModel"/>.
	/// </summary>
	internal enum DirDiffDocStages
	{
		/// <summary>
		/// Give user a chance to supply parameters and edit options.
		/// </summary>
		DirDiffSetup,

		/// <summary>
		/// Review results and work with the actual directory compare data.
		/// </summary>
		DirDiffView
	}

	/// <summary>
	/// Implements a document viewmodel for documents that display directory diff information.
	/// 
	/// This information is dependent on setup parameters (eg: left path and right path)
	/// which can be defined using the <see cref="DirDiffDocSetupViewModel"/> which is
	/// exposed via the <see cref="SelectedDirDiffItem"/> property.
	/// 
	/// The <see cref="SelectedDirDiffItem"/> property can also contain a <see cref="DirDiffDocViewViewModel"/>
	/// to display the results of the processing after the setup has taken place.
	/// 
	/// Summary: The viewmodel supports two stages (setup and view) - the current stage and associated
	/// viewmodel can be determined via the:
	/// - <see cref="CurrentStage"/> and
	/// - <see cref="SelectedDirDiffItem"/> properties.
	/// </summary>
	internal class DirDiffDocViewModel : DocumentBaseViewModel
	{
		#region fields
		public const string SetupContentID = "_Compare Directories_";

		private ICommand _CloseCommand;
		private DirDiffDocStages _CurrentStage;
		private object _SelectedDirDiffItem;

		private readonly IDocumentManagerViewModel _DocumentManager;
		private readonly IDataSource _DirDataSource;
		#endregion fields

		#region ctors
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="docManager"></param>
		/// <param name="leftDirPath"></param>
		/// <param name="rightDirPath"></param>
		public DirDiffDocViewModel(IDocumentManagerViewModel docManager,
								   string leftDirPath,
								   string rightDirPath,
								   IDataSource dirDataSource)
			: this()
		{
			_DocumentManager = docManager;
			_DirDataSource = dirDataSource;

			Title = "Compare Directories";
			ContentId = SetupContentID;

			_CurrentStage = DirDiffDocStages.DirDiffSetup;

			var createViewPageCommand = new Base.RelayCommand<object>(
				(p) => CreatePageViewCommand_Executed(p),
				(p) => CreatePageViewCommand_CanExecute(p));

			_SelectedDirDiffItem =
				new DirDiffDocSetupViewModel(createViewPageCommand,
											 leftDirPath, rightDirPath, dirDataSource);
		}

		/// <summary>
		/// Hidden standard clas constructor
		/// </summary>
		protected DirDiffDocViewModel()
			: base()
		{
		}
		#endregion ctors

		#region properties
		/// <summary>
		/// Gets a command that is invoked when the user clicks the document tab close button.
		/// </summary>
		public override ICommand CloseCommand
		{
			get
			{
				if (_CloseCommand == null)
				{
					_CloseCommand = new Base.RelayCommand<object>(
					(p) =>
					{
						SaveSettings();
						_DocumentManager.CloseDocument(this);
					},
					(p) =>
					{
						var asyncProc = SelectedDirDiffItem as DirDiffDocViewViewModel;
						if (asyncProc != null)
						{
							if (asyncProc.DirDiffDoc.DiffProgress.IsProgressbarVisible == true)
								return false;
						}

						return true;
					});
				}

				return _CloseCommand;
			}
		}

		/// <summary>
		/// Gets a command that is invoked when a document is saved.
		/// 
		/// Returning null here should disable controls that bind to this command,
		/// which is just what we need for documents that cannot save any data (eg.: Start Page).
		/// </summary>
		public override ICommand SaveCommand { get { return null; } }

		/// <summary>
		/// Gets the stage that is currently managed in this viewmodel.
		/// </summary>
		public DirDiffDocStages CurrentStage
		{
			get { return _CurrentStage; }
			private set
			{
				if (_CurrentStage != value)
				{
					_CurrentStage = value;
					NotifyPropertyChanged(() => CurrentStage);
				}
			}
		}

		/// <summary>
		/// Gets the viewmodel that is associated with the <see cref="CurrentStage"/>
		/// to represent all relevant data for display in the UI.
		/// </summary>
		public object SelectedDirDiffItem
		{
			get { return _SelectedDirDiffItem; }
			private set
			{
				if (_SelectedDirDiffItem != value)
				{
					_SelectedDirDiffItem = value;
					NotifyPropertyChanged(() => SelectedDirDiffItem);
				}
			}
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Invoke this method before application shut down
		/// to save all relevant user settings for recovery on appliaction re-start.
		/// </summary>
		public override void SaveSettings()
		{
			var compare = this.SelectedDirDiffItem as DirDiffDocViewViewModel;
			if (compare != null)
			{
				Properties.Settings.Default.LeftDirPath = compare.DirDiffDoc.LeftDirPath;
				Properties.Settings.Default.RightDirPath = compare.DirDiffDoc.RightDirPath;
			}
		}

		private bool CreatePageViewCommand_CanExecute(object parameter)
		{
			var DirDiff = SelectedDirDiffItem as DirDiffDocSetupViewModel;
			if (DirDiff == null)
				return false;

			try
			{
				if (System.IO.Directory.Exists(DirDiff.LeftDirectoryPath) == false)
					return false;

				if (System.IO.Directory.Exists(DirDiff.RightDirectoryPath) == false)
					return false;
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Implements a command that closes the setup view(model) and creates the
		/// view(model) that is actually processing the differences between the given directories.
		/// </summary>
		/// <param name="parameter"></param>
		private void CreatePageViewCommand_Executed(object parameter)
		{
			switch (this.CurrentStage)
			{
				case DirDiffDocStages.DirDiffSetup:
					// Get setup from Directory comapre setup viewmodel
					var setupPage = SelectedDirDiffItem as DirDiffDocSetupViewModel;

					if (setupPage != null)
					{
						ContentId = SetupContentID + Guid.NewGuid().ToString();

						setupPage.NormalizePaths();

						Title = GetTitle(setupPage.LeftDirectoryPath, setupPage.RightDirectoryPath, false);
						ToolTip = GetTooltip(setupPage.LeftDirectoryPath, setupPage.RightDirectoryPath); ;

						// Subscripe document manager to diff file open event
						var newPage = new DirDiffDocViewViewModel(_DocumentManager.DocDiffDoc_CompareFilesRequest
																 , setupPage.GetDirDiffSetup());

						SelectedDirDiffItem = newPage;
						CurrentStage = DirDiffDocStages.DirDiffView;
					}
					break;

				case DirDiffDocStages.DirDiffView:
				default:
					break;
			}

		}
		#endregion methods
	}
}
