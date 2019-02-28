namespace AehnlichViewLib.Interfaces
{
    using AehnlichViewLib.Enums;

    /// <summary>
    /// Gets the elements per line that are relevant for the view to draw
    /// colored backgrounds and (imaginary) line numbers to indicate
    /// change operations (insert, delete, change, none) in order to
    /// sync both views.
    /// </summary>
    public interface IDiffLineInfo
    {
        /// <summary>
        /// Gets the change context for this line in comparison to its
        /// counterpart (insert, delete, change, none).
        /// </summary>
        DiffContext Context      { get; }

        /// <summary>
        /// Gets null if this line is imaginary (has no representation in
        /// the original text) or the line number as it was available in the
        /// original text (without accounting for imaginary lines.
        /// </summary>
        int? ImaginaryLineNumber { get; }
    }
}