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

        #region Diff Color Definitions
        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundBlank"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundBlankProperty =
            DependencyProperty.Register("ColorBackgroundBlank", typeof(SolidColorBrush),
                typeof(TextEditor), new PropertyMetadata(default(SolidColorBrush), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundAdded"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundAddedProperty =
            DependencyProperty.Register("ColorBackgroundAdded", typeof(SolidColorBrush),
                typeof(TextEditor), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xba, 0xff)), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundDeleted"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundDeletedProperty =
            DependencyProperty.Register("ColorBackgroundDeleted", typeof(SolidColorBrush),
                typeof(TextEditor), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x80, 0x80)), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundContext"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundContextProperty =
            DependencyProperty.Register("ColorBackgroundContext", typeof(SolidColorBrush),
                typeof(TextEditor), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x80, 0xFF, 0x80)), OnColorChanged));
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

        private static readonly DependencyProperty EditorCurrentLineBackgroundProperty =
            DependencyProperty.Register("EditorCurrentLineBackground",
                                         typeof(SolidColorBrush),
                                         typeof(DiffView),
                                         new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(33, 33, 33, 33)),
                                         DiffView.OnCurrentLineBackgroundChanged));

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
        /// default is <see cref="Default(SolidColorBrush)"/> - but sometimes it may be useful
        /// to color these lines which is why we have this property here.
        /// </summary>
        public SolidColorBrush ColorBackgroundBlank
        {
            get { return (SolidColorBrush)GetValue(ColorBackgroundBlankProperty); }
            set { SetValue(ColorBackgroundBlankProperty, value); }
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

        /// <summary>
        /// Style the background color of the current editor line
        /// </summary>
        public SolidColorBrush EditorCurrentLineBackground
        {
            get { return (SolidColorBrush)GetValue(EditorCurrentLineBackgroundProperty); }
            set { SetValue(EditorCurrentLineBackgroundProperty, value); }
        }
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
    
        /// <summary>
        /// The dependency property for has changed.
        /// Change the <seealso cref="SolidColorBrush"/> to be used for highlighting the current editor line
        /// in the particular <seealso cref="EdiTextEditor"/> control.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnCurrentLineBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (d is DiffView && e != null)
            {
                var view = d as DiffView;

                if (e.NewValue is SolidColorBrush)
                {
                    SolidColorBrush newValue = e.NewValue as SolidColorBrush;
                    view.AdjustCurrentLineBackground(newValue);
                }
            }
        }
        #endregion static handlers

        /// <summary>
        /// Reset the <seealso cref="SolidColorBrush"/> to be used for highlighting the current editor line.
        /// </summary>
        /// <param name="newValue"></param>
        private void AdjustCurrentLineBackground(SolidColorBrush newValue)
        {
            if (newValue != null)
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

                this.TextArea.TextView.BackgroundRenderers.Remove(oldRenderer);

                this.TextArea.TextView.BackgroundRenderers.Add(new HighlightCurrentLineBackgroundRenderer(this, newValue.Clone()));
            }
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
                line.SetBinding(System.Windows.Shapes.Line.StrokeProperty, lineNumbersForeground);
                lineNumbers.SetBinding(Control.ForegroundProperty, lineNumbersForeground);

                // Attach more event handlers
                this.GotFocus += DiffView_GotFocus;
                ////this.Focus();
                ////this.ForceCursor = true;

                // Restore cursor position for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
                this.ScrollToHorizontalOffset(this.EditorScrollOffsetX);
                this.ScrollToVerticalOffset(this.EditorScrollOffsetY);

                // Highlight current line in editor (even if editor is not focused) via themable dp-property
                this.AdjustCurrentLineBackground(this.EditorCurrentLineBackground);

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
