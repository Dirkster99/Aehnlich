namespace AehnlichViewLib.Controls
{
    using ICSharpCode.AvalonEdit.Editing;
    using AehnlichViewLib.Enums;
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.Rendering;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Data;
    using System.Windows.Controls;
    using AehnlichViewLib.Controls.AvalonEditEx;
    using ICSharpCode.AvalonEdit.Search;
    using AehnlichViewLib.Interfaces;
    using System.Windows.Threading;
    using System.Linq;

    /// <summary>
    /// Implements a <see cref="TextEditor"/> based view that can be used to highlight
    /// text difference through line background coloring.
    /// </summary>
    public partial class DiffView : TextEditor
    {
        #region fields
        /// <summary>
        /// Implements the backing store of the <see cref="ItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable),
                typeof(DiffView), new PropertyMetadata(new PropertyChangedCallback(ItemsSourceChanged)));

        public static readonly DependencyProperty LineDiffDataProviderProperty =
            DependencyProperty.Register("LineDiffDataProvider", typeof(ILineDiffProvider),
                typeof(DiffView), new PropertyMetadata(null, OnLineDiffDataProviderChanged));

        private static void OnLineDiffDataProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DiffView).OnLineDiffDataProviderChanged(e);
        }

        private void OnLineDiffDataProviderChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((e.OldValue as ILineDiffProvider) != null)
                (e.NewValue as ILineDiffProvider).DiffLineInfoChanged -= DiffView_DiffLineInfoChanged;

            if ((e.NewValue as ILineDiffProvider) != null)
                (e.NewValue as ILineDiffProvider).DiffLineInfoChanged += DiffView_DiffLineInfoChanged;
        }

        /// <summary>
        /// Redraw additional line changed background highlighting segments since viewmodel
        /// just computed these (they cannot have been drawn anytime before but user is looking at it).
        /// 
        /// Method execute when bound viewmodel sends the <see cref="Events.DiffLineInfoChangedEvent"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiffView_DiffLineInfoChanged(object sender, Events.DiffLineInfoChangedEvent e)
        {
            if (e.TypeOfInfoChange == Events.DiffLineInfoChange.LineEditScriptSegments)
            {
                var srcLineDiffs = this.ItemsSource as IReadOnlyList<IDiffLineInfo>;

                if (srcLineDiffs == null)
                    return;

                int fline = -1;  // are newly computed lines within current view?
                int lline = -1;
                if (TextArea.TextView.VisualLines.Any())
                {
                    var firstline = this.TextArea.TextView.VisualLines.First();
                    var lastline = this.TextArea.TextView.VisualLines.Last();

                    fline = firstline.FirstDocumentLine.LineNumber;
                    lline = lastline.LastDocumentLine.LineNumber;
                }

                if (fline == -1 && lline == -1)
                    return;

                for (int i = 0; i < e.LinesChanged.Count; i++)
                {
                    int linei = e.LinesChanged[i];          // Is linei within current view?
                    if (fline > linei || lline < linei)
                        continue;                         // No, look at next line then...

                    if (srcLineDiffs[linei].LineEditScriptSegments != null)
                    {
                        foreach (var segment in srcLineDiffs[linei].LineEditScriptSegments)
                            TextArea.TextView.Redraw(segment);    // invalidate this portion of document
                    }
                }
            }
        }

        public ILineDiffProvider LineDiffDataProvider
        {
            get { return (ILineDiffProvider)GetValue(LineDiffDataProviderProperty); }
            set { SetValue(LineDiffDataProviderProperty, value); }
        }

        #region Diff Color Definitions
        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundBlank"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundBlankProperty =
            DependencyProperty.Register("ColorBackgroundBlank", typeof(SolidColorBrush),
                typeof(DiffView), new PropertyMetadata(default(SolidColorBrush), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundAdded"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundAddedProperty =
            DependencyProperty.Register("ColorBackgroundAdded", typeof(SolidColorBrush),
                typeof(DiffView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xba, 0xff)), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundDeleted"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundDeletedProperty =
            DependencyProperty.Register("ColorBackgroundDeleted", typeof(SolidColorBrush),
                typeof(DiffView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x80, 0x80)), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundContext"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundContextProperty =
            DependencyProperty.Register("ColorBackgroundContext", typeof(SolidColorBrush),
                typeof(DiffView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x80, 0xFF, 0x80)), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundImaginaryLineAdded"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundImaginaryLineAddedProperty =
            DependencyProperty.Register("ColorBackgroundImaginaryLineAdded", typeof(SolidColorBrush),
                typeof(DiffView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x60, 0x00, 0xba, 0xff)), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundImaginaryLineDeleted"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundImaginaryLineDeletedProperty =
            DependencyProperty.Register("ColorBackgroundImaginaryLineDeleted", typeof(SolidColorBrush),
                typeof(DiffView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x60, 0x80, 0xFF, 0x80)), OnColorChanged));
        #endregion Diff Color Definitions

        #region EditorScrollOffsetXY
        /// <summary>
        /// Current editor view scroll X position
        /// </summary>
        public static readonly DependencyProperty EditorScrollOffsetXProperty =
            DependencyProperty.Register("EditorScrollOffsetX", typeof(double),
                typeof(DiffView), new UIPropertyMetadata(0.0d));

        /// <summary>
        /// Current editor view scroll Y position
        /// </summary>
        public static readonly DependencyProperty EditorScrollOffsetYProperty =
            DependencyProperty.Register("EditorScrollOffsetY", typeof(double),
                typeof(DiffView), new UIPropertyMetadata(0.0d));
        #endregion EditorScrollOffsetXY

        #region CaretPosition
        private static readonly DependencyProperty ColumnProperty =
            DependencyProperty.Register("Column", typeof(int),
                typeof(DiffView), new UIPropertyMetadata(1));

        private static readonly DependencyProperty LineProperty =
            DependencyProperty.Register("Line", typeof(int),
                typeof(DiffView), new UIPropertyMetadata(1));
        #endregion CaretPosition

        /// <summary>
        /// Implements the backing store of the <see cref="ActivationTimeStamp"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActivationTimeStampProperty =
            DependencyProperty.Register("ActivationTimeStamp", typeof(DateTime),
                typeof(DiffView), new PropertyMetadata(default(DateTime)));

        #region EditorCurrentLine Highlighting Colors
        private static readonly DependencyProperty EditorCurrentLineBackgroundProperty =
            DependencyProperty.Register("EditorCurrentLineBackground",
                                         typeof(Brush),
                                         typeof(DiffView),
                                         new UIPropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty EditorCurrentLineBorderProperty =
            DependencyProperty.Register("EditorCurrentLineBorder", typeof(Brush),
                typeof(DiffView), new PropertyMetadata(new SolidColorBrush(
                    Color.FromArgb(0x60, SystemColors.HighlightBrush.Color.R,
                                         SystemColors.HighlightBrush.Color.G,
                                         SystemColors.HighlightBrush.Color.B))));

        public static readonly DependencyProperty EditorCurrentLineBorderThicknessProperty =
            DependencyProperty.Register("EditorCurrentLineBorderThickness", typeof(double),
                typeof(DiffView), new PropertyMetadata((double)2.0d));
        #endregion EditorCurrentLine Highlighting Colors

        private INotifyCollectionChanged _observeableDiffContext;

        private DiffLineBackgroundRenderer _DiffBackgroundRenderer;
        #endregion fields

        #region ctors
        /// <summary>
        /// Static class constructor
        /// </summary>
        static DiffView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DiffView),
                new FrameworkPropertyMetadata(typeof(DiffView)));
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public DiffView()
            : base()
        {
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets/sets a source of items that can be used to populate marker elements
        /// on the overview bar. This should ideally be an ObservableCollection{T} or
        /// at least an <see cref="IList{T}"/> where T is a <see cref="DiffContext"/>
        /// for a particular line in the merged text views.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        #region Diff Color definitions
        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies changed context (2 lines appear to be similar enough to align them
        /// but still mark them as different).
        /// </summary>
        public SolidColorBrush ColorBackgroundContext
        {
            get { return (SolidColorBrush)GetValue(ColorBackgroundContextProperty); }
            set { SetValue(ColorBackgroundContextProperty, value); }
        }

        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies an element that is missing in one of the two (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorBackgroundDeleted
        {
            get { return (SolidColorBrush)GetValue(ColorBackgroundDeletedProperty); }
            set { SetValue(ColorBackgroundDeletedProperty, value); }
        }

        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies an element that is added in one of the two (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorBackgroundAdded
        {
            get { return (SolidColorBrush)GetValue(ColorBackgroundAddedProperty); }
            set { SetValue(ColorBackgroundAddedProperty, value); }
        }

        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies 2 blank lines in both of the two (text) lines being compared.
        /// 
        /// Normally, there should be no drawing required for this which is why the
        /// default is default(<see cref="SolidColorBrush"/>) - but sometimes it may be useful
        /// to color these lines which is why we have this property here.
        /// </summary>
        public SolidColorBrush ColorBackgroundBlank
        {
            get { return (SolidColorBrush)GetValue(ColorBackgroundBlankProperty); }
            set { SetValue(ColorBackgroundBlankProperty, value); }
        }

        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies an element that is missing in one of the two (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorBackgroundImaginaryLineDeleted
        {
            get { return (SolidColorBrush)GetValue(ColorBackgroundImaginaryLineDeletedProperty); }
            set { SetValue(ColorBackgroundImaginaryLineDeletedProperty, value); }
        }

        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies an element that is added as an imaginary line in one of the two
		/// (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorBackgroundImaginaryLineAdded
        {
            get { return (SolidColorBrush)GetValue(ColorBackgroundImaginaryLineAddedProperty); }
            set { SetValue(ColorBackgroundImaginaryLineAddedProperty, value); }
        }
       
        #endregion Diff Color Definitions

        #region EditorScrollOffsetXY
        /// <summary>
        /// Get/set dependency property to scroll editor by an offset in X direction.
        /// </summary>
        public double EditorScrollOffsetX
        {
            get
            {
                return (double)GetValue(EditorScrollOffsetXProperty);
            }

            set
            {
                SetValue(EditorScrollOffsetXProperty, value);
            }
        }

        /// <summary>
        /// Get/set dependency property to scroll editor by an offset in Y direction.
        /// </summary>
        public double EditorScrollOffsetY
        {
            get
            {
                return (double)GetValue(EditorScrollOffsetYProperty);
            }

            set
            {
                SetValue(EditorScrollOffsetYProperty, value);
            }
        }
        #endregion EditorScrollOffsetXY

        #region CaretPosition
        /// <summary>
        /// Get/set the current column of the editor caret.
        /// </summary>
        public int Column
        {
            get
            {
                return (int)GetValue(ColumnProperty);
            }

            set
            {
                SetValue(ColumnProperty, value);
            }
        }

        /// <summary>
        /// Get/set the current line of the editor caret.
        /// </summary>
        public int Line
        {
            get
            {
                return (int)GetValue(LineProperty);
            }

            set
            {
                SetValue(LineProperty, value);
            }
        }
        #endregion CaretPosition

        /// <summary>
        /// Gets the timestamp for the last time when this view was active (had received focus).
        /// </summary>
        public DateTime ActivationTimeStamp
        {
            get { return (DateTime)GetValue(ActivationTimeStampProperty); }
            set { SetValue(ActivationTimeStampProperty, value); }
        }

        #region EditorCurrentLine Highlighting Colors
        /// <summary>
        /// Style the background color of the current editor line
        /// </summary>
        public Brush EditorCurrentLineBackground
        {
            get { return (Brush)GetValue(EditorCurrentLineBackgroundProperty); }
            set { SetValue(EditorCurrentLineBackgroundProperty, value); }
        }

        public Brush EditorCurrentLineBorder
        {
            get { return (Brush)GetValue(EditorCurrentLineBorderProperty); }
            set { SetValue(EditorCurrentLineBorderProperty, value); }
        }

        public double EditorCurrentLineBorderThickness
        {
            get { return (double)GetValue(EditorCurrentLineBorderThicknessProperty); }
            set { SetValue(EditorCurrentLineBorderThicknessProperty, value); }
        }
        #endregion EditorCurrentLine Highlighting Colors
        #endregion properties

        #region methods
        /// <summary>
        /// Is called after the template was applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.Loaded += new RoutedEventHandler(this.OnLoaded);
            this.Unloaded += new RoutedEventHandler(this.OnUnloaded);
        }

        #region static handlers
        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DiffView)d).OnColorChanged(e.NewValue);
        }

        /// <summary>
        /// Is invoked when the <see cref="ItemsSource"/> dependency property has been changed.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DiffView)d).ItemsSourceChanged(e.NewValue);
        }
        #endregion static handlers

        /// <summary>
        /// Reset the <seealso cref="SolidColorBrush"/> to be used for highlighting the current editor line.
        /// </summary>
        private void AdjustCurrentLineBackground()
        {
            HighlightCurrentLineBackgroundRenderer oldRenderer = null;

            // Make sure there is only one of this type of background renderer
            // Otherwise, we might keep adding and WPF keeps drawing them on top of each other
            foreach (var item in this.TextArea.TextView.BackgroundRenderers)
            {
                if (item != null)
                {
                    if (item is HighlightCurrentLineBackgroundRenderer)
                    {
                        oldRenderer = item as HighlightCurrentLineBackgroundRenderer;
                    }
                }
            }

            if (oldRenderer != null)
                this.TextArea.TextView.BackgroundRenderers.Remove(oldRenderer);

            this.TextArea.TextView.BackgroundRenderers.Add(new HighlightCurrentLineBackgroundRenderer(this));
        }

        /// <summary>
        /// Hock event handlers and restore editor states (if any) or defaults
        /// when the control is fully loaded.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private void OnLoaded(object obj, RoutedEventArgs args)
        {
            try
            {
                // This adds a search panel into the edit
                // It is available via Ctrl-F
                SearchPanel.Install(this);

                // Remove rounded corners from text selection
                this.TextArea.SelectionCornerRadius = 0;
                this.TextArea.SelectionBorder = null;

                // Initialize Diff line background color rendering
                _DiffBackgroundRenderer = new DiffLineBackgroundRenderer(this);
                this.TextArea.TextView.BackgroundRenderers.Add(_DiffBackgroundRenderer);

                // Customize display of line numbers using real lines (shown with a number)
                // and imaginary lines (shown without a number)
                // Switch off default line handler in AvalonEdit
                ShowLineNumbers = false;
                this.TextArea.LeftMargins.Clear();
                var leftMargins = this.TextArea.LeftMargins;

                // Configure and insert custom line number margin indicator
                LineNumberMargin lineNumbers = new CustomLineNumberMargin(this);
                Line line = (Line)DottedLineMargin.Create();
                leftMargins.Insert(0, lineNumbers);
                leftMargins.Insert(1, line);
                var lineNumbersForeground = new Binding("LineNumbersForeground") { Source = this };
                line.SetBinding(Shape.StrokeProperty, lineNumbersForeground);
                lineNumbers.SetBinding(Control.ForegroundProperty, lineNumbersForeground);

                // Attach more event handlers
                this.GotFocus += DiffView_GotFocus;
                ////this.Focus();
                ////this.ForceCursor = true;

                // Restore cursor position for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
                this.ScrollToHorizontalOffset(this.EditorScrollOffsetX);
                this.ScrollToVerticalOffset(this.EditorScrollOffsetY);

                // Highlight current line in editor (even if editor is not focused) via themable dp-property
                this.AdjustCurrentLineBackground();

                this.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            }
            catch
            {
                // catch this to make sure initialization can continue...
            }
        }

        /// <summary>
        /// Unhock event handlers and save editor states (to be recovered later)
        /// when the control is unloaded.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private void OnUnloaded(object obj, RoutedEventArgs args)
        {
            // http://stackoverflow.com/questions/11863273/avalonedit-how-to-get-the-top-visible-line
            this.EditorScrollOffsetX = this.TextArea.TextView.ScrollOffset.X;
            this.EditorScrollOffsetY = this.TextArea.TextView.ScrollOffset.Y;
        }

        /// <summary>
        /// Update Column and Line position properties when caret position is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            this.TextArea.TextView.InvalidateLayer(KnownLayer.Background); //Update current line highlighting

            if (this.TextArea != null)
            {
                this.Column = this.TextArea.Caret.Column;
                this.Line = this.TextArea.Caret.Line;
            }
            else
            {
                this.Column = 0;
                this.Line = 0;
            }
        }

        /// <summary>
        /// Is invoked when the collection bound on the <see cref="ItemsSource"/> dependency
        /// property has changed.
        /// </summary>
        /// <param name="newValue"></param>
        private void ItemsSourceChanged(object newValue)
        {
            // Get observable events should they be available
            if (_observeableDiffContext != null)
            {
                _observeableDiffContext.CollectionChanged -= Observeable_CollectionChanged;
                _observeableDiffContext = null;
            }

            var observeable = newValue as INotifyCollectionChanged;
            if (observeable != null)
                observeable.CollectionChanged += Observeable_CollectionChanged;

            _observeableDiffContext = observeable;

            this.InvalidateVisual();
        }

        private void Observeable_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.InvalidateVisual();
        }

        private void OnColorChanged(object newValue)
        {
            this.InvalidateVisual();
        }

        /// <summary>
        /// Method is invoked to record each time the view has been focused
        /// to record its last time of activation in <see cref="ActivationTimeStamp"/> property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiffView_GotFocus(object sender, RoutedEventArgs e)
        {
            ActivationTimeStamp = DateTime.Now;
        }
        #endregion methods
    }
}
