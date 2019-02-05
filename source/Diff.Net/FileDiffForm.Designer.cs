using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Menees;
using Menees.Diffs;
using Menees.Diffs.Controls;
using Menees.Windows.Forms;
namespace Diff.Net
{
	partial class FileDiffForm
	{
		private MenuStrip MainMenu;
		private ToolStripMenuItem mnuCompareSelectedText;
		private ToolStripMenuItem mnuCopy;
		private ToolStripMenuItem mnuEdit;
		private ToolStripMenuItem mnuFind;
		private ToolStripMenuItem mnuFindNext;
		private ToolStripMenuItem mnuFindPrevious;
		private ToolStripMenuItem mnuGoToFirstDiff;
		private ToolStripMenuItem mnuGoToLastDiff;
		private ToolStripMenuItem mnuGoToLine;
		private ToolStripMenuItem mnuGoToNextDiff;
		private ToolStripMenuItem mnuGoToPreviousDiff;
		private ToolStripMenuItem mnuRecompare;
		private ToolStripMenuItem mnuViewFile;
		private ToolStripSeparator tsEditSep1;
		private ToolStripSeparator tsEditSep2;
		private ToolStripSeparator tsEditSep3;
		private ToolStripSeparator tsEditSep4;
		private Menees.Diffs.Controls.DiffControl DiffCtrl;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileDiffForm));
			this.DiffCtrl = new Menees.Diffs.Controls.DiffControl();
			this.MainMenu = new System.Windows.Forms.MenuStrip();
			this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuViewFile = new System.Windows.Forms.ToolStripMenuItem();
			this.tsEditSep1 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuCompareSelectedText = new System.Windows.Forms.ToolStripMenuItem();
			this.tsEditSep2 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuFind = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuFindNext = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuFindPrevious = new System.Windows.Forms.ToolStripMenuItem();
			this.tsEditSep3 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuGoToFirstDiff = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuGoToPreviousDiff = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuGoToNextDiff = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuGoToLastDiff = new System.Windows.Forms.ToolStripMenuItem();
			this.tsEditSep4 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuGoToLine = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuRecompare = new System.Windows.Forms.ToolStripMenuItem();
			this.MainMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// DiffCtrl
			// 
			this.DiffCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DiffCtrl.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.DiffCtrl.LineDiffHeight = 47;
			this.DiffCtrl.Location = new System.Drawing.Point(0, 24);
			this.DiffCtrl.Name = "DiffCtrl";
			this.DiffCtrl.OverviewWidth = 38;
			this.DiffCtrl.ShowWhiteSpaceInLineDiff = true;
			this.DiffCtrl.Size = new System.Drawing.Size(624, 417);
			this.DiffCtrl.TabIndex = 0;
			this.DiffCtrl.ViewFont = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DiffCtrl.LineDiffSizeChanged += new System.EventHandler(this.DiffCtrl_LineDiffSizeChanged);
			this.DiffCtrl.RecompareNeeded += new System.EventHandler(this.DiffCtrl_RecompareNeeded);
			this.DiffCtrl.ShowTextDifferences += new System.EventHandler<Menees.Diffs.Controls.DifferenceEventArgs>(this.DiffCtrl_ShowTextDifferences);
			// 
			// MainMenu
			// 
			this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEdit});
			this.MainMenu.Location = new System.Drawing.Point(0, 0);
			this.MainMenu.Name = "MainMenu";
			this.MainMenu.Size = new System.Drawing.Size(624, 24);
			this.MainMenu.TabIndex = 1;
			this.MainMenu.ItemRemoved += new System.Windows.Forms.ToolStripItemEventHandler(this.MainMenu_ItemRemoved);
			// 
			// mnuEdit
			// 
			this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewFile,
            this.tsEditSep1,
            this.mnuCopy,
            this.mnuCompareSelectedText,
            this.tsEditSep2,
            this.mnuFind,
            this.mnuFindNext,
            this.mnuFindPrevious,
            this.tsEditSep3,
            this.mnuGoToFirstDiff,
            this.mnuGoToPreviousDiff,
            this.mnuGoToNextDiff,
            this.mnuGoToLastDiff,
            this.tsEditSep4,
            this.mnuGoToLine,
            this.mnuRecompare});
			this.mnuEdit.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.mnuEdit.MergeIndex = 1;
			this.mnuEdit.Name = "mnuEdit";
			this.mnuEdit.Size = new System.Drawing.Size(39, 20);
			this.mnuEdit.Text = "&Edit";
			// 
			// mnuViewFile
			// 
			this.mnuViewFile.Image = global::Diff.Net.Properties.Resources.View;
			this.mnuViewFile.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuViewFile.Name = "mnuViewFile";
			this.mnuViewFile.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.mnuViewFile.Size = new System.Drawing.Size(231, 22);
			this.mnuViewFile.Text = "&View File";
			this.mnuViewFile.Click += new System.EventHandler(this.ViewFile_Click);
			// 
			// tsEditSep1
			// 
			this.tsEditSep1.Name = "tsEditSep1";
			this.tsEditSep1.Size = new System.Drawing.Size(228, 6);
			// 
			// mnuCopy
			// 
			this.mnuCopy.Image = global::Diff.Net.Properties.Resources.Copy;
			this.mnuCopy.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuCopy.Name = "mnuCopy";
			this.mnuCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.mnuCopy.Size = new System.Drawing.Size(231, 22);
			this.mnuCopy.Text = "&Copy";
			this.mnuCopy.Click += new System.EventHandler(this.Copy_Click);
			// 
			// mnuCompareSelectedText
			// 
			this.mnuCompareSelectedText.Image = global::Diff.Net.Properties.Resources.ShowDifferences;
			this.mnuCompareSelectedText.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuCompareSelectedText.Name = "mnuCompareSelectedText";
			this.mnuCompareSelectedText.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
			this.mnuCompareSelectedText.Size = new System.Drawing.Size(231, 22);
			this.mnuCompareSelectedText.Text = "Compare &Text...";
			this.mnuCompareSelectedText.Click += new System.EventHandler(this.CompareSelectedText_Click);
			// 
			// tsEditSep2
			// 
			this.tsEditSep2.Name = "tsEditSep2";
			this.tsEditSep2.Size = new System.Drawing.Size(228, 6);
			// 
			// mnuFind
			// 
			this.mnuFind.Image = global::Diff.Net.Properties.Resources.Find;
			this.mnuFind.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuFind.Name = "mnuFind";
			this.mnuFind.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.mnuFind.Size = new System.Drawing.Size(231, 22);
			this.mnuFind.Text = "&Find...";
			this.mnuFind.Click += new System.EventHandler(this.Find_Click);
			// 
			// mnuFindNext
			// 
			this.mnuFindNext.Image = global::Diff.Net.Properties.Resources.FindNext;
			this.mnuFindNext.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuFindNext.Name = "mnuFindNext";
			this.mnuFindNext.ShortcutKeys = System.Windows.Forms.Keys.F3;
			this.mnuFindNext.Size = new System.Drawing.Size(231, 22);
			this.mnuFindNext.Text = "Find &Next";
			this.mnuFindNext.Click += new System.EventHandler(this.FindNext_Click);
			// 
			// mnuFindPrevious
			// 
			this.mnuFindPrevious.Image = global::Diff.Net.Properties.Resources.FindPrev;
			this.mnuFindPrevious.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuFindPrevious.Name = "mnuFindPrevious";
			this.mnuFindPrevious.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
			this.mnuFindPrevious.Size = new System.Drawing.Size(231, 22);
			this.mnuFindPrevious.Text = "Find &Previous";
			this.mnuFindPrevious.Click += new System.EventHandler(this.FindPrevious_Click);
			// 
			// tsEditSep3
			// 
			this.tsEditSep3.Name = "tsEditSep3";
			this.tsEditSep3.Size = new System.Drawing.Size(228, 6);
			// 
			// mnuGoToFirstDiff
			// 
			this.mnuGoToFirstDiff.Image = global::Diff.Net.Properties.Resources.FirstDiff;
			this.mnuGoToFirstDiff.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuGoToFirstDiff.Name = "mnuGoToFirstDiff";
			this.mnuGoToFirstDiff.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F7)));
			this.mnuGoToFirstDiff.Size = new System.Drawing.Size(231, 22);
			this.mnuGoToFirstDiff.Text = "Go To First Diff";
			this.mnuGoToFirstDiff.Click += new System.EventHandler(this.GoToFirstDiff_Click);
			// 
			// mnuGoToPreviousDiff
			// 
			this.mnuGoToPreviousDiff.Image = global::Diff.Net.Properties.Resources.PrevDiff;
			this.mnuGoToPreviousDiff.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuGoToPreviousDiff.Name = "mnuGoToPreviousDiff";
			this.mnuGoToPreviousDiff.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F7)));
			this.mnuGoToPreviousDiff.Size = new System.Drawing.Size(231, 22);
			this.mnuGoToPreviousDiff.Text = "Go To Pre&vious Diff";
			this.mnuGoToPreviousDiff.Click += new System.EventHandler(this.GoToPreviousDiff_Click);
			// 
			// mnuGoToNextDiff
			// 
			this.mnuGoToNextDiff.Image = global::Diff.Net.Properties.Resources.NextDiff;
			this.mnuGoToNextDiff.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuGoToNextDiff.Name = "mnuGoToNextDiff";
			this.mnuGoToNextDiff.ShortcutKeys = System.Windows.Forms.Keys.F7;
			this.mnuGoToNextDiff.Size = new System.Drawing.Size(231, 22);
			this.mnuGoToNextDiff.Text = "Go To Ne&xt Diff";
			this.mnuGoToNextDiff.Click += new System.EventHandler(this.GoToNextDiff_Click);
			// 
			// mnuGoToLastDiff
			// 
			this.mnuGoToLastDiff.Image = global::Diff.Net.Properties.Resources.LastDiff;
			this.mnuGoToLastDiff.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuGoToLastDiff.Name = "mnuGoToLastDiff";
			this.mnuGoToLastDiff.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F7)));
			this.mnuGoToLastDiff.Size = new System.Drawing.Size(231, 22);
			this.mnuGoToLastDiff.Text = "Go To Last Diff";
			this.mnuGoToLastDiff.Click += new System.EventHandler(this.GoToLastDiff_Click);
			// 
			// tsEditSep4
			// 
			this.tsEditSep4.Name = "tsEditSep4";
			this.tsEditSep4.Size = new System.Drawing.Size(228, 6);
			// 
			// mnuGoToLine
			// 
			this.mnuGoToLine.Image = global::Diff.Net.Properties.Resources.GotoLine;
			this.mnuGoToLine.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuGoToLine.Name = "mnuGoToLine";
			this.mnuGoToLine.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
			this.mnuGoToLine.Size = new System.Drawing.Size(231, 22);
			this.mnuGoToLine.Text = "Go To &Line...";
			this.mnuGoToLine.Click += new System.EventHandler(this.GoToLine_Click);
			// 
			// mnuRecompare
			// 
			this.mnuRecompare.Image = global::Diff.Net.Properties.Resources.Recompare;
			this.mnuRecompare.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuRecompare.Name = "mnuRecompare";
			this.mnuRecompare.Size = new System.Drawing.Size(231, 22);
			this.mnuRecompare.Text = "&Recompare";
			this.mnuRecompare.Click += new System.EventHandler(this.Recompare_Click);
			// 
			// FileDiffForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.ClientSize = new System.Drawing.Size(624, 441);
			this.Controls.Add(this.DiffCtrl);
			this.Controls.Add(this.MainMenu);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.MainMenu;
			this.Name = "FileDiffForm";
			this.Text = "File Comparison";
			this.Closed += new System.EventHandler(this.FileDiffForm_Closed);
			this.Load += new System.EventHandler(this.FileDiffForm_Load);
			this.Shown += new System.EventHandler(this.FileDiffForm_Shown);
			this.MainMenu.ResumeLayout(false);
			this.MainMenu.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
	}
}

