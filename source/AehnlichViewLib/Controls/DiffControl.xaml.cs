namespace AehnlichViewLib.Controls
{
    using AehnlichViewLib.Enums;
    using AehnlichViewLib.Models;
    using ICSharpCode.AvalonEdit;
    using System;
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
    public class DiffControl : Control
    {
        #region fields
        public const string PART_RightDiffView = "PART_TextRight";
        public const string PART_LeftDiffView = "PART_TextLeft";

        /// <summary>
        /// Implements the backing store of the <see cref="SetFocus"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SetFocusProperty =
            DependencyProperty.Register("SetFocus", typeof(Focus),
                typeof(DiffControl), new PropertyMetadata(Enums.Focus.LeftFilePath, OnSetFocusChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="LeftFileName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftFileNameProperty =
            DependencyProperty.Register("LeftFileName", typeof(string),
                typeof(DiffControl), new PropertyMetadata(null));

        /// <summary>
        /// Implements the backing store of the <see cref="RightFileName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RightFileNameProperty =
            DependencyProperty.Register("RightFileName", typeof(string),
                typeof(DiffControl), new PropertyMetadata(null));

        /// <summary>
        /// Implements the backing store of the <see cref="ViewPortChangedCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewPortChangedCommandProperty =
            DependencyProperty.Register("ViewPortChangedCommand", typeof(ICommand),
                typeof(DiffControl), new PropertyMetadata(null));

        public static readonly DependencyProperty DiffViewOptionsProperty =
            DependencyProperty.Register("DiffViewOptions", typeof(TextEditorOptions),
                typeof(DiffControl), new PropertyMetadata(new TextEditorOptions { IndentationSize = 4, ShowTabs = false, ConvertTabsToSpaces = true }));

        #region GridColumn Sync
        /// <summary>
        /// Implements the backing store of the <see cref="WidthColumnA"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthColumnAProperty =
            DependencyProperty.Register("WidthColumnA", typeof(GridLength),
                typeof(DiffControl), new PropertyMetadata(new GridLength(1, GridUnitType.Star)));

        /// <summary>
        /// Implements the backing store of the <see cref="WidthColumnB"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthColumnBProperty =
            DependencyProperty.Register("WidthColumnB", typeof(GridLength),
                typeof(DiffControl), new PropertyMetadata(new GridLength(1, GridUnitType.Star)));
        #endregion GridColumn Sync

        private DiffView _PART_LeftDiffView, _PART_RightDiffView;
        private ScrollViewer _leftScrollViewer, _rightScrollViewer;
        #endregion fields

        #region ctors
        static DiffControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiffControl),
                new FrameworkPropertyMetadata(typeof(DiffControl)));
        }
        #endregion ctors

        #region properties
        public TextEditorOptions DiffViewOptions
        {
            get { return (TextEditorOptions)GetValue(DiffViewOptionsProperty); }
            set { SetValue(DiffViewOptionsProperty, value); }
        }

        public string LeftFileName
        {
            get { return (string)GetValue(LeftFileNameProperty); }
            set { SetValue(LeftFileNameProperty, value); }
        }

        public string RightFileName
        {
            get { return (string)GetValue(RightFileNameProperty); }
            set { SetValue(RightFileNameProperty, value); }
        }

        /// <summary>
        /// Implements a bindable command that can be invoked when the current view
        /// in the left or right view changes (window changes size resulting in less
        /// or more text lines being available in actual displayed view).
        /// </summary>
        public ICommand ViewPortChangedCommand
        {
            get { return (ICommand)GetValue(ViewPortChangedCommandProperty); }
            set { SetValue(ViewPortChangedCommandProperty, value); }
        }

        public Focus SetFocus
        {
            get { return (Focus)GetValue(SetFocusProperty); }
            set { SetValue(SetFocusProperty, value); }
        }

        #region GridColumn Sync
        public GridLength WidthColumnA
        {
            get { return (GridLength)GetValue(WidthColumnAProperty); }
            set { SetValue(WidthColumnAProperty, value); }
        }

        public GridLength WidthColumnB
        {
            get { return (GridLength)GetValue(WidthColumnBProperty); }
            set { SetValue(WidthColumnBProperty, value); }
        }
        #endregion
        #endregion GridColumn Sync

        #region methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Loaded += new RoutedEventHandler(this.OnLoaded);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _PART_LeftDiffView = GetTemplateChild(PART_LeftDiffView) as DiffView;
            _PART_RightDiffView = GetTemplateChild(PART_RightDiffView) as DiffView;

            _leftScrollViewer = GetScrollViewer(_PART_LeftDiffView);
            _rightScrollViewer = GetScrollViewer(_PART_RightDiffView);

            if (_leftScrollViewer != null)
                _leftScrollViewer.ScrollChanged += Scrollviewer_ScrollChanged;

            if (_rightScrollViewer != null)
                _rightScrollViewer.ScrollChanged += Scrollviewer_ScrollChanged;
        }

        /// <summary>
        /// Is invoked when 1 of the scroll viewer view is changing to tell the
        /// viemlodel about it and give it a chance to update its background highlighting
        /// definition items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scrollviewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollToSync = null;
            DiffView sourceToSync = null;

            // Determine the scroller that has send the event so we know the scroller to sync
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

            var src_scrollToSync = sender as ScrollViewer;  // Sync scrollviewers on both side of DiffControl

            scrollToSync.ScrollToVerticalOffset(e.VerticalOffset);
            scrollToSync.ScrollToHorizontalOffset(e.HorizontalOffset);

            // Get current view lines viewport in source of event
            if (sourceToSync.TextArea.TextView.VisualLines.Any())
            {
                var firstline = sourceToSync.TextArea.TextView.VisualLines.First();
                var lastline = sourceToSync.TextArea.TextView.VisualLines.Last();

                int fline = firstline.FirstDocumentLine.LineNumber;
                int lline = lastline.LastDocumentLine.LineNumber;

                int caretLine = sourceToSync.TextArea.Caret.Line;
                int caretCol = sourceToSync.TextArea.Caret.Column;

                if (ViewPortChangedCommand != null)
                {
                    var currentViewPort = new DiffViewPort(fline, lline, sourceToSync.LineCount, caretLine, caretCol);

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

        #region SetFocus Dependency Property
        private static void OnSetFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DiffControl).OnSetFocusChanged(e);
        }

        private void OnSetFocusChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((e.NewValue is Enums.Focus) == false)
                return;

            UIElement elementToFocus = null;

            switch ((Enums.Focus)e.NewValue)
            {
                case Enums.Focus.LeftView:
                    elementToFocus = _PART_LeftDiffView;
                    break;

                case Enums.Focus.RightView:
                    elementToFocus = _PART_RightDiffView;
                    break;

                case Enums.Focus.None:
                    return;

                default:
                    throw new ArgumentException(((Enums.Focus)e.NewValue).ToString());
            }

            if (elementToFocus != null)
            {
                elementToFocus.Focus();
                Keyboard.Focus(elementToFocus);
            }
        }
        #endregion SetFocus Dependency Property
        #endregion methods
    }
}
