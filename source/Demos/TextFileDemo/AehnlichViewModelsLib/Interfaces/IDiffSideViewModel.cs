namespace AehnlichViewModelsLib.Interfaces
{
	using AehnlichViewLib.Interfaces;
	using AehnlichViewModelsLib.Enums;
	using AehnlichViewModelsLib.Events;
	using AehnlichViewModelsLib.ViewModels;
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Defines the viewmodel that controls one side of a text diff view with two sides
	/// (left view A and right view B) where both views are synchronized towards the displayed
	/// line numbers and content being highlighted to visual differences (add, remove, change, no chaneg).
	/// </summary>
	public interface IDiffSideViewModel : ILineDiffProvider, IDiffView, IDisposable
	{
		#region Events
		/// <summary>
		/// Event is raised when the cursor position in the attached view is changed.
		/// </summary>
		event EventHandler<CaretPositionChangedEvent> CaretPositionChanged;
		#endregion Events

		#region Properties
		/// <summary>Gets the document viewmodel thats associated with the current (compare, edit) viewmode.</summary>
		DiffSideTextViewModel CurrentDocumentView { get; }

		/// <summary>
		/// Gets Text/binary specific diff options (eg. ignore white space) which are applied
		/// to compute the text differences shown in the view.
		/// </summary>
		ChangeDiffOptions ChangeDiffOptions { get; }

		/// <summary>
		/// Gets the name of the file from which the content in this viewmodel was red.
		/// </summary>
		string FileName { get; }

		#region DiffLines
		/// <summary>
		/// Gets a list of line information towards their difference when
		/// compared to the other document.
		/// </summary>
		IReadOnlyList<IDiffLineViewModel> DocLineDiffs { get; }

		/// <summary>
		/// Gets the number of line items available in the <see cref="DocLineDiffs"/> property.
		/// </summary>
		int LineCount { get; }

		/// <summary>
		/// Gets the line where a diff identified by an index i starts.
		/// (eg. the diff starts i=0 start at line 4 then we have DiffStartLines[0] == 4 )
		/// see also <see cref="DiffEndLines"/>
		/// </summary>
		int[] DiffStartLines { get; }

		/// <summary>
		/// Gets the line where a diff identified by an index i ends.
		/// (eg. the diff starts i=0 start at line 4 then we have DiffEndLines[0] == 4 )
		/// see also <see cref="DiffStartLines"/>
		/// </summary>
		int[] DiffEndLines { get; }
		#endregion DiffLines

		/// <summary>
		/// Gets a maximum imaginary line number which incorporates not only real text lines
		/// but also imaginary line that where inserted on either side of the comparison
		/// view to sync both sides into a consistent display.
		/// </summary>
		int MaxLineNumber { get; }
		#endregion Properties

		#region Methods
		/// <summary>
		/// Gets the position of the first difference in the text.
		/// </summary>
		/// <returns></returns>
		IDiffViewPosition GetFirstDiffPosition();

		/// <summary>
		/// Gets the position of the next difference in the text.
		/// </summary>
		/// <returns></returns>
		IDiffViewPosition GetNextDiffPosition();

		/// <summary>
		/// Gets the position of the previous difference in the text.
		/// </summary>
		/// <returns></returns>
		IDiffViewPosition GetPrevDiffPosition();

		/// <summary>
		/// Gets the position of the last difference in the text.
		/// </summary>
		/// <returns></returns>
		IDiffViewPosition GetLastDiffPosition();

		/// <summary>
		/// Scrolls the attached view to line <paramref name="n"/>
		/// where n should in the range of [1 ... max lines].
		/// </summary>
		/// <param name="n"></param>
		/// <param name="positionCursor"></param>
		void ScrollToLine(int n, bool positionCursor);

		/// <summary>
		/// (re)Sets the current caret position (column and line) in the text editor view.
		/// </summary>
		/// <param name="gotoPos"></param>
		void SetPosition(IDiffViewPosition gotoPos);

		#region FirstDiff NextDiff PrevDiff LastDiff
		/// <summary>
		/// Determine whether or not we can goto the first difference in the model
		/// based on the current caret position in Line and Column.
		/// 
		/// The function returns false:
		/// - if there is no difference or
		/// - if current positioning already indicates positioning on the 1st difference
		/// </summary>
		bool CanGoToFirstDiff();

		/// <summary>
		/// Determine whether or not we can goto the last difference in the model
		/// based on the current caret position in Line and Column.
		/// 
		/// The function returns false:
		/// - if there is no difference or
		/// - if current positioning already indicates positioning on the 1st difference
		/// </summary>
		bool CanGoToLastDiff();

		/// <summary>
		/// Determine whether or not we can goto the last difference in the model
		/// based on the current caret position in Line and Column.
		/// 
		/// The function returns false:
		/// - if there is no difference or
		/// - if current positioning already indicates positioning on the last difference
		/// </summary>
		bool CanGoToNextDiff();

		/// <summary>
		/// Determine whether or not we can goto the last difference in the model
		/// based on the current caret position in Line and Column.
		/// 
		/// The function returns false:
		/// - if there is no difference or
		/// - if current positioning already indicates positioning on the 1st difference
		/// </summary>
		bool CanGoToPreviousDiff();
		#endregion FirstDiff NextDiff PrevDiff LastDiff
		#endregion Methods
	}
}