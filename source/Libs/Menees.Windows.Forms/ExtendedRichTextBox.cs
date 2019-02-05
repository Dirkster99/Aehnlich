namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Drawing;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Windows.Forms;

	#endregion

	/// <summary>
	/// Exposes additional properties and methods for a RichTextBox.
	/// </summary>
	public sealed class ExtendedRichTextBox : RichTextBox, IFindTarget
	{
		#region Private Data Members

		private int tabSpaces;
		private double pixelsPerDialogUnit = 2;
		private bool pasteTextOnly;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ExtendedRichTextBox()
		{
			this.pixelsPerDialogUnit = TextBoxUtility.CalcPixelsPerDialogUnit(this);

			// By default, most RichTextBoxes just need to paste in
			// text (not objects, images, formatted text, etc.).
			this.PasteTextOnly = true;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the current text in the rich text box.
		/// </summary>
		[DefaultValue("")]
		[RefreshProperties(RefreshProperties.All)]
		public override string Text
		{
			get
			{
				// NOTE: This property override exists so we can specify the DefaultValue("") attribute,
				// which allows the WinForms designer to NOT put a richTextBox1.Text = "" initialization
				// entry in every parent form's InitializeComponent method.  That use of "" would cause a
				// StyleCop violation (SA1122: Use string.Empty rather than "") in the generated code.
				return base.Text;
			}

			set
			{
				base.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of spaces a TAB character should be as wide as.  This only works for monospaced
		/// fonts (e.g., Courier New).  Set to 0 to use the edit control's default tab stops.
		/// </summary>
		[Browsable(true)]
		[DefaultValue(0)]
		[Category("Behavior")]
		[Description("The number of spaces a TAB character should be as wide as.  This only works for monospaced " +
			"fonts (e.g., Courier New).  Set to 0 to use the edit control's default tab stops.")]
		public int TabSpaces
		{
			get
			{
				return this.tabSpaces;
			}

			set
			{
				Conditions.RequireArgument(value >= 0, "TabSpaces must be non-negative.");

				if (value != this.tabSpaces)
				{
					this.tabSpaces = value;
					TextBoxUtility.SetTabWidth(this, this.tabSpaces, this.pixelsPerDialogUnit);
				}
			}
		}

		/// <summary>
		/// Gets whether text can currently be cut.
		/// </summary>
		[Browsable(false)]
		[Description("Whether text can currently be cut.")]
		public bool CanCut
		{
			get
			{
				bool result = this.SelectionLength > 0 && !this.ReadOnly;
				return result;
			}
		}

		/// <summary>
		/// Gets whether text can currently be copied.
		/// </summary>
		[Browsable(false)]
		[Description("Whether text can currently be copied.")]
		public bool CanCopy
		{
			get
			{
				bool result = this.SelectionLength > 0;
				return result;
			}
		}

		/// <summary>
		/// Gets whether text can currently be pasted.
		/// </summary>
		[Browsable(false)]
		[Description("Whether text can currently be pasted.")]
		public bool CanPasteText
		{
			get
			{
				bool result = TextBoxUtility.CanPasteText(this);
				return result;
			}
		}

		/// <summary>
		/// Gets or sets the 0-based line and column for the current caret position.
		/// </summary>
		[Browsable(false)]
		[Description("The 0-based line and column for the current caret position.")]
		[ReadOnly(true)]
		public Point CaretPoint
		{
			get
			{
				Point result = TextBoxUtility.GetCaretPoint(this);
				return result;
			}

			set
			{
				TextBoxUtility.SetCaretPoint(this, value);
			}
		}

		/// <summary>
		/// Gets the 0-based index of the uppermost visible line.
		/// </summary>
		[Browsable(false)]
		[Description("Gets the 0-based index of the uppermost visible line.")]
		public int FirstVisibleLine
		{
			get
			{
				int result = TextBoxUtility.GetFirstVisibleLine(this);
				return result;
			}
		}

		/// <summary>
		/// Gets the number of lines.  Always >= 1.
		/// </summary>
		[Browsable(false)]
		[Description("Gets the number of lines.  Always >= 1.")]
		public int LineCount
		{
			get
			{
				int result = TextBoxUtility.GetLineCount(this);
				return result;
			}
		}

		/// <summary>
		/// Gets or sets whether this control only allows plain text to be pasted
		/// (i.e., not images, objects, or rich formatted text).
		/// </summary>
		[DefaultValue(true)]
		[Description("Whether this control only allows plain text to be pasted.")]
		public bool PasteTextOnly
		{
			get
			{
				return this.pasteTextOnly;
			}

			set
			{
				this.pasteTextOnly = value;

				// If we're behaving as a text-only box, then we don't want the
				// rich text editing keys enabled.  In .NET 2.0 and 3.0, this is an
				// "undocumented" property, but it disables the following alignment keys:
				//
				// 	Ctrl+L - Left
				// 	Ctrl+E - Center
				// 	Ctrl+R - Right
				// 	Ctrl+J - Justified
				//
				// Note: We don't want to mess with the ShortcutsEnabled property
				// because that affects the above keys plus all of the copy-and-paste
				// and Undo/Redo keys too.
				this.RichTextShortcutsEnabled = !this.pasteTextOnly;
			}
		}

		/// <summary>
		/// Gets whether the <see cref="Find"/> method can be used.
		/// </summary>
		[Browsable(false)]
		public bool CanFind
		{
			get
			{
				bool result = this.TextLength > 0;
				return result;
			}
		}

		/// <summary>
		/// Gets or sets the caption to use for find operations.
		/// </summary>
		[DefaultValue(null)]
		[Description("The caption to use for find operations.")]
		public string FindCaption { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the current line of text that the cursor is on.
		/// </summary>
		/// <param name="selectLine">Whether the current line should be selected or not.</param>
		/// <returns>The current line text.</returns>
		public string GetCurrentLineText(bool selectLine)
		{
			int lineIndex = this.GetLineIndex(-1);
			int lineLength = this.GetLineLength(lineIndex);

			string result = string.Empty;

			if (lineIndex >= 0 && lineLength > 0)
			{
				int selStart = this.SelectionStart;
				int selLength = this.SelectionLength;

				this.Select(lineIndex, lineLength);

				string text = this.SelectedText;

				// Restore the previous selection if they don't want
				// the whole line selected.
				if (!selectLine)
				{
					this.Select(selStart, selLength);
				}

				result = text;
			}

			return result;
		}

		/// <summary>
		/// Gets the index of the first character of the specified line.
		/// </summary>
		/// <param name="lineNumber">The 0-based line index.</param>
		/// <returns>The 0-based character index.</returns>
		[Description("Gets the index of the first character of the specified line.")]
		public int GetLineIndex(int lineNumber)
		{
			int result = TextBoxUtility.GetLineIndex(this, lineNumber);
			return result;
		}

		/// <summary>
		/// Gets the length, in characters, of the specified line.
		/// </summary>
		/// <param name="lineNumber">The 0-based line number.</param>
		/// <returns>The line length.</returns>
		[Description("Gets the length, in characters, of the specified line.")]
		public int GetLineLength(int lineNumber)
		{
			int result = TextBoxUtility.GetLineLength(this, lineNumber);
			return result;
		}

		/// <summary>
		/// Gets the index of the line that contains the specified character index.
		/// </summary>
		/// <param name="characterIndex">The 0-based character index.</param>
		/// <returns>The 0-based line index.</returns>
		[Description("Gets the index of the line that contains the specified character index.")]
		public int GetLineFromChar(int characterIndex)
		{
			int result = TextBoxUtility.GetLineFromChar(this, characterIndex);
			return result;
		}

		/// <summary>
		/// Scrolls the text the specified number of characters horizontally and/or lines vertically.
		/// </summary>
		/// <param name="horizontalChars">The number of characters to scroll horizontally.</param>
		/// <param name="verticalLines">The number of lines to scroll vertically.</param>
		/// <returns>True for success.  False otherwise.</returns>
		[Description("Scrolls the text the specified number of characters horizontally and/or lines vertically.")]
		public bool Scroll(int horizontalChars, int verticalLines)
		{
			bool result = TextBoxUtility.Scroll(this, horizontalChars, verticalLines);
			return result;
		}

		/// <summary>
		/// Moves the caret to the 0-based line number.
		/// </summary>
		/// <param name="lineNumber">The 0-based line number.</param>
		/// <param name="selectLine">Whether the new line should be selected.</param>
		/// <returns>True if successful.  False otherwise.</returns>
		[Description("Moves the caret to the 0-based line number.")]
		public bool GotoLine(int lineNumber, bool selectLine)
		{
			bool result = TextBoxUtility.GotoLine(this, lineNumber, selectLine);
			return result;
		}

		/// <summary>
		/// Gets an ITextDocument interface to the editor's contents.
		/// </summary>
		/// <returns>A Text Object Model interface.  The caller can cast the result to tom.ITextDocument
		/// if they add a COM reference to msftedit.dll.</returns>
		[SuppressMessage(
			"Microsoft.Design",
			"CA1024:UsePropertiesWhereAppropriate",
			Justification = "This has to make several interop calls, which can be expensive.")]
		[Description("Gets an object that implements ITextDocument.")]
		public object GetOleInterface()
		{
			object result = null;

			IntPtr ptrOleInterface = IntPtr.Zero;

			const int EM_GETOLEINTERFACE = 0x043C; // 1084;
			int messageResult = NativeMethods.SendMessage(this, EM_GETOLEINTERFACE, 0, ref ptrOleInterface);
			if (messageResult != 0)
			{
				try
				{
					Guid textDocumentIid = new Guid("8CC497C0-A1DF-11CE-8098-00AA0047BE5D"); // IID_ITextDocument
					IntPtr textDocumentPtr;
					int hresult = Marshal.QueryInterface(ptrOleInterface, ref textDocumentIid, out textDocumentPtr);
					Marshal.ThrowExceptionForHR(hresult);
					try
					{
						result = Marshal.GetObjectForIUnknown(textDocumentPtr);
					}
					finally
					{
						Marshal.Release(textDocumentPtr);
					}
				}
				finally
				{
					Marshal.Release(ptrOleInterface);
				}
			}

			return result;
		}

		/// <summary>
		/// Finds the specified text in the target.
		/// </summary>
		/// <param name="findData">The text to search for.</param>
		/// <param name="findMode">Whether to find next, previous, or display a dialog.</param>
		/// <returns>True if the find text was found and selected.  False otherwise.</returns>
		public bool Find(FindData findData, FindMode findMode)
		{
			TextBoxFinder finder = new TextBoxFinder(this);
			bool result = finder.Find(this, findData, findMode);
			return result;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Called when the Enabled state has changed.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		protected override void OnEnabledChanged(EventArgs e)
		{
			// .NET 1.1 had a bug with RichTextBox's scroll bars.
			// After disabling and reenabling the control, the vertical scroll
			// bar would end up permenantly disabled.  But toggling the
			// ScrollBars property on reenable seems to fix it.
			if (this.Enabled && this.ScrollBars != RichTextBoxScrollBars.None)
			{
				RichTextBoxScrollBars original = this.ScrollBars;
				this.ScrollBars = RichTextBoxScrollBars.None;
				this.ScrollBars = original;
			}

			base.OnEnabledChanged(e);
		}

		/// <summary>
		/// Called when the font has changed.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			this.pixelsPerDialogUnit = TextBoxUtility.HandleFontChanged(this, this.tabSpaces);
		}

		/// <summary>
		/// Processes a command key message.
		/// </summary>
		/// <param name="m">The message to process.</param>
		/// <param name="keyData">The key data.</param>
		/// <returns>True if the control processed the key.</returns>
		protected override bool ProcessCmdKey(ref Message m, Keys keyData)
		{
			bool handled = false;

			if (keyData == (Keys.Control | Keys.V) ||
				keyData == (Keys.Shift | Keys.Insert))
			{
				if (this.PasteTextOnly)
				{
					// If we're supposed to paste text only, but we can't paste text,
					// then we still need to return true to indicate that we've handled
					// the key press as much as we can.  If we don't return true, then
					// the base handler will paste in the clipboard contents.
					if (this.CanPasteText)
					{
						this.Paste(DataFormats.GetFormat(DataFormats.Text));
					}

					handled = true;
				}
			}

			if (!handled)
			{
				handled = base.ProcessCmdKey(ref m, keyData);
			}

			return handled;
		}

		/// <summary>
		/// Determines whether the specified key is an input key or a special key that requires preprocessing
		/// </summary>
		/// <param name="keyData">A mask of Keys values.</param>
		/// <returns>True if the specified key is an input key; otherwise, false.</returns>
		protected override bool IsInputKey(Keys keyData)
		{
			// This allows us to get an Enter keypress even if the control is used on a form
			// with the AcceptButton set.  This is important when AcceptTab is false and
			// the control is just being used to enter multiple text lines (e.g., a list of file names)
			// and we want RichTextBox's ability to hide scroll bars until needed, show links, etc.
			//
			// Note: This implementation was copied from TextBox.IsInputKey using Reflector.
			// TextBox implements an AcceptsReturn property, but we'll assume returns should
			// be accepted any time the control is in multiline mode.
			bool result = false;
			if (this.Multiline && ((keyData & Keys.Alt) == Keys.None))
			{
				Keys keys = keyData & Keys.KeyCode;
				if (keys == Keys.Return)
				{
					result = true;
				}
			}

			if (!result)
			{
				result = base.IsInputKey(keyData);
			}

			return result;
		}

		#endregion
	}
}
