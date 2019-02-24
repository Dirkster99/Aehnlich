namespace AehnlichDemo
{
    using AehnlichLibViewModels.ViewModels;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region fields
        private bool IgnoreNextSliderValueChange;
        private bool IgnoreNextTextSyncValueChange;
        private readonly object lockObject = new object();
        #endregion fields

        #region ctors
        /// <summary>
        /// class constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }
        #endregion ctors

        #region methods
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
            var sourceToSync = sender  as ICSharpCode.AvalonEdit.TextEditor;

            textToSync.ScrollToVerticalOffset(e.VerticalOffset);
            textToSync.ScrollToHorizontalOffset(e.HorizontalOffset);

            if (sourceToSync.TextArea.TextView.VisualLines.Any())
            {
                var firstline = sourceToSync.TextArea.TextView.VisualLines.First();
                var lastline = sourceToSync.TextArea.TextView.VisualLines.Last();

                int fline = firstline.FirstDocumentLine.LineNumber;
                int lline = lastline.LastDocumentLine.LineNumber;

                OverviewSlider.NumberOfTextLinesInViewPort = (lline - fline) - 1;

                lock (lockObject)
                {
                    if (IgnoreNextTextSyncValueChange == true)
                    {
                        IgnoreNextTextSyncValueChange = false;
                        return;
                    }

                    IgnoreNextSliderValueChange = true;

                    // Get value of first visible line and set it in Overview slider
                    OverviewSlider.Value = fline;
                }
            }
        }

        private void OverviewSlider_ValueChanged(object sender,
                                                 RoutedPropertyChangedEventArgs<double> e)
        {
            var ctrl = sender as AehnlichViewLib.Controls.Overview;
            if (ctrl == null)
                return;

            var line = (int)ctrl.Value;

            if (TextRight.TextArea.TextView.IsVisible)
            {
                if (TextRight.TextArea.TextView.VisualLines.Any())
                {
                    lock (lockObject)
                    {
                        if (IgnoreNextSliderValueChange == true)
                        {
                            IgnoreNextSliderValueChange = false;
                            return;
                        }

                        IgnoreNextTextSyncValueChange = true;

                        double vertOffset = (TextRight.TextArea.TextView.DefaultLineHeight) * line;
                        TextRight.ScrollToVerticalOffset(vertOffset);
                        TextLeft.ScrollToVerticalOffset(vertOffset);
                    }
                }
            }
        }
        #endregion methods
    }
}
