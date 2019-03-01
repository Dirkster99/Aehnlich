namespace AehnlichViewLib.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// https://stackoverflow.com/questions/10793717/how-to-find-that-scrollviewer-is-scrolled-to-the-end-in-wpf
    /// </summary>
    public class ScrollHelper
    {
        public static readonly DependencyProperty ScrollChangedCommandProperty =
            DependencyProperty.RegisterAttached("ScrollChangedCommand",
                typeof(ICommand), typeof(ScrollHelper),
                new FrameworkPropertyMetadata(null, OnScrollToBottomPropertyChanged));

        public static ICommand GetScrollChangedCommand(DependencyObject ob)
        {
            return (ICommand)ob.GetValue(ScrollChangedCommandProperty);
        }

        public static void SetScrollChangedCommand(DependencyObject ob, ICommand value)
        {
            ob.SetValue(ScrollChangedCommandProperty, value);
        }

        private static void OnScrollToBottomPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = obj as ScrollViewer;

            if (scrollViewer != null)
                scrollViewer.Loaded += OnScrollViewerLoaded;
        }

        private static void OnScrollViewerLoaded(object sender, RoutedEventArgs e)
        {
            (sender as ScrollViewer).Loaded -= OnScrollViewerLoaded;

            (sender as ScrollViewer).Unloaded += OnScrollViewerUnloaded;
            (sender as ScrollViewer).ScrollChanged += OnScrollViewerScrollChanged;
        }

        private static void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;

            if (scrollViewer == null)
                return;

            if (scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight)
            {
                var command = GetScrollChangedCommand(sender as ScrollViewer);

                if (command == null || command.CanExecute(null) == false)
                    return;

                // Check whether this attached behaviour is bound to a RoutedCommand
                if (command is RoutedCommand)
                {
                    // Execute the routed command
                    (command as RoutedCommand).Execute(e, scrollViewer);
                }
                else
                {
                    // Execute the Command as bound delegate
                    command.Execute(e);
                }
            }
        }

        private static void OnScrollViewerUnloaded(object sender, RoutedEventArgs e)
        {
            (sender as ScrollViewer).Unloaded -= OnScrollViewerUnloaded;
            (sender as ScrollViewer).ScrollChanged -= OnScrollViewerScrollChanged;
        }
    }
}
