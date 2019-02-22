namespace DiffViewLib.Controls
{
    using DiffViewLib.Enums;
    using ICSharpCode.AvalonEdit;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// 
    /// </summary>
    public class DiffView : TextEditor
    {
        #region fields
        public static readonly DependencyProperty LineDiffsProperty =
            DependencyProperty.Register("LineDiffs", typeof(Dictionary<int, DiffContext>),
                typeof(DiffView), new PropertyMetadata(null));

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
        public Dictionary<int, DiffContext> LineDiffs
        {
            get { return (Dictionary<int, DiffContext>)GetValue(LineDiffsProperty); }
            set { SetValue(LineDiffsProperty, value); }
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
        #endregion properties

        #region methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion methods
    }
}
