namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Windows.Forms;

	#endregion

	/// <summary>
	/// Handles Find opertions for a TextBox.
	/// </summary>
	public class TextBoxFinder : Finder
	{
		#region Private Data Members

		private TextBoxBase textBox;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="textBox">The <see cref="System.Windows.Forms.TextBox"/>
		/// or <see cref="RichTextBox"/> control to search in.</param>
		public TextBoxFinder(TextBoxBase textBox)
		{
			this.TextBox = textBox;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the TextBoxBase to search in.
		/// </summary>
		public TextBoxBase TextBox
		{
			get
			{
				return this.textBox;
			}

			set
			{
				Conditions.RequireReference(value, "value");
				this.textBox = value;
			}
		}

		#endregion

		#region Protected Overrides

		/// <summary>
		/// Handles displaying the find dialog.
		/// </summary>
		/// <param name="findDialog">The dialog to display.</param>
		/// <param name="owner">The dialog owner.</param>
		/// <param name="findData">The find data.</param>
		/// <returns>True if the user pressed OK.</returns>
		protected override bool OnDialogExecute(IFindDialog findDialog, IWin32Window owner, FindData findData)
		{
			// Initialize the find text from the selection.
			string oldFindText = null;
			if (this.textBox.SelectionLength > 0)
			{
				// Only use the selection if it is one line or less.
				string selectedText = this.textBox.SelectedText;
				if (selectedText.IndexOf('\n') < 0)
				{
					oldFindText = findData.Text;
					findData.Text = this.textBox.SelectedText;
				}
			}

			// Call the base method to display the dialog.
			bool result = base.OnDialogExecute(findDialog, owner, findData);

			// If they canceled, then we may need to restore the old find text.
			if (!result && oldFindText != null)
			{
				findData.Text = oldFindText;
			}

			return result;
		}

		/// <summary>
		/// Finds the next instance.
		/// </summary>
		protected override bool OnFindNext()
		{
			string findText, editText;
			StringComparison comparison = this.GetStrings(out findText, out editText);

			// Search from the starting position to the end.
			int startingPosition = this.textBox.SelectionStart + this.textBox.SelectionLength;
			int findIndex = editText.IndexOf(findText, startingPosition, comparison);

			if (findIndex < 0)
			{
				// If not found, then search from the beginning to the starting position plus up to
				// FindText.Length extra characters in case the caret is already inside the match text.
				int count = Math.Min(startingPosition + findText.Length, editText.Length);
				findIndex = editText.IndexOf(findText, 0, count, comparison);
			}

			bool result = this.HandleFindIndex(findIndex);
			return result;
		}

		/// <summary>
		/// Finds the previous instance.
		/// </summary>
		protected override bool OnFindPrevious()
		{
			string findText, editText;
			StringComparison comparison = this.GetStrings(out findText, out editText);

			// Search from before the starting position to the beginning.  LastIndexOf includes
			// the starting index position in its search, so we need to subtract one.
			int startingPosition = this.textBox.SelectionStart - 1;
			int findIndex = startingPosition >= 0 ? editText.LastIndexOf(findText, startingPosition, comparison) : -1;

			if (findIndex < 0)
			{
				// If not found, then search from the end to the starting position plus up to
				// FindText.Length extra characters in case the caret is already inside the match text.
				int lastIndex = editText.Length;
				int count = Math.Min(lastIndex - startingPosition + findText.Length, lastIndex);
				findIndex = editText.LastIndexOf(findText, lastIndex, count, comparison);
			}

			bool result = this.HandleFindIndex(findIndex);
			return result;
		}

		#endregion

		#region Private Methods

		private StringComparison GetStrings(out string findText, out string editText)
		{
			findText = this.Data.Text;
			editText = this.textBox.Text;
			StringComparison result = this.Data.MatchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
			return result;
		}

		private bool HandleFindIndex(int findIndex)
		{
			bool result = false;

			if (findIndex >= 0)
			{
				this.textBox.SelectionStart = findIndex;
				this.textBox.SelectionLength = this.Data.Text.Length;
				this.textBox.ScrollToCaret();
				result = true;
			}
			else
			{
				this.Data.ReportNotFound(this.textBox);
			}

			return result;
		}

		#endregion
	}
}
