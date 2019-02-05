namespace Menees.Diffs.Controls
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Drawing;
	using System.Windows.Forms;
    using DiffLib.Text;
    using Menees.Windows.Forms;

	public sealed partial class DiffControl : ExtendedUserControl, IDisposable
	{
		#region Private Data Members

		private bool showColorLegend = true;
		private bool showToolBar = true;
		private FindData findData = new FindData();
		private int currentDiffLine = -1;

		#endregion

		#region Constructors

		public DiffControl()
		{
			this.InitializeComponent();

			this.UpdateButtons();
			this.UpdateColors();

			DiffOptions.OptionsChanged += this.DiffOptionsChanged;

			// We have to manually attach to the GotFocus event because it
			// isn't shown by the Event designer.  .NET wants us to use the
			// Enter event.  Unfortunately, the Focused property isn't set
			// yet when that event fires, so ActiveView returns the wrong
			// information.  We really do need the GotFocus event.
			this.ViewA.GotFocus += this.View_PositionChanged;
			this.ViewB.GotFocus += this.View_PositionChanged;
			this.ViewLineDiff.GotFocus += this.View_PositionChanged;
		}

		#endregion

		#region Public Events

		public event EventHandler LineDiffSizeChanged;

		public event EventHandler RecompareNeeded;

		public event EventHandler<DifferenceEventArgs> ShowTextDifferences;

		#endregion

		#region Public Properties

		public bool CanCompareSelectedText => this.ShowTextDifferences != null && this.ViewA.HasSelection && this.ViewB.HasSelection;

		public bool CanCopy => this.ActiveView.HasSelection;

		public bool CanFind => this.HasText;

		public bool CanFindNext => this.HasText && this.HasFindText;

		public bool CanFindPrevious => this.HasText && this.HasFindText;

		public bool CanGoToFirstDiff => this.HasText && this.ActiveView.CanGoToFirstDiff;

		public bool CanGoToLastDiff => this.HasText && this.ActiveView.CanGoToLastDiff;

		public bool CanGoToLine => this.HasText && this.ActiveView != this.ViewLineDiff;

		public bool CanGoToNextDiff => this.HasText && this.ActiveView.CanGoToNextDiff;

		public bool CanGoToPreviousDiff => this.HasText && this.ActiveView.CanGoToPreviousDiff;

		public bool CanRecompare => this.RecompareNeeded != null && this.edtLeft.Visible && this.edtLeft.TextLength > 0;

		public bool CanViewFile
		{
			get
			{
				bool result = (this.ViewA.Focused || this.ViewB.Focused || this.edtLeft.Focused || this.edtRight.Focused) &&
					(this.edtLeft.TextLength > 0 && this.edtRight.TextLength > 0);
				return result;
			}
		}

		[DefaultValue(38)]
		public int LineDiffHeight
		{
			get
			{
				return this.pnlBottom.Height;
			}

			set
			{
				this.pnlBottom.Height = value;
			}
		}

		[DefaultValue(32)]
		public int OverviewWidth
		{
			get
			{
				return this.Overview.Width;
			}

			set
			{
				this.Overview.Width = value;
				this.DiffControl_SizeChanged(this, EventArgs.Empty);
			}
		}

		[DefaultValue(true)]
		public bool ShowColorLegend
		{
			get
			{
				return this.showColorLegend;
			}

			set
			{
				if (this.showColorLegend != value)
				{
					this.showColorLegend = value;
					this.lblDelete.Visible = value;
					this.lblChange.Visible = value;
					this.lblInsert.Visible = value;
					this.tsSep6.Visible = value;
				}
			}
		}

		[DefaultValue(true)]
		public bool ShowToolBar
		{
			get
			{
				return this.showToolBar;
			}

			set
			{
				if (this.showToolBar != value)
				{
					// Note: We have to store the state ourselves because
					// Visible may return false even after we set it to true
					// if any of its parents are visible.
					this.showToolBar = value;
					this.ToolBar.Visible = value;
				}
			}
		}

		[DefaultValue(false)]
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "InLine",
			Justification = "'inline' is not an appropriate term here.")]
		public bool ShowWhiteSpaceInLineDiff
		{
			get
			{
				return this.ViewLineDiff.ShowWhitespace;
			}

			set
			{
				this.ViewLineDiff.ShowWhitespace = value;
			}
		}

		[DefaultValue(false)]
		public bool ShowWhiteSpaceInMainDiff
		{
			get
			{
				return this.ViewA.ShowWhitespace && this.ViewB.ShowWhitespace;
			}

			set
			{
				this.ViewA.ShowWhitespace = value;
				this.ViewB.ShowWhitespace = value;
			}
		}

		[DefaultValue(true)]
		public bool UseTranslucentOverview
		{
			get
			{
				return this.Overview.UseTranslucentView;
			}

			set
			{
				this.Overview.UseTranslucentView = value;
			}
		}

		public Font ViewFont
		{
			get
			{
				return this.ViewA.Font;
			}

			set
			{
				this.ViewA.Font = value;
				this.ViewB.Font = value;
				this.ViewLineDiff.Font = value;
			}
		}

		#endregion

		#region Private Properties

		private DiffView ActiveView
		{
			get
			{
				if (this.ViewLineDiff.Focused)
				{
					return this.ViewLineDiff;
				}
				else if (this.ViewB.Focused)
				{
					return this.ViewB;
				}
				else
				{
					return this.ViewA;
				}
			}
		}

		private bool HasFindText => this.findData.Text.Length > 0;

		private bool HasText => this.ActiveView.LineCount > 0;

		#endregion

		#region Public Methods

		public bool CompareSelectedText()
		{
			if (this.CanCompareSelectedText)
			{
				string textA = this.ViewA.SelectedText;
				string textB = this.ViewB.SelectedText;

				DifferenceEventArgs diffArgs = new DifferenceEventArgs(textA, textB);
				this.ShowTextDifferences(this, diffArgs);
				return true;
			}

			return false;
		}

		public void Copy()
		{
			Clipboard.SetDataObject(this.ActiveView.SelectedText, true);
		}

		public bool Find()
		{
			bool result = this.ActiveView.Find(this.findData);
			this.UpdateButtons();
			return result;
		}

		public bool FindNext()
		{
			bool result = this.ActiveView.FindNext(this.findData);
			this.UpdateButtons();
			return result;
		}

		public bool FindPrevious()
		{
			bool result = this.ActiveView.FindPrevious(this.findData);
			this.UpdateButtons();
			return result;
		}

		public bool GoToFirstDiff() => this.ActiveView.GoToFirstDiff();

		public bool GoToLastDiff() => this.ActiveView.GoToLastDiff();

		public bool GoToLine() => this.ActiveView.GoToLine();

		public bool GoToNextDiff() => this.ActiveView.GoToNextDiff();

		public bool GoToPreviousDiff() => this.ActiveView.GoToPreviousDiff();

		public bool Recompare()
		{
			if (!this.CanRecompare)
			{
				return false;
			}

			this.RecompareNeeded(this, EventArgs.Empty);
			return true;
		}

		public void SetData(IList<string> listA, IList<string> listB, EditScript script)
		{
			this.SetData(listA, listB, script, string.Empty, string.Empty);
		}

		public void SetData(IList<string> listA, IList<string> listB, EditScript script, string nameA, string nameB)
		{
			this.SetData(listA, listB, script, nameA, nameB, false, false, false);
		}

		public void SetData(
			IList<string> listA,
			IList<string> listB,
			EditScript script,
			string nameA,
			string nameB,
			bool changeDiffIgnoreCase,
			bool changeDiffIgnoreWhiteSpace,
			bool changeDiffTreatAsBinaryLines)
		{
			ChangeDiffOptions changeDiffOptions = ChangeDiffOptions.None;
			if (changeDiffTreatAsBinaryLines)
			{
				changeDiffOptions |= ChangeDiffOptions.IgnoreBinaryPrefix;
			}
			else
			{
				if (changeDiffIgnoreCase)
				{
					changeDiffOptions |= ChangeDiffOptions.IgnoreCase;
				}

				if (changeDiffIgnoreWhiteSpace)
				{
					changeDiffOptions |= ChangeDiffOptions.IgnoreWhitespace;
				}
			}

			this.ViewA.ChangeDiffOptions = changeDiffOptions;
			this.ViewB.ChangeDiffOptions = changeDiffOptions;
			this.ViewLineDiff.ChangeDiffOptions = changeDiffOptions;

			this.ViewA.SetData(listA, script, true);
			this.ViewB.SetData(listB, script, false);
			Debug.Assert(this.ViewA.LineCount == this.ViewB.LineCount, "Both DiffView's LineCounts must be the same");
			this.lblSimilarity.Text = string.Format("{0:P}", script.Similarity);

			this.ViewA.SetCounterpartLines(this.ViewB);
			this.Overview.DiffView = this.ViewA;

			bool showNames = !string.IsNullOrEmpty(nameA) || !string.IsNullOrEmpty(nameB);
			this.edtLeft.Visible = showNames;
			this.edtRight.Visible = showNames;
			if (showNames)
			{
				this.edtLeft.Text = nameA;
				this.edtRight.Text = nameB;
			}

			this.UpdateButtons();
			this.currentDiffLine = -1;
			this.UpdateLineDiff();

			this.ActiveControl = this.ViewA;
		}

		public void ViewFile()
		{
			if (!this.CanViewFile)
			{
				return;
			}

			string fileName;
			if (this.ViewA.Focused || this.edtLeft.Focused)
			{
				fileName = this.edtLeft.Text;
			}
			else
			{
				fileName = this.edtRight.Text;
			}

			WindowsUtility.ShellExecute(this, fileName, "open");
		}

		#endregion

		#region Internal Methods

		internal static void PaintColorLegendItem(ToolStripItem item, PaintEventArgs e)
		{
			if (item != null)
			{
				// Make our outermost painting rect a little smaller.
				Rectangle r = e.ClipRectangle;
				r.Inflate(-1, -1);

				// Paint the background.
				Graphics g = e.Graphics;
				using (Brush b = new SolidBrush(item.BackColor))
				{
					g.FillRectangle(b, r);
				}

				// Draw a border.
				Rectangle borderRect = new Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1);
				ControlPaint.DrawVisualStyleBorder(g, borderRect);

				// Draw the image centered.  (I should probably check the
				// item's ImageAlign property here, but I know I'm always
				// using MiddleCenter for all the passed-in items.)
				Image image = item.Image;
				Rectangle imageRect = new Rectangle(r.X + ((r.Width - image.Width) / 2), r.Y + ((r.Height - image.Height) / 2), image.Width, image.Height);
				g.DrawImage(image, imageRect);
			}
		}

		#endregion

		#region Private Methods

		private void Copy_Click(object sender, EventArgs e)
		{
			this.Copy();
		}

		private void Find_Click(object sender, EventArgs e)
		{
			this.Find();
		}

		private void FindNext_Click(object sender, EventArgs e)
		{
			this.FindNext();
		}

		private void FindPrevious_Click(object sender, EventArgs e)
		{
			this.FindPrevious();
		}

		private void FirstDiff_Click(object sender, EventArgs e)
		{
			this.GoToFirstDiff();
		}

		private void GotoLine_Click(object sender, EventArgs e)
		{
			this.GoToLine();
		}

		private void LastDiff_Click(object sender, EventArgs e)
		{
			this.GoToLastDiff();
		}

		private void NextDiff_Click(object sender, EventArgs e)
		{
			this.GoToNextDiff();
		}

		private void PrevDiff_Click(object sender, EventArgs e)
		{
			this.GoToPreviousDiff();
		}

		private void Recompare_Click(object sender, EventArgs e)
		{
			this.Recompare();
		}

		private void ViewFile_Click(object sender, EventArgs e)
		{
			this.ViewFile();
		}

		private void ColorLegend_Paint(object sender, PaintEventArgs e)
		{
			PaintColorLegendItem(sender as ToolStripItem, e);
		}

		private void DiffControl_SizeChanged(object sender, EventArgs e)
		{
			this.pnlLeft.Width = (this.Width - this.pnlLeft.Left - this.MiddleSplitter.Width) / 2;
		}

		private void DiffOptionsChanged(object sender, EventArgs e)
		{
			this.UpdateColors();
		}

		private void TextDiff_Click(object sender, EventArgs e)
		{
			this.CompareSelectedText();
		}

		private void Overview_LineClick(object sender, DiffLineClickEventArgs e)
		{
			this.ViewA.CenterVisibleLine = e.Line;
			this.ActiveView.Position = new DiffViewPosition(e.Line, 0);
		}

		private void UpdateButtons()
		{
			this.btnViewFile.Enabled = this.CanViewFile;
			this.mnuViewFile.Enabled = this.btnViewFile.Enabled;

			this.btnCopy.Enabled = this.CanCopy;
			this.mnuCopy.Enabled = this.btnCopy.Enabled;

			bool canCompareText = this.CanCompareSelectedText;
			this.btnTextDiff.Enabled = canCompareText;
			this.mnuTextDiff.Enabled = canCompareText;

			this.btnFind.Enabled = this.CanFind;
			this.btnFindNext.Enabled = this.CanFindNext;
			this.btnFindPrevious.Enabled = this.CanFindPrevious;

			this.btnFirstDiff.Enabled = this.CanGoToFirstDiff;
			this.btnNextDiff.Enabled = this.CanGoToNextDiff;
			this.btnPrevDiff.Enabled = this.CanGoToPreviousDiff;
			this.btnLastDiff.Enabled = this.CanGoToLastDiff;

			this.btnGotoLine.Enabled = this.CanGoToLine;
			this.btnRecompare.Enabled = this.CanRecompare;
		}

		private void UpdateColors()
		{
			this.lblDelete.BackColor = DiffOptions.DeletedColor;
			this.lblChange.BackColor = DiffOptions.ChangedColor;
			this.lblInsert.BackColor = DiffOptions.InsertedColor;
		}

		private void UpdateLineDiff()
		{
			int line = (this.ActiveView == this.ViewA) ? this.ViewA.Position.Line : this.ViewB.Position.Line;
			if (line == this.currentDiffLine)
			{
				return;
			}

			this.currentDiffLine = line;

			DiffViewLine lineOne = null;
			DiffViewLine lineTwo = null;
			if (line < this.ViewA.LineCount)
			{
				lineOne = this.ViewA.Lines[line];
			}

			// Normally, ViewA.LineCount == ViewB.LineCount, but during
			// SetData they'll be mismatched momentarily as each view
			// rebuilds its lines.
			if (line < this.ViewB.LineCount)
			{
				lineTwo = this.ViewB.Lines[line];
			}

			if (lineOne != null && lineTwo != null)
			{
				this.ViewLineDiff.SetData(lineOne, lineTwo);
			}
		}

		private void View_PositionChanged(object sender, EventArgs e)
		{
			DiffView view = this.ActiveView;
			DiffViewPosition pos = view.Position;
			this.lblPosition.Text = string.Format("Ln {0}, Col {1}", pos.Line + 1, pos.Column + 1);
			this.UpdateButtons();

			if (view != this.ViewLineDiff)
			{
				this.UpdateLineDiff();
			}
		}

		private void ViewA_HScrollPosChanged(object sender, EventArgs e)
		{
			this.ViewB.HScrollPos = this.ViewA.HScrollPos;
		}

		private void ViewA_VScrollPosChanged(object sender, EventArgs e)
		{
			this.ViewB.VScrollPos = this.ViewA.VScrollPos;
		}

		private void ViewB_HScrollPosChanged(object sender, EventArgs e)
		{
			this.ViewA.HScrollPos = this.ViewB.HScrollPos;
		}

		private void ViewB_VScrollPosChanged(object sender, EventArgs e)
		{
			this.ViewA.VScrollPos = this.ViewB.VScrollPos;
		}

		private void ViewLineDiff_SizeChanged(object sender, EventArgs e)
		{
			if (this.LineDiffSizeChanged != null)
			{
				this.LineDiffSizeChanged(this, e);
			}
		}

		#endregion
	}
}
