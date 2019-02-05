namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Windows.Forms;

	#endregion

	internal static class TextBoxUtility
	{
		#region Internal Methods

		internal static bool CanPasteText(TextBoxBase textBox)
		{
			bool result = !textBox.ReadOnly && WindowsUtility.ClipboardContainsText;
			return result;
		}

		internal static Point GetCaretPoint(TextBoxBase textBox)
		{
			// Get offset of current line.
			int lineIndex = GetLineIndex(textBox, -1);

			// Get Caret offset from first char
			int minIndex = 0, maxIndex = 0;
			const int EM_GETSEL = 0x00B0;
			NativeMethods.SendMessage(textBox, EM_GETSEL, ref minIndex, ref maxIndex);

			// Check for multiline selection with the caret at the end.
			// Note: For a single line selection, there is no way to tell
			// if the caret is at the start or end of the selected text.
			// We'll always return the start because several routines
			// use Pt.x to get the selection start relative to the current line.
			int caretIndex = 0;
			if (minIndex >= lineIndex)
			{
				caretIndex = minIndex;
			}
			else
			{
				caretIndex = maxIndex;
			}

			Point result = default(Point);

			// Get Caret Column
			result.X = caretIndex - lineIndex;

			// Get Caret Row
			result.Y = GetLineFromChar(textBox, caretIndex);

			// Validity check
			// Note: This validity check is necessary because of a funny
			// situation where the cursor is on a word wrapped line and
			// you press VK_END.  The cursor moves to the position PAST
			// the wrapping space.  On the first line, we'd have something
			// like: LineIndex = 0, CaretIndex = 66, NextLineIndex = 66.
			// In other words, EM_LINEINDEX thinks we're on line 1, but
			// EM_LINEFROMCHAR will say we're on line 2!
			// However, if you put the cursor at the beginning of the next
			// line, we'll have something like: LineIndex = 66,
			// CaretIndex = 66, NextLineIndex = 66.  That's why all three
			// indexes have to be in the validity check.
			int nextLineIndex = GetLineIndex(textBox, result.Y);
			if (lineIndex != caretIndex && caretIndex == nextLineIndex)
			{
				result.Y--;
			}

			return result;
		}

		internal static void SetCaretPoint(TextBoxBase textBox, Point point)
		{
			int index = GetLineIndex(textBox, point.Y);
			if (index != -1)
			{
				textBox.SelectionStart = index + point.X;
				textBox.SelectionLength = 0;
			}
		}

		internal static int GetFirstVisibleLine(TextBoxBase textBox)
		{
			const int EM_GETFIRSTVISIBLELINE = 0x00CE;
			int result = NativeMethods.SendMessage(textBox, EM_GETFIRSTVISIBLELINE, 0);
			return result;
		}

		internal static int GetLineCount(TextBoxBase textBox)
		{
			const int EM_GETLINECOUNT = 0x00BA;
			int result = NativeMethods.SendMessage(textBox, EM_GETLINECOUNT, 0);
			return result;
		}

		internal static void SetTabWidth(TextBoxBase textBox, int tabSpaces, double pixelsPerDialogUnit)
		{
			int numTabs = 0;
			int tabDialogUnits = 0;

			// Note: This only works well for fixed width fonts.  It works
			// ok for some non-fixed fonts like "Comic Sans MS", but it
			// doesn't work for "less monospaced" fonts like "Garamond" or
			// "Georgia".
			//
			// As far as I can tell, this is because I have to return an
			// integral number of dialog units.  I can calculate the correct
			// number of tab pixels for any font, but when I return the
			// integral DLUs for the tab size, it doesn't multiply back to
			// the correct number of pixels (except on monospaced fonts).
			// If only the stupid TextBox would let us pass in pixels instead
			// of dialog units!

			// If tabSpaces is 0 we'll just set the tab stops to the
			// TextBox default of 32 DLUs.
			if (tabSpaces > 0)
			{
				// Get the pixel width that a space should be.
				double spacePixels;
				using (Graphics graphics = Graphics.FromHwnd(textBox.Handle))
				{
					const TextFormatFlags Flags = TextFormatFlags.TextBoxControl | TextFormatFlags.NoPadding;
					spacePixels = TextRenderer.MeasureText(graphics, " ", textBox.Font, new Size(int.MaxValue, int.MaxValue), Flags).Width;
				}

				// Convert the pixels to "dialog units"
				int spaceDialogUnits = (int)Math.Ceiling(spacePixels / pixelsPerDialogUnit);
				tabDialogUnits = tabSpaces * spaceDialogUnits;
				numTabs = 1;
			}

			// LPARAM has to contain a pointer to the new width,
			// so we have to pass that parameter by reference.
			const int EM_SETTABSTOPS = 0x00CB;
			NativeMethods.SendMessage(textBox, EM_SETTABSTOPS, numTabs, ref tabDialogUnits);
			textBox.Invalidate();
		}

		internal static double CalcPixelsPerDialogUnit(TextBoxBase textBox)
		{
			double avgCurrentFontWidthPixels;

			// Get the pixel width that a space should be.
			using (Graphics graphics = Graphics.FromHwnd(textBox.Handle))
			{
				// See KBase article Q125681 for what I'm doing here to get the average character width.
				const TextFormatFlags Flags = TextFormatFlags.TextBoxControl | TextFormatFlags.NoPadding;
				const string UpperAndLowerAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
				avgCurrentFontWidthPixels = TextRenderer.MeasureText(
					graphics,
					UpperAndLowerAlphabet,
					textBox.Font,
					new Size(int.MaxValue, int.MaxValue),
					Flags).Width / (double)UpperAndLowerAlphabet.Length;
			}

			// Convert the pixels to "dialog units"
			int sysBaseUnits = NativeMethods.GetDialogBaseUnits();
			const int LowWordMask = 0xFFFF;
			int avgSystemFontWidthPixels = sysBaseUnits & LowWordMask; // Get the low-order word

			double result = 2 * avgCurrentFontWidthPixels / avgSystemFontWidthPixels;
			return result;
		}

		internal static double HandleFontChanged(TextBoxBase textBox, int tabSpaces)
		{
			// Update the PixelsPerDialogUnit
			double result = CalcPixelsPerDialogUnit(textBox);

			// Since the tab width is based on the current font,
			// we have to update it when the font changes.
			SetTabWidth(textBox, tabSpaces, result);

			return result;
		}

		internal static bool GotoLine(TextBoxBase textBox, int lineNumber, bool selectLine)
		{
			bool result = false;

			int index = GetLineIndex(textBox, lineNumber);
			if (index != -1)
			{
				textBox.SelectionStart = index;
				if (selectLine)
				{
					textBox.SelectionLength = GetLineLength(textBox, index);
				}
				else
				{
					textBox.SelectionLength = 0;
				}

				result = true;
			}

			return result;
		}

		internal static int GetLineIndex(TextBoxBase textBox, int lineNumber)
		{
			const int EM_LINEINDEX = 0x00BB;
			int result = NativeMethods.SendMessage(textBox, EM_LINEINDEX, lineNumber);
			return result;
		}

		internal static int GetLineLength(TextBoxBase textBox, int lineNumber)
		{
			const int EM_LINELENGTH = 0x00C1;
			int result = NativeMethods.SendMessage(textBox, EM_LINELENGTH, lineNumber);
			return result;
		}

		internal static int GetLineFromChar(TextBoxBase textBox, int characterIndex)
		{
			const int EM_LINEFROMCHAR = 0x00C9;
			int result = NativeMethods.SendMessage(textBox, EM_LINEFROMCHAR, characterIndex);
			return result;
		}

		internal static bool Scroll(TextBoxBase textBox, int horizontalChars, int verticalLines)
		{
			const int EM_LINESCROLL = 0x00B6;
			bool result = NativeMethods.SendMessage(textBox, EM_LINESCROLL, horizontalChars, verticalLines) != 0;
			return result;
		}

		#endregion
	}
}
