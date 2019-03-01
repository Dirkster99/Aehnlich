namespace AehnlichViewLib.Controls
{
    using AehnlichViewLib.Models;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    ///
    /// </summary>
    [TemplatePart(Name = PART_LeftDiffView, Type = typeof(DiffView))]
    [TemplatePart(Name = PART_RightDiffView, Type = typeof(DiffView))]
    [TemplatePart(Name = PART_RightFileNameTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_LeftFileNameTextBox, Type = typeof(TextBox))]
    public class DiffControl : Control
    {
        #region fields
        public const string PART_RightDiffView = "TextRight";
        public const string PART_RightFileNameTextBox = "PART_RightFileNameTextBox";

        public const string PART_LeftDiffView = "TextLeft";
        public const string PART_LeftFileNameTextBox = "PART_LeftFileNameTextBox";

        public static readonly DependencyProperty LeftFileNameProperty =
            DependencyProperty.Register("LeftFileName", typeof(string),
                typeof(DiffControl), new PropertyMetadata(null));


        public static readonly DependencyProperty RightFileNameProperty =
            DependencyProperty.Register("RightFileName", typeof(string),
                typeof(DiffControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ViewPortChangedCommandProperty =
            DependencyProperty.Register("ViewPortChangedCommand", typeof(ICommand),
                typeof(DiffControl), new PropertyMetadata(null));



        public ICommand ViewPortChangedCommand
        {
            get { return (ICommand)GetValue(ViewPortChangedCommandProperty); }
            set { SetValue(ViewPortChangedCommandProperty, value); }
        }

        public string RightFileName
        {
            get { return (string)GetValue(RightFileNameProperty); }
            set { SetValue(RightFileNameProperty, value); }
        }

        /// <summary>
        /// Implements the backing staore of the <see cref="LeftFileName"/> dependency property.
        /// </summary>
        public string LeftFileName
        {
            get { return (string)GetValue(LeftFileNameProperty); }
            set { SetValue(LeftFileNameProperty, value); }
        }

        private DiffView _PART_LeftDiffView;
        private DiffView _PART_RightDiffView;
        private ScrollViewer _leftScrollViewer;
        private ScrollViewer _rightScrollViewer;

        private TextBox _PART_LeftFileNameTextBox;
        private TextBox _PART_RightFileNameTextBox;
        #endregion fields

        #region ctors
        static DiffControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiffControl),
                new FrameworkPropertyMetadata(typeof(DiffControl)));
        }
        #endregion ctors

        #region properties
        #endregion properties

        #region methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Loaded += new RoutedEventHandler(this.OnLoaded);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _PART_LeftFileNameTextBox = GetTemplateChild(PART_LeftFileNameTextBox) as TextBox;
            _PART_RightFileNameTextBox = GetTemplateChild(PART_RightFileNameTextBox) as TextBox;

            _PART_LeftDiffView = GetTemplateChild(PART_LeftDiffView) as DiffView;
            _PART_RightDiffView = GetTemplateChild(PART_RightDiffView) as DiffView;

            _leftScrollViewer = GetScrollViewer(_PART_LeftDiffView);
            _rightScrollViewer = GetScrollViewer(_PART_RightDiffView);

            if (_leftScrollViewer != null)
                _leftScrollViewer.ScrollChanged += Scrollviewer_ScrollChanged;

            if (_rightScrollViewer != null)
                _rightScrollViewer.ScrollChanged += Scrollviewer_ScrollChanged;
        }

        private void Scrollviewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollToSync = null;
            DiffView sourceToSync = null;

            if (sender == _rightScrollViewer)
            {
                scrollToSync = _leftScrollViewer;
                sourceToSync = _PART_LeftDiffView;
            }
            else
            {
                scrollToSync = _rightScrollViewer;
                sourceToSync = _PART_RightDiffView;
            }

            // Sync scrollviewers on both side
            var src_scrollToSync = sender as ScrollViewer;

            scrollToSync.ScrollToVerticalOffset(e.VerticalOffset);
            scrollToSync.ScrollToHorizontalOffset(e.HorizontalOffset);

            // Get currently view lines viewport in source of event
            if (sourceToSync.TextArea.TextView.VisualLines.Any())
            {
                var firstline = sourceToSync.TextArea.TextView.VisualLines.First();
                var lastline = sourceToSync.TextArea.TextView.VisualLines.Last();

                int fline = firstline.FirstDocumentLine.LineNumber;
                int lline = lastline.LastDocumentLine.LineNumber;

                if (ViewPortChangedCommand != null)
                {
                    var currentViewPort = new DiffViewPort(fline, lline);

                    if (ViewPortChangedCommand.CanExecute(currentViewPort))
                    {
                        // Check whether this attached behaviour is bound to a RoutedCommand
                        if (ViewPortChangedCommand is RoutedCommand)
                        {
                            // Execute the routed command
                            (ViewPortChangedCommand as RoutedCommand).Execute(currentViewPort, this);
                        }
                        else
                        {
                            // Execute the Command as bound delegate
                            ViewPortChangedCommand.Execute(currentViewPort);
                        }
                    }
                }
            }

            e.Handled = true;
        }

        public static ScrollViewer GetScrollViewer(UIElement element)
        {
            if (element == null) return null;

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
        #endregion methods
    }
}
