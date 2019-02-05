namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.IO;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Windows.Forms;
	using tom;

	#endregion

	/// <summary>
	/// A read-only output window that supports rich text.
	/// </summary>
	[ToolboxBitmap(typeof(OutputWindow), "Images.OutputWindow.bmp")]
	public partial class OutputWindow : ExtendedUserControl, IOutputWindow, IFindTarget
	{
		#region Private Data Members

		private const string DefaultFindCaption = "Find In Output";

		private List<int> highlights = new List<int>();
		private Dictionary<Guid, int> outputIdToOffsetMap = new Dictionary<Guid, int>();

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public OutputWindow()
		{
			this.FindCaption = DefaultFindCaption;
			this.InitializeComponent();
			base.BorderStyle = BorderStyle.Fixed3D;
			this.output.BorderStyle = BorderStyle.None;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets whether the <see cref="Find"/> method can be used.
		/// </summary>
		[Browsable(false)]
		public bool CanFind => this.HasText;

		/// <summary>
		/// Gets or sets the caption to use for find operations.
		/// </summary>
		[DefaultValue(DefaultFindCaption)]
		[Description("The caption to use for find operations.")]
		public string FindCaption { get; set; }

		/// <summary>
		/// Gets whether any text is currently selected in the output window.
		/// </summary>
		[Browsable(false)]
		public bool HasSelection
		{
			get
			{
				bool result = this.output.SelectionLength > 0;
				return result;
			}
		}

		/// <summary>
		/// Gets whether any text is currently in the output window.
		/// </summary>
		[Browsable(false)]
		public bool HasText
		{
			get
			{
				bool result = this.output.TextLength > 0;
				return result;
			}
		}

		/// <summary>
		/// Gets whether the output window currently has focus.
		/// </summary>
		[Browsable(false)]
		public bool IsFocused
		{
			get
			{
				bool result = this.output.Focused;
				return result;
			}
		}

		/// <summary>
		/// Gets the form that owns the output window.
		/// </summary>
		[Browsable(false)]
		[DefaultValue(null)]
		public IWin32Window OwnerWindow
		{
			get
			{
				Form result = this.output.FindForm();
				return result;
			}

			set
			{
				IWin32Window currentOwner = this.OwnerWindow;

				// Don't let a caller try to change it to something else.
				if (currentOwner != null && currentOwner != value)
				{
					throw Exceptions.NewArgumentException("The OwnerWindow can only be set to the parent form.");
				}
			}
		}

		/// <summary>
		/// Gets the currently selected text in the output window.
		/// </summary>
		[Browsable(false)]
		public string SelectedText
		{
			get
			{
				string result = this.output.SelectedText;
				return result;
			}
		}

		/// <summary>
		/// Gets or sets whether the output window content should word wrap.
		/// </summary>
		[DefaultValue(false)]
		public bool WordWrap
		{
			get
			{
				return this.output.WordWrap;
			}

			set
			{
				this.output.WordWrap = value;
			}
		}

		/// <summary>
		/// Gets or sets the border style of the window.
		/// </summary>
		[DefaultValue(BorderStyle.Fixed3D)]
		public new BorderStyle BorderStyle
		{
			get
			{
				return base.BorderStyle;
			}

			set
			{
				base.BorderStyle = value;
			}
		}

		/// <summary>
		/// Gets or sets a delegate that can remove a custom line prefix after a line is double-clicked
		/// and before the line's file reference is parsed and opened.
		/// </summary>
		public Func<string, string> RemoveLinePrefix { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Appends formatted text to the output.
		/// </summary>
		/// <param name="message">The text to append.</param>
		/// <param name="color">The text color.</param>
		/// <param name="indentLevel">The indent level.</param>
		/// <param name="highlight">Whether this line should be considered a highlight.</param>
		/// <param name="outputId">A guid to uniquely identify this message.</param>
		public void Append(string message, Color color, int indentLevel, bool highlight, Guid outputId)
		{
			// Try to append using the Text Object Model COM interface because it
			// lets us append at the end without moving the caret/selection.  That
			// allows the user move around in the output while we're appending to it
			// just like Visual Studio's output window does.
			bool appended = this.AppendWithTom(message, color, indentLevel, highlight, outputId);
			if (!appended)
			{
				// If the TOM COM API failed, then append the old way.
				this.AppendWithSelection(message, color, indentLevel, highlight, outputId);
			}
		}

		/// <summary>
		/// Clears the output window.
		/// </summary>
		public void Clear()
		{
			this.highlights.Clear();
			this.outputIdToOffsetMap.Clear();
			this.output.Clear();
		}

		/// <summary>
		/// Copies the currently selected text to the clipboard.
		/// </summary>
		public void Copy()
		{
			this.output.Copy();
		}

		/// <summary>
		/// Finds the specified text in the output window.
		/// </summary>
		/// <param name="findData">The text to search for.</param>
		/// <param name="findMode">Whether to find next, previous, or display a dialog.</param>
		/// <returns>True if the find text was found and selected.  False otherwise.</returns>
		public bool Find(FindData findData, FindMode findMode)
		{
			TextBoxFinder finder = new TextBoxFinder(this.output);
			bool result = finder.Find(this.OwnerWindow, findData, findMode);
			return result;
		}

		/// <summary>
		/// Finds the next highlight position.
		/// </summary>
		/// <param name="searchForward">Whether to search forward/down (if true) or backward/up (if false).</param>
		/// <param name="moveCurrentPosition">Whether to move and scroll to the next highlight if found.</param>
		/// <returns>True if the next highlight was found.  False otherwise.</returns>
		public bool FindNextHighlightPosition(bool searchForward, bool moveCurrentPosition)
		{
			int selectionStart = this.output.SelectionStart;
			var highlight = searchForward ? this.GetNextHighlight(selectionStart) : this.GetPreviousHighlight(selectionStart);
			if (moveCurrentPosition)
			{
				this.GoToHighlight(highlight);
			}

			return highlight >= 0;
		}

		/// <summary>
		/// Finds the output entry with the specified guid.
		/// </summary>
		/// <param name="outputId">A guid that identifies an appended message.</param>
		/// <param name="moveCurrentPosition">Whether to move and scroll to the matched output if found.</param>
		/// <returns>True if the specified output was found.  False otherwise.</returns>
		public bool FindOutput(Guid outputId, bool moveCurrentPosition)
		{
			int offset;
			bool result = this.outputIdToOffsetMap.TryGetValue(outputId, out offset);
			if (result && moveCurrentPosition)
			{
				this.GoToHighlight(offset);
			}

			return result;
		}

		/// <summary>
		/// Saves the output window contents to a file.
		/// </summary>
		/// <param name="fileName">The name of the file to save to.</param>
		/// <param name="asRichText">True to save as rich text (RTF) or false to save as plain text.</param>
		public void SaveContent(string fileName, bool asRichText)
		{
			var format = asRichText ? RichTextBoxStreamType.RichText : RichTextBoxStreamType.PlainText;
			this.output.SaveFile(fileName, format);
		}

		/// <summary>
		/// Selects all the text in the output window.
		/// </summary>
		public void SelectAll()
		{
			this.output.SelectAll();
		}

		#endregion

		#region IOutputWindow Members

		void IOutputWindow.Focus()
		{
			this.Focus();
		}

		#endregion

		#region Internal Methods

		internal static bool OpenLineFileReference(IWin32Window owner, string currentLine)
		{
			bool result = false;

			if (!string.IsNullOrWhiteSpace(currentLine))
			{
				currentLine = currentLine.Trim();

				// Parse the file path out of a line like:
				// c:\projects\csharp\megabuild\forms\mainform.cs(2495,17): error CS1002: ; expected
				// But also work with something like:
				// c:\projects\csharp\megabuild\forms\mainform.cs
				// And with threaded C++ builds like:
				// 16>c:\projects\csharp\megabuild\forms\mainform.cs(2495,17): error CS1002: ; expected
				//
				// See if VC++ has put a build thread ID on the front of the line (e.g., 16>...).
				string path = currentLine;
				int threadTokenIndex = path.IndexOf('>');
				int threadId;
				if (threadTokenIndex >= 0 && threadTokenIndex < path.Length && int.TryParse(path.Substring(0, threadTokenIndex), out threadId))
				{
					path = path.Substring(threadTokenIndex + 1);
				}

				// A drive-letter path or a UNC path can never have a colon at index 0 or 1.
				int colonPos = path.IndexOf(':', 2);
				if (colonPos >= 2)
				{
					path = path.Substring(0, colonPos);
				}

				// If it ends with "(###)", strip the line/column numbers off.
				int parenIndex = path.IndexOf('(');
				string lineNumber = string.Empty;
				if (parenIndex >= 0)
				{
					lineNumber = TextUtility.StripQuotes(path.Substring(parenIndex).Trim(), "(", ")").Trim();
					path = path.Substring(0, parenIndex);

					// If the "line number" also contains a column number (e.g., (123,21)),
					// then we have throw out the column information.
					for (int i = 0; i < lineNumber.Length; i++)
					{
						if (!char.IsDigit(lineNumber[i]))
						{
							lineNumber = lineNumber.Substring(0, i);
							break;
						}
					}
				}

				path = path.Trim();
				if (File.Exists(path) || Directory.Exists(path))
				{
					// If we have line information assume it came from a Visual Studio build.
					if (lineNumber.Length == 0 || !(result = VisualStudioInvoker.OpenFile(path, lineNumber)))
					{
						result = WindowsUtility.ShellExecute(owner, path);
					}
				}
			}

			return result;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Called when the control's context menu strip has changed.
		/// </summary>
		protected override void OnContextMenuStripChanged(EventArgs e)
		{
			this.output.ContextMenuStrip = this.ContextMenuStrip;
			base.OnContextMenuStripChanged(e);
		}

		#endregion

		#region Private Event Handlers

		private void RichTextBox_DoubleClick(object sender, EventArgs e)
		{
			string currentLine = this.output.GetCurrentLineText(false);

			currentLine = this.RemoveLinePrefix?.Invoke(currentLine) ?? currentLine;

			OpenLineFileReference(this.OwnerWindow, currentLine);
		}

		private void RichTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
		{
			WindowsUtility.ShellExecute(this.OwnerWindow, e.LinkText);
		}

		#endregion

		#region Private Methods

		private void GoToHighlight(int index)
		{
			if (index >= 0)
			{
				this.output.Select(index, 0);
				this.output.Focus();
				this.output.ScrollToCaret();
			}
		}

		private int GetPreviousHighlight(int selectionStart)
		{
			int result = -1;

			// Do a simple linear search backwards to find the previous index.
			int numHighlights = this.highlights.Count;
			for (int i = numHighlights - 1; i >= 0; i--)
			{
				int highlight = this.highlights[i];
				if (highlight < selectionStart)
				{
					result = highlight;
					break;
				}
			}

			return result;
		}

		private int GetNextHighlight(int selectionStart)
		{
			int result = -1;

			// They're in sorted order, but I'm just going to do a simple linear search.
			// There would have to be thousands of highlights before anyone would
			// notice a speed difference.
			foreach (int highlight in this.highlights)
			{
				if (highlight > selectionStart)
				{
					result = highlight;
					break;
				}
			}

			return result;
		}

		private void MoveCaretToEnd()
		{
			// I'm intentionally not passing TextLength because Select always
			// calls that internally anyway.  So we might as well let it do it.
			this.output.Select(int.MaxValue, 0);
		}

		private void ScrollToBottom()
		{
			// ScrollToCaret only works if the Output window has the focus, so this is more reliable.
			// Also, ScrollToCaret and ITextRange.ScrollIntoView's scrolling are occasionally "jumpy"
			// and don't always scroll consistently to the very bottom.
			const int WM_VSCROLL = 0x0115;
			const int SB_BOTTOM = 7;
			NativeMethods.SendMessage(this.output, WM_VSCROLL, SB_BOTTOM, 0);
		}

		private void AppendWithSelection(string message, Color color, int indentLevel, bool highlight, Guid outputId)
		{
			// Force the cursor to the end.  This isn't necessary for
			// AppendText, but for the color and indent settings it is.
			this.MoveCaretToEnd();

			// Store the index if this is a highlighted message.
			if (highlight)
			{
				// The indexes stay in sorted order because we're only appending text.
				this.highlights.Add(this.output.SelectionStart);
			}

			// If they assigned a guid to this output, the store its location.
			// I'm using Dictionary.Add here, so they'll get an error if they
			// try to reuse a guid for multiple outputs.
			if (outputId != Guid.Empty)
			{
				this.outputIdToOffsetMap.Add(outputId, this.output.SelectionStart);
			}

			// Color
			this.output.SelectionColor = color;

			// Indent
			const int RichTextBoxDefaultTabStopWidth = 36; // From ITextDocument.DefaultTabStop
			this.output.SelectionIndent = RichTextBoxDefaultTabStopWidth * Math.Max(indentLevel, 0);

			// Message
			if (message != null)
			{
				this.output.AppendText(message);
			}

			this.ScrollToBottom();
		}

		private bool AppendWithTom(string message, Color color, int indentLevel, bool highlight, Guid outputId)
		{
			bool result = false;

			using (var doc = new ComInterfaceRef<ITextDocument>((ITextDocument)this.output.GetOleInterface()))
			{
				if (doc.Ref != null)
				{
					// If the caret is already at the end of the document, then we'll
					// automatically scroll to the bottom after we append.
					bool scrollToBottom = this.output.SelectionStart >= this.output.TextLength;

					// We have to make sure the RichEdit control is writable (at least temporarily).
					// Otherwise, ITextRange.CanEdit returns false, and none of ITextRange's methods
					// would let us make changes.
					bool previousReadOnlyState = this.output.ReadOnly;
					this.output.ReadOnly = false;
					try
					{
						// Get a new range and insertion point at the very end of the document.
						using (var range = new ComInterfaceRef<ITextRange>(doc.Ref.Range(int.MaxValue, int.MaxValue)))
						{
							if (range.Ref != null && range.Ref.CanEdit() != 0)
							{
								bool setIndents = false;
								using (var para = new ComInterfaceRef<ITextPara>(range.Ref.Para))
								{
									if (para.Ref != null)
									{
										para.Ref.SetIndents(0, doc.Ref.DefaultTabStop * Math.Max(0, indentLevel), 0);
										setIndents = true;
									}
								}

								bool setColor = false;
								using (var font = new ComInterfaceRef<ITextFont>(range.Ref.Font))
								{
									if (font.Ref != null)
									{
										// Adapted from the RGB macro in WinGDI.h
										const int BitsPerByte = 8;
										font.Ref.ForeColor = color.R | (color.G << BitsPerByte) | (color.B << (2 * BitsPerByte));
										setColor = true;
									}
								}

								// If we were able to set all the options, then go ahead and append the text.
								if (setIndents && setColor)
								{
									range.Ref.Text = message;
									result = true;

									if (highlight)
									{
										this.highlights.Add(range.Ref.Start);
									}

									if (outputId != Guid.Empty)
									{
										this.outputIdToOffsetMap.Add(outputId, range.Ref.Start);
									}

									// If we're supposed to automatically scroll to the bottom, then we
									// need to move the caret to the end, so we'll be able to detect
									// the auto-scrolling need correctly on the next Append too.
									if (scrollToBottom)
									{
										this.MoveCaretToEnd();
										this.ScrollToBottom();
									}
								}
							}
						}
					}
					finally
					{
						this.output.ReadOnly = previousReadOnlyState;
					}
				}
			}

			return result;
		}

		#endregion

		#region Private Types

		/// <summary>
		/// This is a disposable type so we can deterministically release a COM
		/// reference as soon as we're done with it.  Otherwise, we end up with
		/// COM objects that outlive the context they're running in, and Visual
		/// Studio raises "DisconnectedContext was detected" MDA warnings in
		/// the debugger.
		/// </summary>
		private sealed class ComInterfaceRef<T> : IDisposable
			where T : class
		{
			#region Constructors

			public ComInterfaceRef(T reference)
			{
				this.Ref = reference;
			}

			#endregion

			#region Public Properties

			public T Ref { get; private set; }

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				if (this.Ref != null)
				{
					Marshal.ReleaseComObject(this.Ref);
					this.Ref = null;
				}
			}

			#endregion
		}

		#endregion
	}
}
