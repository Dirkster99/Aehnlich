namespace DiffViewLib.Controls
{
    using DiffViewLib.Enums;
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.Rendering;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Implements a <see cref="TextEditor"/> based view that can be used to highlight
    /// text difference through line background coloring.
    /// </summary>
    public class DiffView : TextEditor
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
            DependencyProperty.Register("ColorBackgroundBlank", typeof(Color),
                typeof(TextEditor), new PropertyMetadata(default(Color), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundAdded"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundAddedProperty =
            DependencyProperty.Register("ColorBackgroundAdded", typeof(Color),
                typeof(TextEditor), new PropertyMetadata(Color.FromArgb(0xFF, 0x00, 0xba, 0xff), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundDeleted"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundDeletedProperty =
            DependencyProperty.Register("ColorBackgroundDeleted", typeof(Color),
                typeof(TextEditor), new PropertyMetadata(Color.FromArgb(0xFF, 0xFF, 0x80, 0x80), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundContext"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundContextProperty =
            DependencyProperty.Register("ColorBackgroundContext", typeof(Color),
                typeof(TextEditor), new PropertyMetadata(Color.FromArgb(0xFF, 0x80, 0xFF, 0x80), OnColorChanged));
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

        private readonly DiffLineBackgroundRenderer _DiffBackgroundRenderer;
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
            _DiffBackgroundRenderer = new DiffLineBackgroundRenderer(this);
            this.TextArea.TextView.BackgroundRenderers.Add(_DiffBackgroundRenderer);
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
        public Color ColorBackgroundContext
        {
            get { return (Color)GetValue(ColorBackgroundContextProperty); }
            set { SetValue(ColorBackgroundContextProperty, value); }
        }

        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies an element that is missing in one of the two (text) lines being compared.
        /// </summary>
        public Color ColorBackgroundDeleted
        {
            get { return (Color)GetValue(ColorBackgroundDeletedProperty); }
            set { SetValue(ColorBackgroundDeletedProperty, value); }
        }

        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies an element that is added in one of the two (text) lines being compared.
        /// </summary>
        public Color ColorBackgroundAdded
        {
            get { return (Color)GetValue(ColorBackgroundAddedProperty); }
            set { SetValue(ColorBackgroundAddedProperty, value); }
        }

        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies 2 blank lines in both of the two (text) lines being compared.
        /// 
        /// Normally, there should be no drawing required for this which is why the
        /// default is <see cref="Default(Color)"/> - but sometimes it may be useful
        /// to color these lines which is why we have this property here.
        /// </summary>
        public Color ColorBackgroundBlank
        {
            get { return (Color)GetValue(ColorBackgroundBlankProperty); }
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
        /// Hock event handlers and restore editor states (if any) or defaults
        /// when the control is fully loaded.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private void OnLoaded(object obj, RoutedEventArgs args)
        {
            try
            {
                this.Focus();
                this.ForceCursor = true;

                // Restore cursor position for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
                this.ScrollToHorizontalOffset(this.EditorScrollOffsetX);
                this.ScrollToVerticalOffset(this.EditorScrollOffsetY);

                this.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            }
            catch
            {
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
                this.Column = this.Line = 0;
        }
        /// <summary>
        /// Is invoked when the collection bound on the <see cref="ItemsSource"/> dependency
        /// property has changed.
        /// </summary>
        /// <param name="newValue"></param>
        private void ItemsSourceChanged(object newValue)
        {
            IReadOnlyList<DiffContext> newList = newValue as IReadOnlyList<DiffContext>;

            // ToDo Update UI
        }

        private void OnColorChanged(object newValue)
        {
            // ToDo Update UI
        }
        #endregion methods
    }
}
