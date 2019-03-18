namespace AehnlichDirDemo
{
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
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Reload settings about last left and right path being shown for merging
            Properties.Settings.Default.Reload();

            string leftDirPath = Properties.Settings.Default.LeftDirPath;
            string rightDirPath = Properties.Settings.Default.RightDirPath;

            // Construct AppViewModel and attach to datacontext
            var appVM = new AppViewModel();

            this.DataContext = appVM;

            appVM.Initialize(leftDirPath, rightDirPath);
        }

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            var appVM = DataContext as AppViewModel;
            if (appVM == null)
                return;

            Properties.Settings.Default.LeftDirPath = appVM.LeftDirPath;
            Properties.Settings.Default.RightDirPath = appVM.RightDirPath;

            Properties.Settings.Default.Save();
        }

        #region Column Width A B Synchronization
        private void MainSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            DiffDir.ColumnWidthA = this.TopColumnA.Width;
            DiffDir.ColumnWidthB = this.TopColumnB.Width;
        }

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            DiffDir.ColumnWidthA = this.TopColumnA.Width;
            DiffDir.ColumnWidthB = this.TopColumnB.Width;
        }

        private void DiffDir_ColumnWidthChanged(object sender, AehnlichViewLib.Events.ColumnWidthChangedEvent e)
        {
            this.TopColumnA.Width = e.ColumnWidthA;
            this.TopColumnB.Width = e.ColumnWidthB;
        }
        #endregion Column Width A B Synchronization
    }
}
