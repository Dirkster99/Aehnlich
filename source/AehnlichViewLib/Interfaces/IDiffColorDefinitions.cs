namespace AehnlichViewLib.Interfaces
{
    using System.Windows.Media;

    /// <summary>
    /// Gets a set of color definitions that can be bound to
    /// in one place to process all color definition based on one object binding rather
    /// than 5-6 additional bindings for each individual color.
    /// </summary>
    public interface IDiffColorDefinitions
    {
        /// <summary>
        /// Gets the background color that is applied when drawing areas that
        /// signifies 2 blank lines in both of the two (text) lines being compared.
        /// 
        /// Normally, there should be no drawing required for this which is why the
        /// default is default(<see cref="SolidColorBrush"/>) - but sometimes it may be useful
        /// to color these lines which is why we have this property here.
        /// </summary>
        SolidColorBrush ColorBackgroundBlank { get; }

        /// <summary>
        /// Gets the background color that is applied when drawing areas that
        /// signifies an element that is added in one of the two (text) lines being compared.
        /// </summary>
        SolidColorBrush ColorBackgroundAdded { get; }

        /// <summary>
        /// Gets the background color that is applied when drawing areas that
        /// signifies an element that is missing in one of the two (text) lines being compared.
        /// </summary>
        SolidColorBrush ColorBackgroundDeleted { get; }

        /// <summary>
        /// Gets the background color that is applied when drawing areas that
        /// signifies changed context (2 lines appear to be similar enough to align them
        /// but still mark them as different).
        /// </summary>
        SolidColorBrush ColorBackgroundContext { get; }

        /// <summary>
        /// Gets the background color that is applied when drawing areas that
        /// signifies an element that is added as an imaginary line in one of the two
		/// (text) lines being compared.
        /// </summary>
        SolidColorBrush ColorBackgroundImaginaryLineAdded { get; }

        /// <summary>
        /// Gets the background color that is applied when drawing areas that
        /// signifies an element that is missing in one of the two (text) lines being compared.
        /// </summary>
        SolidColorBrush ColorBackgroundImaginaryLineDeleted { get; }
    }
}