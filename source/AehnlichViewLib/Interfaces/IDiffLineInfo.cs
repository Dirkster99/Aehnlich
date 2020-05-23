namespace AehnlichViewLib.Interfaces
{
	using AehnlichViewLib.Enums;
	using ICSharpCode.AvalonEdit.Document;
	using System.Collections.Generic;

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
		DiffContext Context { get; }

		/// <summary>
		/// Gets null if this line is imaginary (has no representation in
		/// the original text) or the line number as it was available in the
		/// original text (without accounting for imaginary lines.
		/// </summary>
		int? ImaginaryLineNumber { get; }

		/// <summary>
		/// Gets the <see cref="ISegment"/> collection that describes the difference
		/// between 2 matched lines through their indicated background highlighting.
		/// </summary>
		IReadOnlyCollection<ISegment> LineEditScriptSegments { get; }

		/// <summary>
		/// Gets whether <see cref="LineEditScriptSegments"/> have previously been
		/// computed or whether these segments are yet to be computed on demand.
		///
		/// Gets false if the line has previously been matched to its counterpart line
		/// and true if matching is still to be performed on demand.
		/// </summary>
		bool LineEditScriptSegmentsIsDirty { get; }

		/// <summary>
		/// Gets whether this line is from the (left) viewA (usually used as reference)
		/// or not.
		/// </summary>
		bool FromA { get; }
	}
}