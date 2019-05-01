namespace Aehnlich.ViewModels
{
    using MLib.Interfaces;
    using Models;
    using Settings.Interfaces;
    using Settings.UserProfile;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Input;

    /// <summary>
    /// Implements application life cycle relevant properties and methods,
    /// such as: state for shutdown, shutdown_cancel, command for shutdown,
    /// and methods for save and load application configuration.
    /// </summary>
    public class AppLifeCycleViewModel : Base.ViewModelBase
    {
        #region fields
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool? _DialogCloseResult = null;
        private bool _ShutDownInProgress = false;
        private bool _ShutDownInProgress_Cancel = false;

        private ICommand mExitApp = null;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets a string for display of the application title.
        /// </summary>
        public string Application_Title
        {
            get
            {
                return Models.AppCore.Application_Title;
            }
        }

        /// <summary>
        /// Get path and file name to application specific settings file
        /// </summary>
        public string DirFileAppSettingsData
        {
            get
            {
                return System.IO.Path.Combine(Models.AppCore.DirAppData,
                                              string.Format(CultureInfo.InvariantCulture, "{0}.App.settings",
                                              Models.AppCore.AssemblyTitle));
            }
        }

        /// <summary>
        /// This can be used to close the attached view via ViewModel
        /// 
        /// Source: http://stackoverflow.com/questions/501886/wpf-mvvm-newbie-how-should-the-viewmodel-close-the-form
        /// </summary>
        public bool? DialogCloseResult
        {
            get
            {
                return _DialogCloseResult;
            }

            private set
            {
                if (_DialogCloseResult != value)
                {
                    _DialogCloseResult = value;
                    NotifyPropertyChanged(() => DialogCloseResult);
                }
            }
        }

        /// <summary>
        /// Gets a command to exit (end) the application.
        /// </summary>
        public ICommand ExitApp
        {
            get
            {
                if (mExitApp == null)
                {
                    mExitApp = new Base.RelayCommand<object>((p) => AppExit_CommandExecuted(),
                                                             (p) => Closing_CanExecute());
                }

                return mExitApp;
            }
        }

        /// <summary>
        /// Get a path to the directory where the user store his documents
        /// </summary>
        public static string MyDocumentsUserDir
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

        public bool ShutDownInProgress_Cancel
        {
            get
            {
                return _ShutDownInProgress_Cancel;
            }

            set
            {
                if (_ShutDownInProgress_Cancel != value)
                    _ShutDownInProgress_Cancel = value;
            }
        }
        #endregion properties

        #region methods
        private void CreateDefaultsSettings(ISettingsManager settings
                                          , IAppearanceManager appearance)
        {
            try
            {
                // Add default themings for Dark and Light
                appearance.SetDefaultThemes(settings.Themes);

                // Add additional Dark resources to those theme resources added above
                appearance.AddThemeResources("Dark", new List<Uri>
                {
                  new Uri("/MWindowLib;component/Themes/DarkTheme.xaml", UriKind.RelativeOrAbsolute)
                 ,new Uri("/Aehnlich;component/BindToMLib/MWindowLib/DarkLightBrushs.xaml", UriKind.RelativeOrAbsolute)
                 ,new Uri("/Xceed.Wpf.AvalonDock.Themes.VS2013;component/DarkTheme.xaml", UriKind.RelativeOrAbsolute)
                 ,new Uri("/Aehnlich;component/BindToMLib/AvalonDock_Dark_LightBrushs.xaml", UriKind.RelativeOrAbsolute)
                 ,new Uri("/AehnlichViewLib;component/Themes/LightBrushs.xaml", UriKind.RelativeOrAbsolute)
                 ,new Uri("/AehnlichViewLib;component/Themes/LightIcons.xaml", UriKind.RelativeOrAbsolute)

                }, settings.Themes);
            }
            catch
            {
                // Ignore issues in theme definition stage to ensure app starts up
            }

            try
            {
                // Add additional Light resources to those theme resources added above
                appearance.AddThemeResources("Light", new List<Uri>
                {
                  new Uri("/MWindowLib;component/Themes/LightTheme.xaml", UriKind.RelativeOrAbsolute)
                 ,new Uri("/Aehnlich;component/BindToMLib/MWindowLib/DarkLightBrushs.xaml", UriKind.RelativeOrAbsolute)
                 ,new Uri("/Xceed.Wpf.AvalonDock.Themes.VS2013;component/LightTheme.xaml", UriKind.RelativeOrAbsolute)
                 ,new Uri("/Aehnlich;component/BindToMLib/AvalonDock_Dark_LightBrushs.xaml", UriKind.RelativeOrAbsolute)
                 ,new Uri("/AehnlichViewLib;component/Themes/LightBrushs.xaml", UriKind.RelativeOrAbsolute)
                 ,new Uri("/AehnlichViewLib;component/Themes/LightIcons.xaml", UriKind.RelativeOrAbsolute)

                }, settings.Themes);
            }
            catch
            {
                // Ignore issues in theme definition stage to ensure app starts up
            }

            try
            {
                // Create a general settings model to make sure the app is at least governed by defaults
                // if there are no customized settings on first ever start-up of application
                var options = settings.Options;

                SettingDefaults.CreateGeneralSettings(options);
                SettingDefaults.CreateAppearanceSettings(options, settings);

                settings.Options.SetUndirty();
            }
            catch
            {
                // Ignore issues in theme definition stage to ensure app starts up
            }
        }

        #region Save Load Application configuration
        /// <summary>
        /// Save application settings when the application is being closed down
        /// </summary>
        public void SaveConfigOnAppClosed(IViewSize win)
        {
            /***
                        try
                        {
                            Models.AppCore.CreateAppDataFolder();

                            // Save App view model fields
                            var settings = base.GetService<ISettingsManager>();

                            //// settings.SessionData.LastActiveSourceFile = this.mStringDiff.SourceFilePath;
                            //// settings.SessionData.LastActiveTargetFile = this.mStringDiff.TargetFilePath;

                            // Save program options only if there are un-saved changes that need persistence
                            // This can be caused when WPF theme was changed or something else
                            // but should normally not occur as often as saving session data
                            if (settings.Options.IsDirty == true)
                            {
                                ////settings.SaveOptions(AppCore.DirFileAppSettingsData, settings.SettingData);
                                settings.Options.WriteXML(DirFileAppSettingsData);
                            }

                            settings.SaveSessionData(Models.AppCore.DirFileAppSessionData, settings.SessionData);
                        }
                        catch (Exception exp)
                        {
                            var msg = GetService<IMessageBoxService>();
                            msg.Show(exp, "Unexpected Error" // Local.Strings.STR_UnexpectedError_Caption
                                        , MsgBoxButtons.OK, MsgBoxImage.Error);
                        }
            ***/
        }

        /// <summary>
        /// Load configuration from persistence on startup of application
        /// </summary>
        public void LoadConfigOnAppStartup(ISettingsManager settings
                                          , IAppearanceManager appearance)
        {
            try
            {
                CreateDefaultsSettings(settings, appearance);

                /***
                    // Re/Load program options and user profile session data to control global behaviour of program
                    ////settings.LoadOptions(AppCore.DirFileAppSettingsData);
                    settings.Options.ReadXML(DirFileAppSettingsData);
                    settings.LoadSessionData(Models.AppCore.DirFileAppSessionData);

                    settings.CheckSettingsOnLoad(SystemParameters.VirtualScreenLeft,
                                                    SystemParameters.VirtualScreenTop);
                ***/
            }
            catch
            {
                // Ignore issues in load settings to ensure app starts up
            }
        }
        #endregion Save Load Application configuration

        #region StartUp/ShutDown
        private void AppExit_CommandExecuted()
        {
            try
            {
                if (Closing_CanExecute() == true)
                {
                    _ShutDownInProgress_Cancel = false;
                    OnRequestClose();
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);

                ////                var msg = GetService<IMessageBoxService>();
                ////                msg.Show(exp, "Unknown Error",
                ////                MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton);
            }
        }

        private bool Closing_CanExecute()
        {
            return true;
        }

        /// <summary>
        /// Check if pre-requisites for closing application are available.
        /// Save session data on closing and cancel closing process if necessary.
        /// </summary>
        /// <returns>true if application is OK to proceed closing with closed, otherwise false.</returns>
        public bool Exit_CheckConditions(object sender)
        {
            //// var msg = ServiceLocator.ServiceContainer.Instance.GetService<IMessageBoxService>();
            try
            {
                if (_ShutDownInProgress == true)
                    return true;

                // this return is normally computed if there are documents open with unsaved data
                return true;

                ////// Do layout serialization after saving/closing files
                ////// since changes implemented by shut-down process are otherwise lost
                ////try
                ////{
                ////    App.CreateAppDataFolder();
                ////    this.SerializeLayout(sender);            // Store the current layout for later retrieval
                ////}
                ////catch
                ////{
                ////}
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);

                ////                var msg = GetService<IMessageBoxService>();
                ////
                ////                msg.Show(exp, "Unexpected Error"//Local.Strings.STR_UnexpectedError_Caption,
                ////                            , MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton);
                ////                //App.IssueTrackerLink, App.IssueTrackerLink, Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }

            return true;
        }

        #region RequestClose [event]
        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        ////public event EventHandler ApplicationClosed;

        /// <summary>
        /// Method to be executed when user (or program) tries to close the application
        /// </summary>
        public void OnRequestClose()
        {
            OnRequestClose(true);
        }

        /// <summary>
        /// Method to be executed when user (or program) tries to close the application
        /// </summary>
        public void OnRequestClose(bool ShutDownAfterClosing)
        {
            try
            {
                if (ShutDownAfterClosing == true)
                {
                    if (_ShutDownInProgress == false)
                    {
                        if (DialogCloseResult == null)
                            DialogCloseResult = true;      // Execute Closing event via attached property

                        if (_ShutDownInProgress_Cancel == true)
                        {
                            _ShutDownInProgress = false;
                            _ShutDownInProgress_Cancel = false;
                            DialogCloseResult = null;
                        }
                    }
                }
                else
                    _ShutDownInProgress = true;

                CommandManager.InvalidateRequerySuggested();

                ////EventHandler handler = ApplicationClosed;
                ////if (handler != null)
                ////  handler(this, EventArgs.Empty);
            }
            catch (Exception exp)
            {
                _ShutDownInProgress = false;

                logger.Error(exp.Message, exp);

                ////                var msg = GetService<IMessageBoxService>();
                ////                msg.Show(exp, "Unexpected Error" //Local.Strings.STR_UnexpectedError_Caption
                ////                            , MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton);
            }
        }

        public void CancelShutDown()
        {
            DialogCloseResult = null;
        }
        #endregion // RequestClose [event]
        #endregion StartUp/ShutDown
        #endregion methods
    }
}
