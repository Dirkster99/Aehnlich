namespace AehnlichDirDemo
{
    using AehnlichDirViewModelLib.Interfaces;
    using AehnlichDirViewModelLib.ViewModels;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Unloaded += MainWindow_Unloaded;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            var appVM = this.DataContext as System.IDisposable;
            if (appVM != null)
            {
                appVM.Dispose();
            }

            this.DataContext = null;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Reload settings about last left and right path being shown for merging
            Properties.Settings.Default.Reload();

            string leftDirPath = Properties.Settings.Default.LeftDirPath;
            string rightDirPath = Properties.Settings.Default.RightDirPath;

            // Construct AppViewModel and attach to datacontext
            var appVM = Factory.ConstructAppViewModel();

            this.DataContext = appVM;

            appVM.Initialize(leftDirPath, rightDirPath);
        }

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            try
            {
                var appVM = DataContext as IAppViewModel;
                if (appVM == null)
                    return;

                Properties.Settings.Default.LeftDirPath = appVM.LeftDirPath;
                Properties.Settings.Default.RightDirPath = appVM.RightDirPath;

                Properties.Settings.Default.Save();
            }
            catch
            {
                // Make sure saving values does not crash the app shut down process
            }

            try
            {
                var appVM = DataContext as System.IDisposable;
                if (appVM != null)
                    appVM.Dispose();

                DataContext = null;
            }
            catch
            {
                // Make sure disposing objects does not crash the app shut down process
            }
        }
    }
}
