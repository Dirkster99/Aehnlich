namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	#endregion

	#region public IFindDialog

	/// <summary>
	/// Provides a generic interface to a Find dialog.
	/// </summary>
	public interface IFindDialog : IDisposable
	{
		/// <summary>
		/// Used to display the find dialog with the given data.
		/// </summary>
		/// <param name="owner">The dialog owner.</param>
		/// <param name="data">The find data.</param>
		/// <returns>True if OK was pressed.</returns>
		bool Execute(IWin32Window owner, FindData data);
	}

	#endregion

	#region public IFindTarget

	/// <summary>
	/// Provides a generic interface to a find target (e.g., a custom control or a window).
	/// </summary>
	public interface IFindTarget
	{
		/// <summary>
		/// Gets whether the <see cref="Find"/> method can be used.
		/// </summary>
		bool CanFind { get; }

		/// <summary>
		/// Gets the caption to use for dialogs displayed for this find target.
		/// </summary>
		string FindCaption { get; }

		/// <summary>
		/// Finds the specified text in the target.
		/// </summary>
		/// <param name="findData">The text to search for.</param>
		/// <param name="findMode">Whether to find next, previous, or display a dialog.</param>
		/// <returns>True if the find text was found and selected.  False otherwise.</returns>
		bool Find(FindData findData, FindMode findMode);
	}

	#endregion

	#region internal IOutputWindow

	// This is only implemented once for now, but it could also be
	// implemented by an HtmlOutputWindow if we ever need one.
	internal interface IOutputWindow : IFindTarget
	{
		bool HasSelection { get; }

		bool HasText { get; }

		bool IsFocused { get; }

		IWin32Window OwnerWindow { get; set; }

		string SelectedText { get; }

		bool WordWrap { get; set; }

		void Append(string message, Color color, int indentLevel, bool highlight, Guid outputId);

		void Clear();

		void Copy();

		bool FindNextHighlightPosition(bool searchForward, bool moveCurrentPosition);

		bool FindOutput(Guid outputId, bool moveCurrentPosition);

		void Focus();

		void SaveContent(string fileName, bool asRichText);

		void SelectAll();
	}

	#endregion
}
