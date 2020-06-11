namespace AehnlichViewModelsLib.ViewModels
{
	using AehnlichViewLib.Controls.AvalonEditEx;
	using AehnlichViewLib.Enums;
	using ICSharpCode.AvalonEdit.Document;
	using System.ComponentModel;
	using System.Text;

	/// <summary>
	/// Defines an API for a text viewmodel for AvalonEdit that supports requirements for
	/// 1) Comparing text (with background diff highlighting) or
	/// 2) Editing text (without background diff highlighting) (see <see cref="ViewMode"/> for more details).
	/// </summary>
	public interface IDiffSideTextViewModel : INotifyPropertyChanged
	{
		/// <summary>Gets the <see cref="TextDocument"/> viewmodel of the attached AvalonEdit text editor control.</summary>
		TextDocument Document { get; }

		/// <summary>Gets the encoding of the text hosted in this viewmodel</summary>
		Encoding TextEncoding { get; }

		/// <summary>Gets/sets whether the currently shown text in the textedior has been changed
		/// without saving or not.</summary>
		bool IsDirty { get; set; }

		/// <summary>Gets whether the text displayed in the diff should be editable or not.</summary>
		bool IsReadOnly { get; }

		/// <summary>Gets a human readable reason why this might be readonly.</summary>
		string IsReadOnlyReason { get; }

		/// <summary>Gets the name of the file from which the content in this viewmodel was red.</summary>
		string FileName { get; }

		/// <summary>Gets a view mode indicating whether we are comparing text or editing text.</summary>
		DisplayMode ViewMode { get; }

		/// <summary>Gets the original text (not necessarily changed by editor).</summary>
		string OriginalText { get; }

		/// <summary>Gets the textbox controller that is used to drive the view
		/// from within the viewmodel (with event based commands like goto line x,y).</summary>
		TextBoxController TxtControl { get; }

		/// <summary>Gets the column of the text cursor position.</summary>
		int Column { get; }

		/// <summary>Gets the line of the text cursor position.</summary>
		int Line { get; }
	}
}