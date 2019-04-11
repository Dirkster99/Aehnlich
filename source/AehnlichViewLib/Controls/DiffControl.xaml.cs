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
    [TemplatePart(Name = PART_RightFileNameTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_LeftFileNameTextBox, Type = typeof(TextBox))]
    public class DiffControl : Control
    {
        #region fields
        public const string PART_RightDiffView = "TextRight";
        public const string PART_RightFileNameTextBox = "PART_RightFileNameTextBox";

        public const string PART_LeftDiffView = "TextLeft";
        public const string PART_LeftFileNameTextBox = "PART_LeftFileNameTextBox";

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

        // Using a DependencyProperty as the backing store for NextLeftTargetLocation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextLeftTargetLocationProperty =
            DependencyProperty.Register("NextLeftTargetLocation", typeof(ICommand),
                typeof(DiffControl), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for NextRightTargetLocation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextRightTargetLocationProperty =
            DependencyProperty.Register("NextRightTargetLocation", typeof(ICommand),
                typeof(DiffControl), new PropertyMetadata(null));

        private DiffView _PART_LeftDiffView, _PART_RightDiffView;
        private SuggestBoxLib.SuggestBox _PART_LeftFileNameTextBox, _PART_RightFileNameTextBox;
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

        public ICommand NextLeftTargetLocation
        {
            get { return (ICommand)GetValue(NextLeftTargetLocationProperty); }
            set { SetValue(NextLeftTargetLocationProperty, value); }
        }

        public ICommand NextRightTargetLocation
        {
            get { return (ICommand)GetValue(NextRightTargetLocationProperty); }
            set { SetValue(NextRightTargetLocationProperty, value); }
        }
        #endregion properties

        #region methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Loaded += new RoutedEventHandler(this.OnLoaded);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _PART_LeftFileNameTextBox = GetTemplateChild(PART_LeftFileNameTextBox) as SuggestBoxLib.SuggestBox;
            _PART_RightFileNameTextBox = GetTemplateChild(PART_RightFileNameTextBox) as SuggestBoxLib.SuggestBox;

            _PART_LeftDiffView = GetTemplateChild(PART_LeftDiffView) as DiffView;
            _PART_RightDiffView = GetTemplateChild(PART_RightDiffView) as DiffView;

            _leftScrollViewer = GetScrollViewer(_PART_LeftDiffView);
            _rightScrollViewer = GetScrollViewer(_PART_RightDiffView);

            if (_leftScrollViewer != null)
                _leftScrollViewer.ScrollChanged += Scrollviewer_ScrollChanged;

            if (_rightScrollViewer != null)
                _rightScrollViewer.ScrollChanged += Scrollviewer_ScrollChanged;

            if (_PART_LeftFileNameTextBox != null)
                _PART_LeftFileNameTextBox.NewLocationRequestEvent += _PART_FileNameTextBox_NewLocationRequestEvent;

            if (_PART_RightFileNameTextBox != null)
                _PART_RightFileNameTextBox.NewLocationRequestEvent += _PART_FileNameTextBox_NewLocationRequestEvent;

        }

        private void _PART_FileNameTextBox_NewLocationRequestEvent(object sender,
                                                                   SuggestBoxLib.Events.NextTargetLocationArgs e)
        {
            ICommand commandToExec = null;

            if (sender == _PART_LeftFileNameTextBox)
                commandToExec = NextLeftTargetLocation;

            if (sender == _PART_RightFileNameTextBox)
                commandToExec = NextRightTargetLocation;

            if (commandToExec == null)
                return;

            if (commandToExec is RoutedCommand)
            {
                if (((RoutedCommand)commandToExec).CanExecute(e, this))
                    ((RoutedCommand)commandToExec).Execute(e, this);
            }
            else
            {
                if (commandToExec.CanExecute(e))
                    commandToExec.Execute(e);
            }
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
                case Enums.Focus.LeftFilePath:
                    elementToFocus = _PART_LeftFileNameTextBox;
                    break;

                case Enums.Focus.RightFilePath:
                    elementToFocus = _PART_RightFileNameTextBox;
                    break;

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
