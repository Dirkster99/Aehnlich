namespace Menees.Diffs.Controls
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;
    using DiffLib.Enums;
    using DiffLib.Text;
	using Menees.Windows.Forms;

	/// <summary>
	/// Single pane that a diff utility would display on the left or right.
	/// </summary>
	internal sealed class DiffView : Control
	{
		#region Private Data Members

		private const int CaretWidth = 2;
		private const int GutterSeparatorWidth = 2;
		private const TextFormatFlags DefaultTextFormat =
			TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.PreserveGraphicsTranslateTransform |
			TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.TextBoxControl;

		private static readonly Size MaxSize = new Size(int.MaxValue, int.MaxValue);

		private Timer autoScrollTimer;
		private bool capturedMouse;
		private BorderStyle borderStyle = BorderStyle.Fixed3D;
		private bool showWhitespace;
		private Caret caret;
		private int charWidth = 1;
		private int gutterWidth = 1;
		private int horizontalAutoScrollAmount;
		private int lineHeight = 1;
		private int verticalAutoScrollAmount;
		private int wheelDelta;
		private DiffViewLines lines;
		private DiffViewPosition position;
		private DiffViewPosition selectionStart = DiffViewPosition.Empty;
		private string gutterFormat = "{0}";

		#endregion

		#region Constructors

		[SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges",
			Justification = "This timer is only used to auto-scroll while the mouse is captured and dragging.")]
		public DiffView()
		{
			// Set some important control styles
			this.SetStyle(ControlStyles.Opaque, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.Selectable, true);
			this.SetStyle(ControlStyles.StandardClick, true);
			this.SetStyle(ControlStyles.StandardDoubleClick, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);

			this.position = new DiffViewPosition(0, 0);

			this.UpdateTextMetrics(true);

			this.autoScrollTimer = new Timer();
			this.autoScrollTimer.Enabled = false;
			this.autoScrollTimer.Interval = 100;
			this.autoScrollTimer.Tick += this.AutoScrollTimer_Tick;

			DiffOptions.OptionsChanged += this.DiffOptionsChanged;

			this.Cursor = Cursors.IBeam;
		}

		#endregion

		#region Public Events

		public event EventHandler HScrollPosChanged;

		public event EventHandler LinesChanged;

		public event EventHandler PositionChanged;

		public event EventHandler SelectionChanged;

		public event EventHandler VScrollPosChanged;

		#endregion

		#region Private Enums

		private enum TokenCharacterType
		{
			// We haven't determined the character type yet.
			Unknown,

			// A letter, number, or underscore
			Identifier,

			// Whitespace
			Whitespace,

			// Another non-whitespace, non-identifier character
			Other
		}

		#endregion

		#region Public Properties

		[Browsable(false)]
		public bool CanGoToFirstDiff
		{
			get
			{
				bool result = false;

				if (this.lines != null)
				{
					int[] starts = this.lines.DiffStartLines;
					int[] ends = this.lines.DiffEndLines;
					result = starts.Length > 0 && ends.Length > 0 && (this.position.Line < starts[0] || this.position.Line > ends[0]);
				}

				return result;
			}
		}

		[Browsable(false)]
		public bool CanGoToLastDiff
		{
			get
			{
				bool result = false;

				if (this.lines != null)
				{
					int[] starts = this.lines.DiffStartLines;
					int[] ends = this.lines.DiffEndLines;
					result = starts.Length > 0 && ends.Length > 0 &&
						(this.position.Line < starts[starts.Length - 1] || this.position.Line > ends[ends.Length - 1]);
				}

				return result;
			}
		}

		[Browsable(false)]
		public bool CanGoToNextDiff
		{
			get
			{
				bool result = false;
				if (this.lines != null)
				{
					int[] starts = this.lines.DiffStartLines;
					result = starts.Length > 0 && this.position.Line < starts[starts.Length - 1];
				}

				return result;
			}
		}

		[Browsable(false)]
		public bool CanGoToPreviousDiff
		{
			get
			{
				bool result = false;

				if (this.lines != null)
				{
					int[] ends = this.lines.DiffEndLines;
					result = ends.Length > 0 && this.position.Line > ends[0];
				}

				return result;
			}
		}

		[Browsable(false)]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
			Justification = "The get_CenterVisibleLine accessor is only called by the Windows Forms designer via reflection.")]
		public int CenterVisibleLine
		{
			get
			{
				int result = this.FirstVisibleLine + (this.VisibleLineCount / 2);
				return result;
			}

			set
			{
				// Make this line the center of the view.
				int firstLine = value - (this.VisibleLineCount / 2);
				this.FirstVisibleLine = firstLine;
			}
		}

		public ChangeDiffOptions ChangeDiffOptions
		{
			get;
			set;
		}

		[Browsable(false)]
		public int FirstVisibleLine
		{
			get
			{
				return this.VScrollPos;
			}

			set
			{
				this.VScrollPos = value;
			}
		}

		public bool HasSelection => this.selectionStart != DiffViewPosition.Empty;

		[Browsable(false)]
		public int HScrollPos
		{
			get
			{
				return NativeMethods.GetScrollPos(this, true);
			}

			set
			{
				this.ScrollHorizontally(value, this.HScrollPos);
			}
		}

		[Browsable(false)]
		public int LineCount => this.lines != null ? this.lines.Count : 0;

		/// <summary>
		/// Stores each line's text, color, and original number.
		/// </summary>
		[Browsable(false)]
		public DiffViewLines Lines => this.lines;

		[Browsable(false)]
		public DiffViewPosition Position
		{
			get
			{
				return this.position;
			}

			set
			{
				if (!this.position.Equals(value))
				{
					this.SetPosition(value.Line, value.Column, true, false);
				}
			}
		}

		public string SelectedText
		{
			get
			{
				if (!this.HasSelection || this.lines == null)
				{
					return string.Empty;
				}
				else
				{
					DiffViewPosition startSel, endSel;
					this.GetForwardOrderSelection(out startSel, out endSel);

					int numLines = endSel.Line - startSel.Line + 1;
					StringBuilder sb = new StringBuilder(numLines * 50);

					for (int i = startSel.Line; i <= endSel.Line; i++)
					{
						// Leave out lines that are only in the display for alignment
						// purposes.  This makes SelectedText useful for "Compare Text",
						// and typically much more useful for "Copy".
						DiffViewLine line = this.lines[i];
						if (line.Number.HasValue)
						{
							DisplayLine displayLine = this.GetDisplayLine(line);
							int displayLength = displayLine.GetDisplayTextLength();
							int selStartColumn = (i == startSel.Line) ? startSel.Column : 0;
							int selEndColumn = (i == endSel.Line) ? Math.Min(endSel.Column, displayLength) : displayLength;

							bool lineFullySelected = (i > startSel.Line && i < endSel.Line) || (selStartColumn == 0 && selEndColumn == displayLength);
							string originalText;
							if (lineFullySelected)
							{
								originalText = line.Text;
							}
							else
							{
								originalText = displayLine.GetTextBetweenDisplayColumns(selStartColumn, selEndColumn);
							}

							sb.Append(originalText);
							if (i != endSel.Line)
							{
								sb.AppendLine();
							}
						}
					}

					return sb.ToString();
				}
			}
		}

		[DefaultValue(false)]
		public bool ShowWhitespace
		{
			get
			{
				return this.showWhitespace;
			}

			set
			{
				if (this.showWhitespace != value)
				{
					this.showWhitespace = value;
					this.Invalidate();
				}
			}
		}

		[Browsable(false)]
		public int VisibleLineCount => this.ClientSize.Height / this.lineHeight;

		[Browsable(false)]
		public int VScrollPos
		{
			get
			{
				return NativeMethods.GetScrollPos(this, false);
			}

			set
			{
				this.ScrollVertically(value, this.VScrollPos);
			}
		}

		#endregion

		#region Protected Properties

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams result = base.CreateParams;
				result.Style = result.Style | NativeMethods.WS_VSCROLL | NativeMethods.WS_HSCROLL;
				NativeMethods.SetBorderStyle(result, this.borderStyle);
				return result;
			}
		}

		#endregion

		#region Public Methods

		public bool Find(FindData data)
		{
			// If text is selected on a single line, then use that for the new Find text.
			string originalFindText = data.Text;
			string selectedText;
			if (this.GetSingleLineSelectedText(out selectedText))
			{
				data.Text = selectedText;
			}

			using (FindDialog dialog = new FindDialog())
			{
				if (dialog.Execute(this, data))
				{
					if (data.SearchUp)
					{
						return this.FindPrevious(data);
					}
					else
					{
						return this.FindNext(data);
					}
				}
				else
				{
					// Reset the Find text if the user cancelled.
					data.Text = originalFindText;
				}
			}

			return false;
		}

		public bool FindNext(FindData data)
		{
			if (string.IsNullOrEmpty(data.Text))
			{
				data.SearchUp = false;
				return this.Find(data);
			}
			else
			{
				int numLines = this.LineCount;
				if (numLines > 0)
				{
					string text = data.Text;
					if (!data.MatchCase)
					{
						text = text.ToUpper();
					}

					DiffViewPosition startPosition = this.GetFindStartPosition(false);
					int lastLineLastColumn = startPosition.Column;

					// Use <= so we check the start line again from the beginning
					for (int i = 0; i <= numLines; i++)
					{
						// Use % so we wrap around at the end.
						int lineNumber = (startPosition.Line + i) % numLines;

						// This needs to search the original text.
						DisplayLine displayLine = this.GetDisplayLine(lineNumber);
						string line = displayLine.OriginalText;

						if (!data.MatchCase)
						{
							line = line.ToUpper();
						}

						int index;
						if (i == numLines)
						{
							// We're rechecking the start line from the beginning.
							index = line.IndexOf(text, 0, displayLine.GetTextIndexFromDisplayColumn(lastLineLastColumn));
						}
						else
						{
							index = line.IndexOf(text, displayLine.GetTextIndexFromDisplayColumn(startPosition.Column));
						}

						if (index >= 0)
						{
							this.GoToPosition(lineNumber, displayLine.GetDisplayColumnFromTextIndex(index));
							this.ExtendSelection(0, text.Length);
							return true;
						}

						// On all lines but the first, we need to start at 0
						startPosition = new DiffViewPosition(startPosition.Line, 0);
					}
				}

				string message = string.Format("'{0}' was not found.", data.Text);
				MessageBox.Show(this, message, "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);

				return false;
			}
		}

		public bool FindPrevious(FindData data)
		{
			if (string.IsNullOrEmpty(data.Text))
			{
				data.SearchUp = true;
				return this.Find(data);
			}
			else
			{
				int numLines = this.LineCount;
				if (numLines > 0)
				{
					string text = data.Text;
					if (!data.MatchCase)
					{
						text = text.ToUpper();
					}

					DiffViewPosition startPosition = this.GetFindStartPosition(true);
					int lastLineLastColumn = startPosition.Column;

					// Use <= so we check the start line again from the end
					for (int i = 0; i <= numLines; i++)
					{
						// Use % so we wrap around at the end.
						int lineNumber = (startPosition.Line - i + numLines) % numLines;

						// This needs to search the original text.
						DisplayLine displayLine = this.GetDisplayLine(lineNumber);
						string line = displayLine.OriginalText;

						if (!data.MatchCase)
						{
							line = line.ToUpper();
						}

						const int StartAtEndColumn = -1;
						if (startPosition.Column == StartAtEndColumn)
						{
							startPosition = new DiffViewPosition(startPosition.Line, Math.Max(0, displayLine.GetDisplayTextLength()));
						}

						int index;
						if (i == numLines)
						{
							// We're rechecking the start line from the end.
							int startIndex = displayLine.GetTextIndexFromDisplayColumn(startPosition.Column);
							int lastIndex = displayLine.GetTextIndexFromDisplayColumn(lastLineLastColumn);
							index = line.LastIndexOf(text, startIndex, startIndex - lastIndex + 1);
						}
						else
						{
							index = line.LastIndexOf(text, displayLine.GetTextIndexFromDisplayColumn(startPosition.Column));
						}

						if (index >= 0)
						{
							this.GoToPosition(lineNumber, displayLine.GetDisplayColumnFromTextIndex(index));
							this.ExtendSelection(0, text.Length);
							return true;
						}

						// On all lines but the first, we need to start at the end
						startPosition = new DiffViewPosition(startPosition.Line, StartAtEndColumn);
					}
				}

				string message = string.Format("'{0}' was not found.", data.Text);
				MessageBox.Show(this, message, "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);

				return false;
			}
		}

		public Point GetPointFromPos(int line, int column)
		{
			int y = (line - this.VScrollPos) * this.lineHeight;

			// Because we're not guaranteed to have a monospaced font,
			// this gets tricky.  We have to measure the substring to
			// get the correct X.
			DisplayLine displayLine = this.GetDisplayLine(line);
			using (Graphics g = Graphics.FromHwnd(this.Handle))
			{
				int x = this.GetXForColumn(g, displayLine, null, column);
				return new Point(x, y);
			}
		}

		public DiffViewPosition GetPosFromPoint(int x, int y)
		{
			int line = (y / this.lineHeight) + this.VScrollPos;

			// Because we're not guaranteed to have a monospaced font,
			// this gets tricky.  We have to make an initial guess at
			// the column, and then we'll converge to the best one.
			DisplayLine displayLine = this.GetDisplayLine(line);
			string text = displayLine.GetDisplayText();

			// Make a starting guess.  Because of tabs and variable width
			// fonts, this may be nowhere near the right place...
			int column = (int)((x - this.gutterWidth + (this.HScrollPos * this.charWidth)) / this.charWidth);

			using (Graphics g = Graphics.FromHwnd(this.Handle))
			{
				int textLength = text.Length;
				int columnGreater = -1;
				int columnLess = -1;

				int columnX = this.GetXForColumn(g, displayLine, text, column);
				if (columnX != x)
				{
					if (columnX > x)
					{
						columnGreater = column;
						columnLess = 0;
						for (column = columnGreater - 1; column >= 0; column--)
						{
							columnX = this.GetXForColumn(g, displayLine, text, column);
							if (columnX > x)
							{
								columnGreater = column;
							}
							else
							{
								columnLess = column;
								break;
							}
						}
					}
					else
					{
						// columnX < X
						columnLess = column;
						columnGreater = textLength;
						for (column = columnLess + 1; column <= textLength; column++)
						{
							columnX = this.GetXForColumn(g, displayLine, text, column);
							if (columnX < x)
							{
								columnLess = column;
							}
							else
							{
								columnGreater = column;
								break;
							}
						}
					}

					columnGreater = EnsureInRange(0, columnGreater, textLength);
					columnLess = EnsureInRange(0, columnLess, textLength);

					int greaterX = this.GetXForColumn(g, displayLine, text, columnGreater);
					int lessX = this.GetXForColumn(g, displayLine, text, columnLess);

					if (Math.Abs(greaterX - x) < Math.Abs(lessX - x))
					{
						column = columnGreater;
					}
					else
					{
						column = columnLess;
					}
				}
			}

			return new DiffViewPosition(line, column);
		}

		public bool GoToFirstDiff()
		{
			if (!this.CanGoToFirstDiff)
			{
				return false;
			}

			this.GoToPosition(this.lines.DiffStartLines[0], this.position.Column);
			return true;
		}

		public bool GoToLastDiff()
		{
			if (!this.CanGoToLastDiff)
			{
				return false;
			}

			int[] starts = this.lines.DiffStartLines;
			this.GoToPosition(starts[starts.Length - 1], this.position.Column);
			return true;
		}

		public bool GoToLine()
		{
			int maxLineNumber = 0;
			for (int i = this.lines.Count - 1; i >= 0; i--)
			{
				DiffViewLine line = this.lines[i];
				if (line.Number.HasValue)
				{
					// Add 1 because display numbers are 1-based.
					maxLineNumber = line.Number.Value + 1;
					break;
				}
			}

			if (maxLineNumber > 0)
			{
				using (GoToDialog dialog = new GoToDialog())
				{
					int line;
					if (dialog.Execute(this, maxLineNumber, out line))
					{
						// Subtract 1 because the dialog returns a 1-based number
						return this.GoToLine(line - 1);
					}
				}
			}

			return false;
		}

		public bool GoToLine(int line)
		{
			// We know the original line number will be in a DiffViewLine at a position >= iLine.
			if (line >= 0 && this.lines != null && line < this.lines.Count)
			{
				for (int i = line; i < this.lines.Count; i++)
				{
					DiffViewLine diffLine = this.lines[i];
					if (diffLine.Number.HasValue && diffLine.Number.Value == line)
					{
						this.GoToPosition(i, 0);
						return true;
					}
				}
			}

			return false;
		}

		public bool GoToNextDiff()
		{
			if (!this.CanGoToNextDiff)
			{
				return false;
			}

			int[] starts = this.lines.DiffStartLines;
			int numStarts = starts.Length;
			for (int i = 0; i < numStarts; i++)
			{
				if (this.position.Line < starts[i])
				{
					this.GoToPosition(starts[i], this.position.Column);
					return true;
				}
			}

			// We should never get here.
			Debug.Assert(false, "CanGoToNextDiff was wrong.");
			return false;
		}

		public bool GoToPreviousDiff()
		{
			if (!this.CanGoToPreviousDiff)
			{
				return false;
			}

			int[] ends = this.lines.DiffEndLines;
			int numEnds = ends.Length;
			for (int i = numEnds - 1; i >= 0; i--)
			{
				if (this.position.Line > ends[i])
				{
					// I'm intentionally setting the line to Starts[i] here instead of Ends[i].
					this.GoToPosition(this.lines.DiffStartLines[i], this.position.Column);
					return true;
				}
			}

			// We should never get here.
			Debug.Assert(false, "CanGoToPreviousDiff was wrong.");
			return false;
		}

		public void ScrollToCaret()
		{
			if (this.caret != null)
			{
				// Assume the caret is always at this.Position.
				// It would be nice if we had:
				// Debug.Assert(this.Position.Line == CaretPos.Line && this.Position.Column == CaretPos.Column);
				// but that fails on occasion because of rounding problems
				// between calling GetPointFromPos and then GetPosFromPoint.
				DiffViewPosition caretPos = this.position;
				Point caretPoint = this.GetPointFromPos(caretPos.Line, caretPos.Column);

				// Make sure that position is on the screen by
				// scrolling the minimal number of lines and characters.
				int firstVisibleLine = this.FirstVisibleLine;
				int lastVisibleLine = firstVisibleLine + this.VisibleLineCount - 1;

				if (caretPos.Line < firstVisibleLine)
				{
					this.VScrollPos -= firstVisibleLine - caretPos.Line;
				}
				else if (caretPos.Line > lastVisibleLine)
				{
					this.VScrollPos += caretPos.Line - lastVisibleLine;
				}

				// This is tricky because we might not have a monospaced font.
				// We have to figure out the number of pixels we need to scroll
				// and then translate that into characters (i.e. CharWidths).
				int firstVisibleX = this.gutterWidth - GutterSeparatorWidth;
				int lastVisibleX = this.ClientSize.Width - this.caret.Size.Width;
				if (caretPoint.X < firstVisibleX)
				{
					int scrollPixels = caretPoint.X - firstVisibleX;
					this.HScrollPos += (int)Math.Floor(scrollPixels / (double)this.charWidth);
				}
				else if (caretPoint.X > lastVisibleX)
				{
					int scrollPixels = caretPoint.X - lastVisibleX;
					this.HScrollPos += (int)Math.Ceiling(scrollPixels / (double)this.charWidth);
				}
			}
		}

		public void SelectAll()
		{
			this.SetPosition(0, 0, true, false);
			DiffViewPosition endPos = this.GetDocumentEndPosition();
			this.SetSelectionEnd(endPos.Line, endPos.Column, false);
		}

        /// <summary>
        /// Sets the Counterpart property in each line property of each
        /// <see cref="DiffViewModel"/> to refer to each other. This information
        /// can be used for finding equivelant from left to right lines[] collection
        /// and vice versa.
        /// </summary>
        /// <param name="counterpartView"></param>
		public void SetCounterpartLines(DiffView counterpartView)
		{
			int numLines = this.LineCount;
			if (numLines != counterpartView.LineCount)
			{
				throw new ArgumentException("The counterpart view has a different number of view lines.", nameof(counterpartView));
			}

			for (int i = 0; i < numLines; i++)
			{
				DiffViewLine line = this.lines[i];
				DiffViewLine counterpart = counterpartView.lines[i];

				// Make the counterpart lines refer to each other.
				line.Counterpart = counterpart;
				counterpart.Counterpart = line;
			}
		}

        /// <summary>
        /// Used to setup the ViewA/ViewB view that shows the left and right text views
        /// with the textual content and imaginary lines.
        /// each other.
        /// </summary>
        /// <param name="lineOne"></param>
        /// <param name="lineTwo"></param>
        public void SetData(IList<string> stringList, EditScript script, bool useA)
		{
			this.lines = new DiffViewLines(stringList, script, useA);
			this.UpdateAfterSetData();
		}

        /// <summary>
        /// Used to setup the ViewLineDiff view that shows only 2 lines over each other
        /// representing the currently active line from the left/right side views under
        /// each other.
        /// </summary>
        /// <param name="lineOne"></param>
        /// <param name="lineTwo"></param>
		public void SetData(DiffViewLine lineOne, DiffViewLine lineTwo)
		{
			this.lines = new DiffViewLines(lineOne, lineTwo);
			this.UpdateAfterSetData();
		}

		#endregion

		#region Internal Methods

		internal static int EnsureInRange(int min, int value, int max)
		{
			int result = Math.Max(min, Math.Min(value, max));
			return result;
		}

		#endregion

		#region Protected Methods

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.caret != null)
				{
					this.caret.Dispose();
				}

				DiffOptions.OptionsChanged -= this.DiffOptionsChanged;
			}

			base.Dispose(disposing);
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			// Select the identifier at the double-clicked position.
			DiffViewPosition pos = this.Position;
			int startColumn = this.FindCurrentTokenStart(pos, true);
			int endColumn = this.FindCurrentTokenEnd(pos, false);
			this.SetPosition(pos.Line, startColumn);
			if (endColumn != startColumn)
			{
				this.SetSelectionEnd(pos.Line, endColumn);
			}

			base.OnDoubleClick(e);
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			this.Invalidate();
		}

		protected override void OnFontChanged(EventArgs e)
		{
			// Do these first so event handlers can pull correct values.
			this.UpdateTextMetrics(true);
			this.SetupScrollBars();

			// Now, call the base handler and let it notify registered delegates.
			base.OnFontChanged(e);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);

			this.ReleaseCaret();
			this.caret = new Caret(this, CaretWidth, this.lineHeight);
			this.UpdateCaret();

			this.InvalidateSelection();
			this.InvalidateCaretGutter();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			bool ctrlShiftPressed = e.Modifiers == (Keys.Control | Keys.Shift);
			bool ctrlPressed = e.Modifiers == Keys.Control;
			bool shiftPressed = e.Modifiers == Keys.Shift;
			bool noModifierPressed = e.Modifiers == 0;

			if (ctrlShiftPressed || ctrlPressed || shiftPressed || noModifierPressed)
			{
				switch (e.KeyCode)
				{
					case Keys.A:
						if (ctrlPressed)
						{
							this.SelectAll();
						}

						break;

					case Keys.C:
						if (ctrlPressed && this.HasSelection)
						{
							Clipboard.SetDataObject(this.SelectedText, true);
						}

						break;

					case Keys.Up:
						this.HandleArrowUpDown(-1, ctrlPressed, shiftPressed, noModifierPressed);
						break;

					case Keys.Down:
						this.HandleArrowUpDown(+1, ctrlPressed, shiftPressed, noModifierPressed);
						break;

					case Keys.Left:
						this.HandleArrowLeft(ctrlShiftPressed, ctrlPressed, shiftPressed, noModifierPressed);
						break;

					case Keys.Right:
						this.HandleArrowRight(ctrlShiftPressed, ctrlPressed, shiftPressed, noModifierPressed);
						break;

					case Keys.PageUp:
						this.HandlePageUpDown(-1, shiftPressed, noModifierPressed);
						break;

					case Keys.PageDown:
						this.HandlePageUpDown(+1, shiftPressed, noModifierPressed);
						break;

					case Keys.Home:
						if (ctrlShiftPressed)
						{
							this.SetSelectionEnd(0, 0);
						}
						else if (ctrlPressed)
						{
							this.SetPosition(0, 0);
						}
						else if (shiftPressed)
						{
							this.ExtendSelection(0, -this.Position.Column);
						}
						else if (noModifierPressed)
						{
							this.SetPosition(this.Position.Line, 0);
						}

						break;

					case Keys.End:
						if (ctrlShiftPressed)
						{
							DiffViewPosition endPos = this.GetDocumentEndPosition();
							this.SetSelectionEnd(endPos.Line, endPos.Column);
						}
						else if (ctrlPressed)
						{
							DiffViewPosition endPos = this.GetDocumentEndPosition();
							this.SetPosition(endPos.Line, endPos.Column);
						}
						else if (shiftPressed)
						{
							this.ExtendSelection(0, this.GetDisplayLineLength(this.Position.Line) - this.Position.Column);
						}
						else if (noModifierPressed)
						{
							int line = this.Position.Line;
							this.SetPosition(line, this.GetDisplayLineLength(line));
						}

						break;
				}
			}
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);

			this.ReleaseCaret();

			this.InvalidateSelection();
			this.InvalidateCaretGutter();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (!this.Focused && this.CanFocus)
			{
				this.Focus();
			}

			if (e.X >= this.gutterWidth)
			{
				DiffViewPosition pos = this.GetPosFromPoint(e.X, e.Y);

				// Only change pos if non-right-click or right-click not in selection
				if (e.Button != MouseButtons.Right || !this.InSelection(pos))
				{
					this.SetPosition(pos.Line, pos.Column);
				}

				if (e.Button == MouseButtons.Left)
				{
					this.Capture = true;
					this.capturedMouse = true;
				}
			}

			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			// Update the mouse cursor to be an IBeam if we're not in the gutter.
			this.Cursor = (e.X < this.gutterWidth) ? Cursors.Default : Cursors.IBeam;

			if (this.capturedMouse && e.Button == MouseButtons.Left)
			{
				// Determine if we're at or above the first visible line
				// or at or below the last visible line.  If so, then
				// auto-scroll.  Similarly, if we're on the first or last
				// character or beyond, then auto-scroll.
				Rectangle r = new Rectangle(this.gutterWidth, 0, this.ClientSize.Width, this.ClientSize.Height);
				r.Inflate(-this.charWidth, -this.lineHeight);
				if (!r.Contains(e.X, e.Y))
				{
					this.verticalAutoScrollAmount = 0;
					if (e.Y < r.Y)
					{
						this.verticalAutoScrollAmount = -1;
					}
					else if (e.Y > r.Bottom)
					{
						this.verticalAutoScrollAmount = 1;
					}

					this.horizontalAutoScrollAmount = 0;
					if (e.X < r.X)
					{
						this.horizontalAutoScrollAmount = -1;
					}
					else if (e.X > r.Right)
					{
						this.horizontalAutoScrollAmount = 1;
					}

					this.autoScrollTimer.Enabled = true;
				}
				else
				{
					this.autoScrollTimer.Enabled = false;
				}

				// Set the selection end to the current mouse position
				// if the new position is different from the caret position.
				DiffViewPosition pos = this.GetPosFromPoint(e.X, e.Y);
				if (pos != this.position)
				{
					this.SetSelectionEnd(pos.Line, pos.Column);
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if (this.capturedMouse && e.Button == MouseButtons.Left)
			{
				this.Capture = false;
				this.capturedMouse = false;
				this.autoScrollTimer.Enabled = false;
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			this.wheelDelta += e.Delta;
			if (Math.Abs(this.wheelDelta) >= 120)
			{
				// I'm using "-=" here because Delta is reversed from what seems normal to me.
				// (e.g. wheel scrolling towards the user returns a negative value).
				this.VScrollPos -= SystemInformation.MouseWheelScrollLines * (this.wheelDelta / 120);
				this.wheelDelta = 0;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			// Get scroll positions
			int posY = this.VScrollPos;
			int posX = this.HScrollPos;

			// Find painting limits
			int numLines = this.LineCount;
			int firstLine = Math.Max(0, posY + (e.ClipRectangle.Top / this.lineHeight));
			int lastCalcLine = posY + (e.ClipRectangle.Bottom / this.lineHeight);
			int lastLine = Math.Min(numLines - 1, lastCalcLine);

			// Create some graphics objects
			Graphics g = e.Graphics;
			using (SolidBrush fontBrush = new SolidBrush(this.Enabled ? this.ForeColor : SystemColors.GrayText))
			using (SolidBrush backBrush = new SolidBrush(this.BackColor))
			{
				// We can't free GutterBrush since it is a system brush.
				Brush gutterBrush = SystemBrushes.Control;

				// Set the correct origin for HatchBrushes (used when painting dead space).
				g.RenderingOrigin = new Point(-posX, -posY);

				// See what we need to paint.  For horz scrolling,
				// the gutter won't need it.  For focus changes,
				// the lines won't need it.
				bool paintGutter = e.ClipRectangle.X < this.gutterWidth;
				bool paintLine = e.ClipRectangle.X + e.ClipRectangle.Width >= this.gutterWidth;
				bool hasFocus = this.Focused;

				// Indent the gutter text horizontally a little bit
				int lineNumIndent = this.charWidth / 2; // This centers it since it has 1 extra char width

				// Determine the selection positions in forward order
				bool hasSelection = this.HasSelection;
				DiffViewPosition startSel, endSel;
				this.GetForwardOrderSelection(out startSel, out endSel);

				// Paint each line
				for (int i = firstLine; i <= lastLine; i++)
				{
					// If we get inside this loop there must be at least one line.
					Debug.Assert(this.LineCount > 0, "There must be at least one line.");

					int x = (this.charWidth * (-posX)) + this.gutterWidth;
					int y = this.lineHeight * (i - posY);

					DiffViewLine line = this.lines[i];
					if (paintLine)
					{
						this.DrawLine(g, fontBrush, backBrush, hasFocus, i, line, x, y, hasSelection, startSel, endSel);
					}

					if (paintGutter)
					{
						this.DrawGutter(g, fontBrush, backBrush, hasFocus, i, line, y, gutterBrush, lineNumIndent);
					}
				}

				// Draw the background and an empty gutter for any
				// blank lines past the end of the actual lines.
				backBrush.Color = this.BackColor;
				for (int i = lastLine + 1; i <= lastCalcLine; i++)
				{
					int y = this.lineHeight * (i - posY);

					this.DrawBackground(g, backBrush, y, true);

					if (paintGutter)
					{
						this.DrawGutterBackground(g, gutterBrush, backBrush, y);
					}
				}
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			// We must setup the scroll bars before calling the base handler.
			// Attached event handlers like DiffOverview need to be able to pull
			// the correct FirstVisibleLine and VisibleLineCount properties.
			this.SetupScrollBars();

			// Now, call the base handler and let it notify registered delegates.
			base.OnSizeChanged(e);
		}

		protected override bool IsInputKey(Keys keyData)
		{
			// We have to do this so the arrow keys will go to our OnKeyDown method.
			// Alternately, we could handle WM_GETDLGCODE in WndProc and include
			// the DLGC_WANTARROWS flag, but this way is easier and fully managed.
			if (keyData.HasFlag(Keys.Left) ||
				keyData.HasFlag(Keys.Right) ||
				keyData.HasFlag(Keys.Up) ||
				keyData.HasFlag(Keys.Down))
			{
				return true;
			}

			return base.IsInputKey(keyData);
		}

		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			if (m.Msg == NativeMethods.WM_VSCROLL || m.Msg == NativeMethods.WM_HSCROLL)
			{
				bool horz = m.Msg == NativeMethods.WM_HSCROLL;

				NativeMethods.ScrollInfo info = NativeMethods.GetScrollInfo(this, horz);
				int newPos = info.nPos;
				int originalPos = newPos;

				// The SB_THUMBTRACK code is only in the lower word.
				ushort scrollCode = (ushort)((int)m.WParam & 0xFFFF);
				switch (scrollCode)
				{
					case NativeMethods.SB_TOP: // SB_LEFT
						newPos = info.nMin;
						break;

					case NativeMethods.SB_BOTTOM: // SB_RIGHT
						newPos = info.nMax;
						break;

					case NativeMethods.SB_LINEUP: // SB_LINELEFT;
						newPos--;
						break;

					case NativeMethods.SB_LINEDOWN: // SB_LINERIGHT
						newPos++;
						break;

					case NativeMethods.SB_PAGEUP: // SB_PAGELEFT
						newPos -= (int)info.nPage;
						break;

					case NativeMethods.SB_PAGEDOWN: // SB_PAGERIGHT
						newPos += (int)info.nPage;
						break;

					case NativeMethods.SB_THUMBTRACK:
						newPos = info.nTrackPos;
						break;
				}

				if (horz)
				{
					this.ScrollHorizontally(newPos, originalPos);
				}
				else
				{
					this.ScrollVertically(newPos, originalPos);
				}
			}
			else
			{
				base.WndProc(ref m);
			}
		}

		#endregion

		#region Private Methods

		private static bool MatchTokenCharacterType(char ch, ref TokenCharacterType charTypeToMatch)
		{
			TokenCharacterType charType = TokenCharacterType.Other;
			if (ch == '_' || char.IsLetterOrDigit(ch))
			{
				charType = TokenCharacterType.Identifier;
			}
			else if (char.IsWhiteSpace(ch))
			{
				charType = TokenCharacterType.Whitespace;
			}

			bool result = false;
			if (charTypeToMatch == TokenCharacterType.Unknown)
			{
				// On the first character, we'll only return false if the type was whitespace.
				charTypeToMatch = charType;
				result = charType != TokenCharacterType.Whitespace;
			}
			else
			{
				result = charType == charTypeToMatch;
			}

			return result;
		}

		private static int MeasureString(Graphics g, string displayText, int length, Font font)
		{
			// The caller should pass display text, so that tabs are expanded to spaces properly.
			string segment = displayText.Substring(0, length);
			Size result = TextRenderer.MeasureText(g, segment, font, MaxSize, DefaultTextFormat);
			return result.Width;
		}

		private void AutoScrollTimer_Tick(object sender, EventArgs e)
		{
			this.VScrollPos += this.verticalAutoScrollAmount;
			this.HScrollPos += this.horizontalAutoScrollAmount;

			// Set the selection end
			Point point = this.PointToClient(Control.MousePosition);
			DiffViewPosition pos = this.GetPosFromPoint(point.X, point.Y);
			this.SetSelectionEnd(pos.Line, pos.Column);
		}

		private void ClearSelection()
		{
			if (this.HasSelection)
			{
				this.InvalidateSelection();
				this.selectionStart = DiffViewPosition.Empty;
				this.FireSelectionChanged();
			}
		}

		private void DiffOptionsChanged(object sender, EventArgs e)
		{
			// The colors and/or tab width changed.
			this.UpdateTextMetrics(true);

			// If the tab width changed, we have to recalculate the scroll boundaries based on string lengths.
			this.SetupScrollBars();

			// Invalidating the whole window will take care of the color change.
			this.Invalidate();
		}

		private void DrawBackground(Graphics g, SolidBrush brush, int y, bool deadSpace)
		{
			if (deadSpace)
			{
				using (Brush deadBrush = DiffOptions.TryCreateDeadSpaceBrush(brush.Color))
				{
					// If hatching is turned off, then we have to fallback to the solid brush.
					g.FillRectangle(deadBrush ?? brush, this.gutterWidth, y, this.ClientSize.Width, this.lineHeight);
				}
			}
			else
			{
				g.FillRectangle(brush, this.gutterWidth, y, this.ClientSize.Width, this.lineHeight);
			}
		}

		private void DrawChangedLineBackground(
			Graphics g,
			DisplayLine displayLine,
			string displayText,
			EditScript changeEditScript,
			bool useA,
			int x,
			int y)
		{
			IList<Segment> segments = displayLine.GetChangeSegments(changeEditScript, useA);

			// If the change only inserted or only deleted chars, then one side will have changes but the other won't.
			if (segments.Count > 0)
			{
				// The main line background has already been drawn, so we just
				// need to draw the deleted or inserted background segments.
				Color changeColor = DiffOptions.GetColorForEditType(useA ? EditType.Delete : EditType.Insert);
				using (Brush changeBrush = new SolidBrush(changeColor))
				{
					Font font = this.Font;
					foreach (Segment segment in segments)
					{
						int segStartX = MeasureString(g, displayText, segment.Start, font);
						int segEndX = MeasureString(g, displayText, segment.Start + segment.Length, font);
						g.FillRectangle(changeBrush, x + segStartX, y, segEndX - segStartX, this.lineHeight);
					}
				}
			}
		}

		private void DrawGutter(
			Graphics g,
			SolidBrush fontBrush,
			SolidBrush backBrush,
			bool hasFocus,
			int lineNumber,
			DiffViewLine line,
			int y,
			Brush gutterBrush,
			int lineNumIndent)
		{
			// Draw the gutter background
			backBrush.Color = this.BackColor;

			Brush lineGutterBrush = gutterBrush;
			Brush lineFontBrush = fontBrush;

			if (lineNumber == this.position.Line && hasFocus)
			{
				lineGutterBrush = SystemBrushes.Highlight;
				lineFontBrush = SystemBrushes.HighlightText;
			}

			this.DrawGutterBackground(g, lineGutterBrush, backBrush, y);

			// Draw the line number (as 1-based)
			if (line.Number.HasValue)
			{
				this.DrawString(g, string.Format(this.gutterFormat, line.Number.Value + 1), lineFontBrush, lineNumIndent, y);
			}
		}

		private void DrawGutterBackground(Graphics g, Brush gutterBrush, Brush backBrush, int y)
		{
			int darkWidth = this.gutterWidth - GutterSeparatorWidth;
			g.FillRectangle(gutterBrush, 0, y, darkWidth, this.lineHeight);
			g.FillRectangle(backBrush, darkWidth, y, GutterSeparatorWidth, this.lineHeight);
			g.DrawLine(SystemPens.ControlDark, darkWidth - 1, y, darkWidth - 1, y + this.lineHeight);
		}

		private void DrawLine(
			Graphics g,
			SolidBrush fontBrush,
			SolidBrush backBrush,
			bool hasFocus,
			int lineNumber,
			DiffViewLine line,
			int x,
			int y,
			bool hasSelection,
			DiffViewPosition startSel,
			DiffViewPosition endSel)
		{
			DisplayLine displayLine = this.GetDisplayLine(line);
			string lineText = displayLine.GetDisplayText();

			// If any portion of the line is selected, we have to paint that too.
			int selStartColumn = 0, selEndColumn = 0, selStartX = 0, selEndX = 0;
			bool lineHasSelection = false;
			bool lineFullySelected = false;
			if (hasSelection && lineNumber >= startSel.Line && lineNumber <= endSel.Line)
			{
				int lineLength = lineText.Length;
				selStartColumn = (lineNumber == startSel.Line) ? startSel.Column : 0;
				selEndColumn = (lineNumber == endSel.Line) ? Math.Min(endSel.Column, lineLength) : lineLength;

				lineHasSelection = true;
				lineFullySelected = (lineNumber > startSel.Line && lineNumber < endSel.Line) || (selStartColumn == 0 && selEndColumn == lineLength);

				selStartX = this.GetXForColumn(g, displayLine, lineText, selStartColumn);
				selEndX = this.GetXForColumn(g, displayLine, lineText, selEndColumn);
			}

			// Draw the background.  Even if the line is completely selected,
			// we want to do this because after the last char, we don't paint
			// with the selection color.  So it needs to be the normal back color.
			backBrush.Color = line.Edited ? DiffOptions.GetColorForEditType(line.EditType) : this.BackColor;
			this.DrawBackground(g, backBrush, y, !line.Number.HasValue);

			// Draw the line text if any portion of it is unselected.
			if (!lineFullySelected)
			{
				// The DiffViewLine will cache the edit script, but I'm intentionally not
				// pulling it until we have to have it for rendering.  Getting intra-line
				// diffs makes the whole process into an O(n^2) operation instead of
				// just an O(n) operation for line-by-line diffs.  So I'm try to defer the
				// extra work until the user requests to see the changed line.  It's still
				// the same amount of work if they view every line, but it makes the
				// user interface more responsive to split it up like this.
				EditScript changeEditScript = line.GetChangeEditScript(this.ChangeDiffOptions);
				if (changeEditScript != null)
				{
					this.DrawChangedLineBackground(g, displayLine, lineText, changeEditScript, line.FromA, x, y);
				}

				this.DrawString(g, lineText, fontBrush, x, y);
			}

			// Draw the selection
			if (lineHasSelection)
			{
				// Draw the background
				RectangleF r = new RectangleF(selStartX, y, selEndX - selStartX, this.lineHeight);
				Brush brush = hasFocus ? SystemBrushes.Highlight : SystemBrushes.Control;
				g.FillRectangle(brush, r);

				// Draw the selected text.  This draws the string from the original X, but it
				// changes the clipping region so that only the portion inside the highlighted
				// rectangle will paint with the selected text color.
				Region originalClipRegion = g.Clip;
				using (Region textClip = new Region(r))
				{
					g.Clip = textClip;
					brush = hasFocus ? SystemBrushes.HighlightText : SystemBrushes.WindowText;
					this.DrawString(g, lineText, brush, x, y);
					g.Clip = originalClipRegion;
				}
			}
		}

		private void DrawString(Graphics g, string displayText, Brush brush, int x, int y)
		{
			// The caller should pass display text, so that tabs are expanded to spaces properly.
			//
			// DrawText has some type of limit around 32K pixels where it won't draw and won't raise an error.
			// When using Consolas 10pt, I've only seen this problem with lines over 4800 characters long.
			// So I'll check for this if the text is "long" and then manually draw the text in two smaller parts.
			// I'll set my "long" length to a fraction of 4800, so we won't do extra measuring very often.
			// Note: This could still have a problem if someone uses "long" lines with an 80pt font, but that
			// should be extremely rare.
			const int LongLineLength = 600;
			const int GdiWidthLimit = 32000;
			int displayTextLength = displayText.Length;
			int measuredWidth;
			if (displayTextLength < LongLineLength || (measuredWidth = MeasureString(g, displayText, displayTextLength, this.Font)) < GdiWidthLimit)
			{
				TextRenderer.DrawText(g, displayText, this.Font, new Point(x, y), ((SolidBrush)brush).Color, DefaultTextFormat);
			}
			else
			{
				int splitIndex = displayTextLength / 2;
				string leftText = displayText.Substring(0, splitIndex);
				string rightText = displayText.Substring(splitIndex);
				this.DrawString(g, leftText, brush, x, y);
				measuredWidth = MeasureString(g, leftText, leftText.Length, this.Font);
				this.DrawString(g, rightText, brush, x + measuredWidth, y);
			}
		}

		private void ExtendSelection(int lines, int columns)
		{
			int line = this.position.Line + lines;
			int column = this.position.Column + columns;
			this.SetSelectionEnd(line, column);
		}

		private int FindCurrentTokenEnd(DiffViewPosition start, bool includeTrailingWhitespace)
		{
			DisplayLine displayLine = this.GetDisplayLine(start.Line);
			int startTextIndex = displayLine.GetTextIndexFromDisplayColumn(start.Column);
			string lineText = displayLine.OriginalText;
			int lineTextLength = lineText.Length;

			int endTextIndex = startTextIndex;
			TokenCharacterType charTypeToMatch = TokenCharacterType.Unknown;
			while (endTextIndex < lineTextLength)
			{
				char ch = lineText[endTextIndex];
				if (!MatchTokenCharacterType(ch, ref charTypeToMatch))
				{
					break;
				}

				endTextIndex++;
			}

			if (!includeTrailingWhitespace && endTextIndex == startTextIndex)
			{
				// If we didn't move, then we should skip over any upcoming whitespace
				// rather than just skipping one whitespace character with the logic below.
				includeTrailingWhitespace = true;
			}

			if (includeTrailingWhitespace)
			{
				while (endTextIndex < lineTextLength && char.IsWhiteSpace(lineText[endTextIndex]))
				{
					endTextIndex++;
				}
			}

			if (endTextIndex == startTextIndex)
			{
				// Make sure we move forward at least one character.
				endTextIndex++;
			}

			int result = displayLine.GetDisplayColumnFromTextIndex(endTextIndex);
			return result;
		}

		private int FindCurrentTokenStart(DiffViewPosition start, bool allowStartAsResult)
		{
			DisplayLine displayLine = this.GetDisplayLine(start.Line);
			int originalStartTextIndex = displayLine.GetTextIndexFromDisplayColumn(start.Column);
			string lineText = displayLine.OriginalText;
			int lineTextLength = lineText.Length;

			int startTextIndex = originalStartTextIndex;
			if (lineTextLength > 0)
			{
				if (!allowStartAsResult)
				{
					// Skip any "leading" whitespace (but we're going in reverse, so it's actually trailing).
					while (startTextIndex > 0 && startTextIndex < lineTextLength && char.IsWhiteSpace(lineText[startTextIndex - 1]))
					{
						startTextIndex--;
					}
				}

				TokenCharacterType charTypeToMatch = TokenCharacterType.Unknown;
				while (startTextIndex > 0)
				{
					// Look at the previous character.
					char prevChar = lineText[startTextIndex - 1];
					if (!MatchTokenCharacterType(prevChar, ref charTypeToMatch))
					{
						break;
					}

					startTextIndex--;
				}
			}

			if (startTextIndex == originalStartTextIndex && !allowStartAsResult)
			{
				// Make sure we move back at least one character.
				startTextIndex--;
			}

			int result = displayLine.GetDisplayColumnFromTextIndex(startTextIndex);
			return result;
		}

		private void FireSelectionChanged()
		{
			if (this.SelectionChanged != null)
			{
				this.SelectionChanged(this, EventArgs.Empty);
			}
		}

		private DisplayLine GetDisplayLine(int line)
		{
			DisplayLine result;
			if (line >= 0 && line < this.LineCount)
			{
				result = this.GetDisplayLine(this.lines[line]);
			}
			else
			{
				result = this.GetDisplayLine(DiffViewLine.Empty);
			}

			return result;
		}

		private DisplayLine GetDisplayLine(DiffViewLine line)
		{
			DisplayLine result = new DisplayLine(line, this.showWhitespace, DiffOptions.SpacesPerTab);
			return result;
		}

		private int GetDisplayLineLength(int line)
		{
			DisplayLine displayLine = this.GetDisplayLine(line);
			int result = displayLine.GetDisplayTextLength();
			return result;
		}

		private DiffViewPosition GetDocumentEndPosition()
		{
			int line = Math.Max(this.LineCount - 1, 0);
			int column = this.GetDisplayLineLength(line);
			return new DiffViewPosition(line, column);
		}

		private DiffViewPosition GetFindStartPosition(bool searchUp)
		{
			DiffViewPosition result = this.position;

			if (this.HasSelection)
			{
				DiffViewPosition startSel, endSel;
				this.GetForwardOrderSelection(out startSel, out endSel);

				if (searchUp)
				{
					result = startSel;
				}
				else
				{
					result = endSel;
				}
			}

			return result;
		}

		private void GetForwardOrderSelection(out DiffViewPosition startSel, out DiffViewPosition endSel)
		{
			// Determine the selection positions in forward order.
			// Get them in order in case we have a reverse selection.
			startSel = this.selectionStart;
			endSel = this.position;
			if (startSel.Line > endSel.Line || (startSel.Line == endSel.Line && startSel.Column > endSel.Column))
			{
				startSel = this.position;
				endSel = this.selectionStart;
			}
		}

		private bool GetSingleLineSelectedText(out string text)
		{
			if (this.HasSelection)
			{
				DiffViewPosition startSel, endSel;
				this.GetForwardOrderSelection(out startSel, out endSel);
				if (startSel.Line == endSel.Line && endSel.Column > startSel.Column)
				{
					DisplayLine displayLine = this.GetDisplayLine(startSel.Line);
					text = displayLine.GetTextBetweenDisplayColumns(startSel.Column, endSel.Column);
					return true;
				}
			}

			text = null;
			return false;
		}

		private int GetXForColumn(Graphics g, DisplayLine displayLine, string displayText, int column)
		{
			if (displayText == null)
			{
				displayText = displayLine.GetDisplayText();
			}

			int length = Math.Max(0, Math.Min(displayText.Length, column));

			// Make sure the column's X position is always shifted left to the
			// start of the character that contains/crosses the column.
			length = displayLine.GetColumnStart(length);

			int x = MeasureString(g, displayText, length, this.Font);
			int result = x - (this.HScrollPos * this.charWidth) + this.gutterWidth;
			return result;
		}

		private void GoToPosition(int line, int column)
		{
			// If requested line is on the screen and not in the last 3 lines, then go to it without re-centering.
			// I picked 3 because that usually gets a reasonable amount of context information on the screen
			// for most differences.
			int firstVisibleLine = this.FirstVisibleLine;
			int lastVisibleLine = firstVisibleLine + this.VisibleLineCount - 1;
			if (line < firstVisibleLine || line > (lastVisibleLine - 3))
			{
				this.CenterVisibleLine = line;
			}

			this.SetPosition(line, column);
		}

		private bool InSelection(DiffViewPosition pos)
		{
			bool result = false;

			if (this.HasSelection)
			{
				DiffViewPosition startSel, endSel;
				this.GetForwardOrderSelection(out startSel, out endSel);
				result = pos >= startSel && pos <= endSel;
			}

			return result;
		}

		private void InvalidateCaretGutter()
		{
			// Invalidate the gutter portion for the line with the caret.
			Point point = this.GetPointFromPos(this.position.Line, 0);
			Rectangle r = new Rectangle(0, point.Y, this.gutterWidth, this.lineHeight);
			this.Invalidate(r);
		}

		private void InvalidateSelection()
		{
			if (this.HasSelection)
			{
				int firstLine = Math.Min(this.selectionStart.Line, this.position.Line);
				Point point = this.GetPointFromPos(firstLine, 0);
				int numLines = Math.Abs(this.selectionStart.Line - this.position.Line) + 1;
				Rectangle r = new Rectangle(this.gutterWidth, point.Y, this.ClientSize.Width, numLines * this.lineHeight);
				this.Invalidate(r);
			}
		}

		private void OffsetPosition(int lines, int columns)
		{
			int line = this.position.Line + lines;
			int column = this.position.Column + columns;
			this.SetPosition(line, column);
		}

		private void ReleaseCaret()
		{
			// Sometimes in the debugger, the IDE seems to steal focus, and the OnGot/LostFocus event can
			// fire twice without us receiving the matching OnLost/GotFocus call.  So we have to protect against
			// that here and check to see if we still have the caret.  We'll clean it up and recreate it when needed
			// because that seems safer than trying to reuse an existing caret if it's left over from a weird
			// intermediate state that shouldn't be happening normally.
			if (this.caret != null)
			{
				this.caret.Dispose();
				this.caret = null;
			}
		}

		private void ScrollHorizontally(int newPos, int originalPos)
		{
			int pos = this.UpdateScrollPos(newPos, true);
			if (pos != originalPos)
			{
				// Don't scroll the line number gutter
				Rectangle r = this.ClientRectangle;
				r.Offset(this.gutterWidth, 0);

				// This really seems like it should be necessary,
				// but if I do it then a GutterWidth sized band
				// is skipped on the scrolling/invalidated end...
				// r.Width -= GutterWidth;
				int numPixels = this.charWidth * (originalPos - pos);
				if (Math.Abs(numPixels) < this.ClientSize.Width)
				{
					// Scroll the subset of the window in the clipping region.
					//
					// Note: We must scroll by the integral this.CharWidth.  Otherwise,
					// round off causes pixel columns to occasionally get dropped
					// or duplicated.  This makes for ugly text until the next full
					// repaint.  By always using the same integral this.CharWidth, the
					// text scrolls smoothly and correctly.
					//
					// To make this smooth and correct, we also set the scroll bar
					// Page size and calculate X in OnPaint using the integral
					// this.CharWidth.
					NativeMethods.ScrollWindow(this, numPixels, 0, ref r, ref r);
				}
				else
				{
					this.Invalidate(r);
				}

				// ScrollWindow is supposed to update the caret position too,
				// but it doesn't when a rect and clipping rect are specified.
				// So we have to update it manually.  This is also necessary
				// because we don't ever want the caret to display in the gutter.
				this.UpdateCaret();

				if (this.HScrollPosChanged != null)
				{
					this.HScrollPosChanged(this, EventArgs.Empty);
				}
			}
		}

		private void ScrollVertically(int newPos, int originalPos)
		{
			int pos = this.UpdateScrollPos(newPos, false);
			if (pos != originalPos)
			{
				int numPixels = this.lineHeight * (originalPos - pos);
				if (numPixels < this.ClientSize.Height)
				{
					NativeMethods.ScrollWindow(this, 0, numPixels);
				}
				else
				{
					this.Invalidate();

					// We have to manually update the caret if we don't call ScrollWindow.
					this.UpdateCaret();
				}

				if (this.VScrollPosChanged != null)
				{
					this.VScrollPosChanged(this, EventArgs.Empty);
				}
			}
		}

		private void SetPosition(int line, int column)
		{
			this.SetPosition(line, column, true, true);
		}

		private void SetPosition(int line, int column, bool clearSelection, bool scrollToCaret)
		{
			line = EnsureInRange(0, line, this.LineCount - 1);

			DisplayLine displayLine = this.GetDisplayLine(line);
			int length = displayLine.GetDisplayTextLength();
			column = EnsureInRange(0, column, length);

			// Align the column to the start of the character it overlaps (in case it's inside a tab).
			column = displayLine.GetColumnStart(column);

			if (clearSelection)
			{
				this.ClearSelection();
			}

			bool lineNumberChanged = this.position.Line != line;
			bool columnNumberChanged = this.position.Column != column;

			if (lineNumberChanged || columnNumberChanged)
			{
				// Invalidate the old gutter line.
				if (lineNumberChanged)
				{
					this.InvalidateCaretGutter();
				}

				this.position = new DiffViewPosition(line, column);

				// Invalidate the new gutter line.
				if (lineNumberChanged)
				{
					this.InvalidateCaretGutter();
				}

				this.UpdateCaret();

				// If the selection range is now empty, then clear the selection.
				if (this.position == this.selectionStart)
				{
					this.ClearSelection();

					// Set the flag so we don't refire the SelectionChanged event below.
					clearSelection = true;
				}

				if (this.lines != null)
				{
					if (this.PositionChanged != null)
					{
						this.PositionChanged(this, EventArgs.Empty);
					}

					// If we cleared the selection earlier, then that
					// fire a SelectionChanged event.  If not, then we
					// need to fire it now because we've changed the
					// selection end point.
					if (!clearSelection)
					{
						this.FireSelectionChanged();
					}
				}
			}

			if (scrollToCaret)
			{
				this.ScrollToCaret();
			}
		}

		private void SetSelectionEnd(int line, int column, bool scrollToCaret = true)
		{
			bool selectionChanged = false;
			if (!this.HasSelection)
			{
				this.selectionStart = this.position;
				selectionChanged = true;
			}

			// Move the Position but keep the selection start
			int originalLine = this.position.Line;
			this.SetPosition(line, column, false, scrollToCaret);
			int numLines = Math.Abs(line - originalLine);

			// Invalidate new selection
			int firstLine = Math.Min(originalLine, line);
			Point point = this.GetPointFromPos(firstLine, 0);
			Rectangle r = new Rectangle(this.gutterWidth, point.Y, this.ClientSize.Width, (numLines + 1) * this.lineHeight);
			this.Invalidate(r);

			if (selectionChanged)
			{
				this.FireSelectionChanged();
			}
		}

		private void SetupScrollBars()
		{
			// Vertical - Scroll by lines
			int page = this.ClientSize.Height / this.lineHeight;
			int max = this.lines != null ? this.lines.Count - 1 : 0;
			NativeMethods.SetScrollPageAndRange(this, false, 0, max, page);

			// Horizontal - Scroll by characters
			page = this.ClientSize.Width / this.charWidth;
			max = 0;
			if (this.lines != null)
			{
				foreach (DiffViewLine line in this.lines)
				{
					int length = this.GetDisplayLine(line).GetDisplayTextLength();
					if (length > max)
					{
						max = length;
					}
				}
			}

			// We must include enough characters for the gutter line numbers and the separator.
			max += this.gutterWidth / this.charWidth;
			NativeMethods.SetScrollPageAndRange(this, true, 0, max, page);
		}

		private void UpdateAfterSetData()
		{
			// Reset the position before we start calculating things
			this.position = new DiffViewPosition(0, 0);
			this.selectionStart = DiffViewPosition.Empty;

			// We have to call this to recalc the gutter width
			this.UpdateTextMetrics(false);

			// We have to call this to setup the scroll bars
			this.SetupScrollBars();

			// Reset the scroll position
			this.VScrollPos = 0;
			this.HScrollPos = 0;

			// Update the caret
			this.UpdateCaret();

			// Force a repaint
			this.Invalidate();

			// Fire the LinesChanged event
			if (this.LinesChanged != null)
			{
				this.LinesChanged(this, EventArgs.Empty);
			}

			// Fire the position changed event
			if (this.PositionChanged != null)
			{
				this.PositionChanged(this, EventArgs.Empty);
			}

			this.FireSelectionChanged();
		}

		private void UpdateCaret()
		{
			if (this.lines != null && this.caret != null)
			{
				Point newPt = this.GetPointFromPos(this.position.Line, this.position.Column);
				this.caret.Visible = newPt.X >= (this.gutterWidth - GutterSeparatorWidth);
				this.caret.Position = newPt;
			}
		}

		private int UpdateScrollPos(int newPos, bool horz)
		{
			// Set the position and then retrieve it.  Due to adjustments by Windows
			// (e.g. if Pos is > Max) it may not be the same as the value set.
			NativeMethods.SetScrollPos(this, horz, newPos);
			return NativeMethods.GetScrollPos(this, horz);
		}

		private void UpdateTextMetrics(bool fontOrTabsChanged)
		{
			if (fontOrTabsChanged)
			{
				// Get the pixel width that a space should be.
				float dpi;
				using (Graphics g = Graphics.FromHwnd(this.Handle))
				{
					// See KBase article Q125681 for what I'm doing here to get the average character width.
					const string AvgCharWidthText = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
					this.charWidth = MeasureString(g, AvgCharWidthText, AvgCharWidthText.Length, this.Font) / AvgCharWidthText.Length;

					// Get the average pixels per inch
					dpi = (g.DpiX + g.DpiY) / 2;
				}

				// Get the line height in pixels
				FontFamily family = this.Font.FontFamily;
				int lineSpacingDesignUnits = family.GetLineSpacing(this.Font.Style);
				int fontHeightDesignUnits = family.GetEmHeight(this.Font.Style);
				float fontPoints = this.Font.Size;
				float fontPixels = fontPoints * dpi / 72;
				this.lineHeight = (int)Math.Ceiling((fontPixels * lineSpacingDesignUnits) / fontHeightDesignUnits);

				// This height still isn't "enough" (i.e. it still doesn't match
				// what the GetTextMetrics API would return as TEXTMETRICS.Height
				// + TEXTMETRICS.ExternalLeading.  It seems to be one pixel too
				// short, so I'll just add it back.
				this.lineHeight++;
			}

			// Set the gutter width to the this.CharWidth times the
			// number of characters we'll need to display.  Then
			// add another character for padding, another
			// pixel so we can have a separator line, and then
			// a small separator window-colored area.
			int maxLineNumChars = 1;
			if (this.LineCount > 0)
			{
				// Get the largest number.  Add 1 to it because we will
				// when we display it.  This is important when the number
				// is 9, but will be displayed as 10, etc.
				//
				// Also, we want to take the max of MaxLineNumber and 1 to
				// ensure that we never take the Log of 1 or less.  Negatives
				// and 0 don't have Logs, and Log(1) returns 0.  We always
				// want to end up with at least one for maxLineNumChars.
				int maxLineNumber = Math.Max(this.lines.MaxLineNumber, 1) + 1;

				// If the number of lines is NNNN (e.g. 1234), we need to get 4.
				// Add 1 and take the floor so that 10, 100, 1000, etc. will work
				// correctly.
				maxLineNumChars = (int)Math.Floor(Math.Log10(maxLineNumber) + 1);
			}

			this.gutterWidth = (this.charWidth * (maxLineNumChars + 1)) + 1 + GutterSeparatorWidth;

			// Build the gutter format string
			StringBuilder sb = new StringBuilder(20);
			sb.Append("{0:");
			sb.Append('0', maxLineNumChars);
			sb.Append("}");
			this.gutterFormat = sb.ToString();

			// Update the caret position (Gutter width or Font changes affect it)
			this.UpdateCaret();
		}

		private void HandleArrowUpDown(int lines, bool ctrlPressed, bool shiftPressed, bool noModifierPressed)
		{
			if (ctrlPressed)
			{
				this.VScrollPos += lines;
				this.OffsetPosition(lines, 0);
			}
			else if (shiftPressed)
			{
				this.ExtendSelection(lines, 0);
			}
			else if (noModifierPressed)
			{
				this.OffsetPosition(lines, 0);
			}
		}

		private void HandleArrowLeft(bool ctrlShiftPressed, bool ctrlPressed, bool shiftPressed, bool noModifierPressed)
		{
			DiffViewPosition pos = this.Position;
			if (pos.Line > 0 && pos.Column <= 0)
			{
				// We're at the beginning of a line other than the first line,
				// so we need to move back to the end of the previous line.
				int prevLine = pos.Line - 1;
				DisplayLine prevDisplayLine = this.GetDisplayLine(prevLine);
				int prevLineLength = prevDisplayLine.GetDisplayTextLength();
				if (shiftPressed || ctrlShiftPressed)
				{
					this.SetSelectionEnd(prevLine, prevLineLength);
				}
				else if (noModifierPressed || ctrlPressed)
				{
					this.SetPosition(prevLine, prevLineLength);
				}
			}
			else
			{
				if (ctrlShiftPressed)
				{
					int startColumn = this.FindCurrentTokenStart(pos, false);
					this.SetSelectionEnd(pos.Line, startColumn);
				}
				else if (ctrlPressed)
				{
					int startColumn = this.FindCurrentTokenStart(pos, false);
					this.SetPosition(pos.Line, startColumn);
				}
				else if (shiftPressed)
				{
					this.ExtendSelection(0, -1);
				}
				else if (noModifierPressed)
				{
					this.OffsetPosition(0, -1);
				}
			}
		}

		private void HandleArrowRight(bool ctrlShiftPressed, bool ctrlPressed, bool shiftPressed, bool noModifierPressed)
		{
			DiffViewPosition pos = this.Position;
			DisplayLine displayLine = this.GetDisplayLine(pos.Line);
			int lineLength = displayLine.GetDisplayTextLength();
			if (pos.Line < (this.LineCount - 1) && pos.Column >= lineLength)
			{
				// We're at the end of a line other than the last line,
				// so we need to move to the beginning of the next line.
				if (shiftPressed || ctrlShiftPressed)
				{
					this.SetSelectionEnd(pos.Line + 1, 0);
				}
				else if (noModifierPressed || ctrlPressed)
				{
					this.SetPosition(pos.Line + 1, 0);
				}
			}
			else
			{
				if (ctrlShiftPressed)
				{
					int endColumn = this.FindCurrentTokenEnd(pos, false);
					this.SetSelectionEnd(pos.Line, endColumn);
				}
				else if (ctrlPressed)
				{
					int endColumn = this.FindCurrentTokenEnd(pos, true);
					this.SetPosition(pos.Line, endColumn);
				}
				else
				{
					// Tab characters can be 1 to N columns wide, so we need
					// to find out how many columns over we have to go.
					int offset = displayLine.GetColumnWidth(pos.Column);
					if (shiftPressed)
					{
						this.ExtendSelection(0, offset);
					}
					else if (noModifierPressed)
					{
						this.OffsetPosition(0, offset);
					}
				}
			}
		}

		private void HandlePageUpDown(int sign, bool shiftPressed, bool noModifierPressed)
		{
			int page = NativeMethods.GetScrollPage(this, false);
			if (shiftPressed)
			{
				this.ExtendSelection(sign * page, 0);
			}
			else if (noModifierPressed)
			{
				this.VScrollPos += sign * page;
				this.OffsetPosition(sign * page, 0);
			}
		}

		#endregion
	}
}
