namespace AehnlichDirDemo
{
    using AehnlichDirViewModelLib.ViewModels;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
////        private ScrollViewer _leftScrollViewer;
////        private ScrollViewer _rightScrollViewer;
        private ScrollViewer _leftGridScrollViewer;
        private ScrollViewer _rightGridScrollViewer;

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

////            _leftScrollViewer = GetScrollViewer(ListA);
////            _rightScrollViewer = GetScrollViewer(ListB);

            _leftGridScrollViewer = GetScrollViewer(GridA);
            _rightGridScrollViewer = GetScrollViewer(GridB);

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

////        /// <summary>
////        /// Is invoked by the ListBox selection changed event handler
////        /// of the right or left ListBox A or B.
////        /// </summary>
////        /// <param name="sender"></param>
////        /// <param name="e"></param>
////        private void List_SelectionChanged(object sender,
////                                            System.Windows.Controls.SelectionChangedEventArgs e)
////        {
////            ListBox listboxDestinationToSync = null;
////            ListBox listboxSourceToSync = null;
////
////            if (sender == ListA)
////            {
////                listboxDestinationToSync = ListB;
////                listboxSourceToSync = ListA;
////            }
////            else
////            {
////                if (sender == ListB)
////                {
////                    listboxDestinationToSync = ListA;
////                    listboxSourceToSync = ListB;
////                }
////            }
////
////            // Not sure who's been sending this so we can't process it
////            if (listboxDestinationToSync == null || listboxSourceToSync == null)
////                return;
////
////            if (listboxDestinationToSync.SelectedIndex != listboxSourceToSync.SelectedIndex)
////            {
////                listboxDestinationToSync.SelectedIndex = listboxSourceToSync.SelectedIndex;
////            }
////        }
////
////        private void List_ScrollChanged(object sender, ScrollChangedEventArgs e)
////        {
////            if (_leftScrollViewer == null && _rightScrollViewer == null)
////                return;
////
////            ScrollViewer scrollToSync = null;
////
////            // Determine the scroller that has send the event so we know the scroller to sync
////            if (sender == ListB)
////                scrollToSync = _leftScrollViewer;
////            else
////                scrollToSync = _rightScrollViewer;
////
////            var src_scrollToSync = sender as ScrollViewer;  // Sync scrollviewers on both side of DiffControl
////
////            scrollToSync.ScrollToVerticalOffset(e.VerticalOffset);
////            scrollToSync.ScrollToHorizontalOffset(e.HorizontalOffset);
////        }

        private static ScrollViewer GetScrollViewer(UIElement element)
        {
            if (element == null)
                return null;

            ScrollViewer retour = null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element) && retour == null; i++)
            {
                if (VisualTreeHelper.GetChild(element, i) is ScrollViewer)
                {
                    retour = (ScrollViewer)(VisualTreeHelper.GetChild(element, i));
                }
                else
                {
                    retour = GetScrollViewer(VisualTreeHelper.GetChild(element, i) as UIElement);
                }
            }

            return retour;
        }

        private void Grid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_leftGridScrollViewer == null && _rightGridScrollViewer == null)
                return;

            ScrollViewer scrollToSync = null;

            // Determine the scroller that has send the event so we know the scroller to sync
            if (sender == GridB)
                scrollToSync = _leftGridScrollViewer;
            else
                scrollToSync = _rightGridScrollViewer;

            var src_scrollToSync = sender as ScrollViewer;  // Sync scrollviewers on both side of DiffControl

            scrollToSync.ScrollToVerticalOffset(e.VerticalOffset);
            scrollToSync.ScrollToHorizontalOffset(e.HorizontalOffset);
        }

        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gridDestinationToSync = null;
            DataGrid gridSourceToSync = null;

            if (sender == GridA)
            {
                gridDestinationToSync = GridB;
                gridSourceToSync = GridA;
            }
            else
            {
                if (sender == GridB)
                {
                    gridDestinationToSync = GridA;
                    gridSourceToSync = GridB;
                }
            }

            // Not sure who's been sending this so we can't process it
            if (gridDestinationToSync == null || gridSourceToSync == null)
                return;

            if (gridDestinationToSync.SelectedIndex != gridSourceToSync.SelectedIndex)
            {
                gridDestinationToSync.SelectedIndex = gridSourceToSync.SelectedIndex;
            }
        }
    }
}
