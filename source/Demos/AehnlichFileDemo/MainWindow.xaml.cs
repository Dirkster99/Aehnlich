namespace AehnlichDemo
{
    using AehnlichLibViewModels.ViewModels;
    using System;
    using System.Reflection;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// class constructor
        /// </summary>
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

            //var appVM = new AppViewModel(fspath + @"\DemoTestFiles\ClassTemplate.txt",
            //                             fspath + @"\DemoTestFiles\Empty.txt");

            var appVM = new AppViewModel(fspath + @"\DemoTestFiles\MyersDiff.txt",
                                         fspath + @"\DemoTestFiles\MyersDiff_V1.txt");

            //var appVM = new AppViewModel(fspath + @"\DemoTestFiles\ClassTemplate.txt",
            //                             fspath + @"\DemoTestFiles\ClassTemplate1.txt");

            this.DataContext = appVM;
        }
    }
}
