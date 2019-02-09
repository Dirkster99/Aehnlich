namespace GenericDemo
{
    using DiffLibViewModels.ViewModels;
    using GenericDemo.Models;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
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

            TextLeft.Text =
                "Line 1\n" +
                "Line 2\n" +
                "Line 3\n" +
                "Line 4\n" +
                "Line 5\n" +
                "Line 6\n";

            Dictionary<int, DiffContext> lines = new Dictionary<int, DiffContext>();

            lines.Add(1, DiffContext.Added);
            lines.Add(2, DiffContext.Deleted);
            lines.Add(3, DiffContext.Added);
            lines.Add(5, DiffContext.Added);

            var backgroundRenderer = new DiffLineBackgroundRenderer { Lines = lines };

            TextLeft.TextArea.TextView.BackgroundRenderers.Add(backgroundRenderer);
        }
    }
}
