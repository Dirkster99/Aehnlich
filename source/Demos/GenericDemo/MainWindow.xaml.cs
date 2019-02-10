namespace GenericDemo
{
    using DiffLibViewModels.ViewModels;
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;

            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            var fspath = System.IO.Path.GetDirectoryName(path);

            var appVM = new AppViewModel(fspath + @"\DemoTestFiles\MyersDiff.txt",
                                         fspath + @"\DemoTestFiles\MyersDiff_V1.txt");

            this.DataContext = appVM;
        }

        /// <summary>
        /// Implements scrollviewer synchronization
        /// https://stackoverflow.com/questions/20864503/synchronizing-two-rich-text-box-scroll-bars-in-wpf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiffView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var textToSync = (sender == TextRight) ? TextLeft : TextRight;

            textToSync.ScrollToVerticalOffset(e.VerticalOffset);
            textToSync.ScrollToHorizontalOffset(e.HorizontalOffset);
        }
    }
}
