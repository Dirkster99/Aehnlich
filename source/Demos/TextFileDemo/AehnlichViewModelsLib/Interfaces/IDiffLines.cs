namespace AehnlichViewModelsLib.Interfaces
{
	using AehnlichViewModelsLib.ViewModels;
	using System.Collections.Generic;

	/// <summary>
	/// Defines an interface to an object that contains a list of viewmodel
	/// objects that describe the difference in each line of text when compared
	/// to another text document.
	/// </summary>
	internal interface IDiffLines
	{
		/// <summary>
		/// Gets a collection of viewmodel objects that describe each line of text
		/// and whether they are equal or inequal and what the degree of equality is ...
		/// </summary>
		IReadOnlyCollection<DiffLineViewModel> DocLineDiffs { get; }

		int[] DiffEndLines { get; }
		int[] DiffStartLines { get; }

		/// <summary>
		/// Maximum imaginary line number which incorporates not only real text lines
		/// but also imaginary line that where inserted on either side of the comparison
		/// view to sync both sides into a consistent display.
		/// </summary>
		int MaxImaginaryLineNumber { get; }
	}
}
