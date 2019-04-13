namespace AehnlichViewLib.Models
{
    using AehnlichViewLib.Interfaces;
    using System.Windows.Media;

    /// <summary>
    /// Implements an object that gets a set of color definitions that can be bound to
    /// in one place to process all color definition based on one object binding rather
    /// than 5-6 additional bindings for each individual color.
    /// </summary>
    public class DiffColorDefinitions : IDiffColorDefinitions
    {
        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="colorBackgroundBlank"></param>
        /// <param name="colorBackgroundAdded"></param>
        /// <param name="colorBackgroundDeleted"></param>
        /// <param name="colorBackgroundContext"></param>
        /// <param name="colorBackgroundImaginaryLineAdded"></param>
        /// <param name="colorBackgroundImaginaryLineDeleted"></param>
        /// <param name="colorForegroundBlank"></param>
        /// <param name="colorForegroundAdded"></param>
        /// <param name="colorForegroundContext"></param>
        /// <param name="colorForegroundDeleted"></param>
        public DiffColorDefinitions(SolidColorBrush colorBackgroundBlank,
                                    SolidColorBrush colorBackgroundAdded,
                                    SolidColorBrush colorBackgroundDeleted,
                                    SolidColorBrush colorBackgroundContext,
                                    SolidColorBrush colorBackgroundImaginaryLineAdded,
                                    SolidColorBrush colorBackgroundImaginaryLineDeleted,

                                    SolidColorBrush colorForegroundBlank,
                                    SolidColorBrush colorForegroundAdded,
                                    SolidColorBrush colorForegroundDeleted,
                                    SolidColorBrush colorForegroundContext)
            : this()
        {
            ColorBackgroundBlank = colorBackgroundBlank;
            ColorBackgroundAdded = colorBackgroundAdded;
            ColorBackgroundDeleted = colorBackgroundDeleted;
            ColorBackgroundContext = colorBackgroundContext;
            ColorBackgroundImaginaryLineAdded = colorBackgroundImaginaryLineAdded;
            ColorBackgroundImaginaryLineDeleted = colorBackgroundImaginaryLineDeleted;

            ColorForegroundBlank = colorForegroundBlank;
            ColorForegroundAdded = colorForegroundAdded;
            ColorForegroundDeleted = colorForegroundDeleted;
            ColorForegroundContext = colorForegroundContext;
        }

        /// <summary>
        /// Hidden class constructor.
        /// </summary>
        protected DiffColorDefinitions()
        {
        }
        #endregion ctors

        #region properties
        #region Background
        /// <summary>
        /// Gets the background color that is applied when drawing areas that
        /// signifies 2 blank lines in both of the two (text) lines being compared.
        /// 
        /// Normally, there should be no drawing required for this which is why the
        /// default is default(<see cref="SolidColorBrush"/>) - but sometimes it may be useful
        /// to color these lines which is why we have this property here.
        /// </summary>
        public SolidColorBrush ColorBackgroundBlank { get; }

        /// <summary>
        /// Gets the background color that is applied when drawing areas that
        /// signifies an element that is added in one of the two (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorBackgroundAdded { get; }

        /// <summary>
        /// Gets the background color that is applied when drawing areas that
        /// signifies an element that is missing in one of the two (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorBackgroundDeleted { get; }

        /// <summary>
        /// Gets the background color that is applied when drawing areas that
        /// signifies changed context (2 lines appear to be similar enough to align them
        /// but still mark them as different).
        /// </summary>
        public SolidColorBrush ColorBackgroundContext { get; }

        /// <summary>
        /// Gets the background color that is applied when drawing areas that
        /// signifies an element that is added as an imaginary line in one of the two
		/// (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorBackgroundImaginaryLineAdded { get; }

        /// <summary>
        /// Gets the background color that is applied when drawing areas that
        /// signifies an element that is missing in one of the two (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorBackgroundImaginaryLineDeleted { get; }
        #endregion Background

        #region Foreground
        /// <summary>
        /// Gets the Foreground color that is applied when drawing areas that
        /// signifies 2 blank lines in both of the two (text) lines being compared.
        /// 
        /// Normally, there should be no drawing required for this which is why the
        /// default is default(<see cref="SolidColorBrush"/>) - but sometimes it may be useful
        /// to color these lines which is why we have this property here.
        /// </summary>
        public SolidColorBrush ColorForegroundBlank { get; }

        /// <summary>
        /// Gets the Foreground color that is applied when drawing areas that
        /// signifies an element that is added in one of the two (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorForegroundAdded { get; }

        /// <summary>
        /// Gets the Foreground color that is applied when drawing areas that
        /// signifies an element that is missing in one of the two (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorForegroundDeleted { get; }

        /// <summary>
        /// Gets the Foreground color that is applied when drawing areas that
        /// signifies changed context (2 lines appear to be similar enough to align them
        /// but still mark them as different).
        /// </summary>
        public SolidColorBrush ColorForegroundContext { get; }
        #endregion Foreground
        #endregion properties
    }
}
