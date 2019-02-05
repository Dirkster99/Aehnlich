using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Menees.Windows.Forms;
namespace Menees.Diffs.Controls
{
	partial class DiffControl
	{
		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.Overview = new Menees.Diffs.Controls.DiffOverview();
			this.ViewA = new Menees.Diffs.Controls.DiffView();
			this.CtxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mnuViewFile = new System.Windows.Forms.ToolStripMenuItem();
			this.tsSep7 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuTextDiff = new System.Windows.Forms.ToolStripMenuItem();
			this.ViewB = new Menees.Diffs.Controls.DiffView();
			this.pnlSeparator = new System.Windows.Forms.Panel();
			this.pnlMiddle = new System.Windows.Forms.Panel();
			this.pnlRight = new System.Windows.Forms.Panel();
			this.edtRight = new System.Windows.Forms.TextBox();
			this.MiddleSplitter = new System.Windows.Forms.Splitter();
			this.pnlLeft = new System.Windows.Forms.Panel();
			this.edtLeft = new System.Windows.Forms.TextBox();
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.ViewLineDiff = new Menees.Diffs.Controls.DiffView();
			this.BottomSplitter = new System.Windows.Forms.Splitter();
			this.ToolBar = new System.Windows.Forms.ToolStrip();
			this.btnViewFile = new System.Windows.Forms.ToolStripButton();
			this.tsSep1 = new System.Windows.Forms.ToolStripSeparator();
			this.btnCopy = new System.Windows.Forms.ToolStripButton();
			this.btnTextDiff = new System.Windows.Forms.ToolStripButton();
			this.tsSep2 = new System.Windows.Forms.ToolStripSeparator();
			this.btnFind = new System.Windows.Forms.ToolStripButton();
			this.btnFindNext = new System.Windows.Forms.ToolStripButton();
			this.btnFindPrevious = new System.Windows.Forms.ToolStripButton();
			this.tsSep3 = new System.Windows.Forms.ToolStripSeparator();
			this.btnFirstDiff = new System.Windows.Forms.ToolStripButton();
			this.btnPrevDiff = new System.Windows.Forms.ToolStripButton();
			this.btnNextDiff = new System.Windows.Forms.ToolStripButton();
			this.btnLastDiff = new System.Windows.Forms.ToolStripButton();
			this.tsSep4 = new System.Windows.Forms.ToolStripSeparator();
			this.btnGotoLine = new System.Windows.Forms.ToolStripButton();
			this.btnRecompare = new System.Windows.Forms.ToolStripButton();
			this.tsSep5 = new System.Windows.Forms.ToolStripSeparator();
			this.lblDelete = new System.Windows.Forms.ToolStripLabel();
			this.lblChange = new System.Windows.Forms.ToolStripLabel();
			this.lblInsert = new System.Windows.Forms.ToolStripLabel();
			this.tsSep6 = new System.Windows.Forms.ToolStripSeparator();
			this.lblPosition = new System.Windows.Forms.ToolStripLabel();
			this.tsSep8 = new System.Windows.Forms.ToolStripSeparator();
			this.lblSimilarity = new System.Windows.Forms.ToolStripLabel();
			this.CtxMenu.SuspendLayout();
			this.pnlMiddle.SuspendLayout();
			this.pnlRight.SuspendLayout();
			this.pnlLeft.SuspendLayout();
			this.pnlBottom.SuspendLayout();
			this.ToolBar.SuspendLayout();
			this.SuspendLayout();
			// 
			// Overview
			// 
			this.Overview.BackColor = System.Drawing.SystemColors.Window;
			this.Overview.DiffView = null;
			this.Overview.Dock = System.Windows.Forms.DockStyle.Left;
			this.Overview.Location = new System.Drawing.Point(0, 0);
			this.Overview.Name = "Overview";
			this.Overview.Size = new System.Drawing.Size(32, 217);
			this.Overview.TabIndex = 0;
			this.Overview.TabStop = false;
			this.Overview.Text = "diffOverview1";
			this.Overview.LineClick += new System.EventHandler<Menees.Diffs.Controls.DiffLineClickEventArgs>(this.Overview_LineClick);
			// 
			// ViewA
			// 
			this.ViewA.BackColor = System.Drawing.SystemColors.Window;
			this.ViewA.CenterVisibleLine = 5;
			this.ViewA.ChangeDiffOptions = Menees.Diffs.Controls.ChangeDiffOptions.None;
			this.ViewA.ContextMenuStrip = this.CtxMenu;
			this.ViewA.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.ViewA.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ViewA.FirstVisibleLine = 0;
			this.ViewA.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ViewA.HScrollPos = 0;
			this.ViewA.Location = new System.Drawing.Point(0, 23);
			this.ViewA.Name = "ViewA";
			this.ViewA.Size = new System.Drawing.Size(213, 194);
			this.ViewA.TabIndex = 2;
			this.ViewA.Text = "diffView1";
			this.ViewA.VScrollPos = 0;
			this.ViewA.HScrollPosChanged += new System.EventHandler(this.ViewA_HScrollPosChanged);
			this.ViewA.PositionChanged += new System.EventHandler(this.View_PositionChanged);
			this.ViewA.SelectionChanged += new System.EventHandler(this.View_PositionChanged);
			this.ViewA.VScrollPosChanged += new System.EventHandler(this.ViewA_VScrollPosChanged);
			// 
			// CtxMenu
			// 
			this.CtxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewFile,
            this.tsSep7,
            this.mnuCopy,
            this.mnuTextDiff});
			this.CtxMenu.Name = "CtxMenu";
			this.CtxMenu.Size = new System.Drawing.Size(158, 76);
			// 
			// mnuViewFile
			// 
			this.mnuViewFile.Image = global::Menees.Diffs.Controls.Properties.Resources.View;
			this.mnuViewFile.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuViewFile.Name = "mnuViewFile";
			this.mnuViewFile.Size = new System.Drawing.Size(157, 22);
			this.mnuViewFile.Text = "&View File";
			this.mnuViewFile.Click += new System.EventHandler(this.ViewFile_Click);
			// 
			// tsSep7
			// 
			this.tsSep7.Name = "tsSep7";
			this.tsSep7.Size = new System.Drawing.Size(154, 6);
			// 
			// mnuCopy
			// 
			this.mnuCopy.Image = global::Menees.Diffs.Controls.Properties.Resources.Copy;
			this.mnuCopy.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuCopy.Name = "mnuCopy";
			this.mnuCopy.Size = new System.Drawing.Size(157, 22);
			this.mnuCopy.Text = "&Copy";
			this.mnuCopy.Click += new System.EventHandler(this.Copy_Click);
			// 
			// mnuTextDiff
			// 
			this.mnuTextDiff.Image = global::Menees.Diffs.Controls.Properties.Resources.ShowDifferences;
			this.mnuTextDiff.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuTextDiff.Name = "mnuTextDiff";
			this.mnuTextDiff.Size = new System.Drawing.Size(157, 22);
			this.mnuTextDiff.Text = "Compare &Text...";
			this.mnuTextDiff.Click += new System.EventHandler(this.TextDiff_Click);
			// 
			// ViewB
			// 
			this.ViewB.BackColor = System.Drawing.SystemColors.Window;
			this.ViewB.CenterVisibleLine = 5;
			this.ViewB.ChangeDiffOptions = Menees.Diffs.Controls.ChangeDiffOptions.None;
			this.ViewB.ContextMenuStrip = this.CtxMenu;
			this.ViewB.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.ViewB.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ViewB.FirstVisibleLine = 0;
			this.ViewB.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ViewB.HScrollPos = 0;
			this.ViewB.Location = new System.Drawing.Point(0, 23);
			this.ViewB.Name = "ViewB";
			this.ViewB.Size = new System.Drawing.Size(336, 194);
			this.ViewB.TabIndex = 4;
			this.ViewB.Text = "diffView2";
			this.ViewB.VScrollPos = 0;
			this.ViewB.HScrollPosChanged += new System.EventHandler(this.ViewB_HScrollPosChanged);
			this.ViewB.PositionChanged += new System.EventHandler(this.View_PositionChanged);
			this.ViewB.SelectionChanged += new System.EventHandler(this.View_PositionChanged);
			this.ViewB.VScrollPosChanged += new System.EventHandler(this.ViewB_VScrollPosChanged);
			// 
			// pnlSeparator
			// 
			this.pnlSeparator.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlSeparator.Location = new System.Drawing.Point(32, 0);
			this.pnlSeparator.Name = "pnlSeparator";
			this.pnlSeparator.Size = new System.Drawing.Size(3, 217);
			this.pnlSeparator.TabIndex = 1;
			// 
			// pnlMiddle
			// 
			this.pnlMiddle.Controls.Add(this.pnlRight);
			this.pnlMiddle.Controls.Add(this.MiddleSplitter);
			this.pnlMiddle.Controls.Add(this.pnlLeft);
			this.pnlMiddle.Controls.Add(this.pnlSeparator);
			this.pnlMiddle.Controls.Add(this.Overview);
			this.pnlMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMiddle.Location = new System.Drawing.Point(0, 25);
			this.pnlMiddle.Name = "pnlMiddle";
			this.pnlMiddle.Size = new System.Drawing.Size(587, 217);
			this.pnlMiddle.TabIndex = 1;
			// 
			// pnlRight
			// 
			this.pnlRight.Controls.Add(this.ViewB);
			this.pnlRight.Controls.Add(this.edtRight);
			this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlRight.Location = new System.Drawing.Point(251, 0);
			this.pnlRight.Name = "pnlRight";
			this.pnlRight.Size = new System.Drawing.Size(336, 217);
			this.pnlRight.TabIndex = 5;
			// 
			// edtRight
			// 
			this.edtRight.Dock = System.Windows.Forms.DockStyle.Top;
			this.edtRight.Location = new System.Drawing.Point(0, 0);
			this.edtRight.Name = "edtRight";
			this.edtRight.ReadOnly = true;
			this.edtRight.Size = new System.Drawing.Size(336, 23);
			this.edtRight.TabIndex = 5;
			// 
			// MiddleSplitter
			// 
			this.MiddleSplitter.Location = new System.Drawing.Point(248, 0);
			this.MiddleSplitter.Name = "MiddleSplitter";
			this.MiddleSplitter.Size = new System.Drawing.Size(3, 217);
			this.MiddleSplitter.TabIndex = 3;
			this.MiddleSplitter.TabStop = false;
			// 
			// pnlLeft
			// 
			this.pnlLeft.Controls.Add(this.ViewA);
			this.pnlLeft.Controls.Add(this.edtLeft);
			this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlLeft.Location = new System.Drawing.Point(35, 0);
			this.pnlLeft.Name = "pnlLeft";
			this.pnlLeft.Size = new System.Drawing.Size(213, 217);
			this.pnlLeft.TabIndex = 4;
			// 
			// edtLeft
			// 
			this.edtLeft.Dock = System.Windows.Forms.DockStyle.Top;
			this.edtLeft.Location = new System.Drawing.Point(0, 0);
			this.edtLeft.Name = "edtLeft";
			this.edtLeft.ReadOnly = true;
			this.edtLeft.Size = new System.Drawing.Size(213, 23);
			this.edtLeft.TabIndex = 3;
			// 
			// pnlBottom
			// 
			this.pnlBottom.Controls.Add(this.ViewLineDiff);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 245);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(587, 38);
			this.pnlBottom.TabIndex = 5;
			// 
			// ViewLineDiff
			// 
			this.ViewLineDiff.BackColor = System.Drawing.SystemColors.Window;
			this.ViewLineDiff.CenterVisibleLine = 1;
			this.ViewLineDiff.ChangeDiffOptions = Menees.Diffs.Controls.ChangeDiffOptions.None;
			this.ViewLineDiff.ContextMenuStrip = this.CtxMenu;
			this.ViewLineDiff.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.ViewLineDiff.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ViewLineDiff.FirstVisibleLine = 0;
			this.ViewLineDiff.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ViewLineDiff.HScrollPos = 0;
			this.ViewLineDiff.Location = new System.Drawing.Point(0, 0);
			this.ViewLineDiff.Name = "ViewLineDiff";
			this.ViewLineDiff.ShowWhitespace = true;
			this.ViewLineDiff.Size = new System.Drawing.Size(587, 38);
			this.ViewLineDiff.TabIndex = 3;
			this.ViewLineDiff.Text = "diffView1";
			this.ViewLineDiff.VScrollPos = 0;
			this.ViewLineDiff.PositionChanged += new System.EventHandler(this.View_PositionChanged);
			this.ViewLineDiff.SelectionChanged += new System.EventHandler(this.View_PositionChanged);
			this.ViewLineDiff.SizeChanged += new System.EventHandler(this.ViewLineDiff_SizeChanged);
			// 
			// BottomSplitter
			// 
			this.BottomSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.BottomSplitter.Location = new System.Drawing.Point(0, 242);
			this.BottomSplitter.Name = "BottomSplitter";
			this.BottomSplitter.Size = new System.Drawing.Size(587, 3);
			this.BottomSplitter.TabIndex = 6;
			this.BottomSplitter.TabStop = false;
			// 
			// ToolBar
			// 
			this.ToolBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.ToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnViewFile,
            this.tsSep1,
            this.btnCopy,
            this.btnTextDiff,
            this.tsSep2,
            this.btnFind,
            this.btnFindNext,
            this.btnFindPrevious,
            this.tsSep3,
            this.btnFirstDiff,
            this.btnPrevDiff,
            this.btnNextDiff,
            this.btnLastDiff,
            this.tsSep4,
            this.btnGotoLine,
            this.btnRecompare,
            this.tsSep5,
            this.lblDelete,
            this.lblChange,
            this.lblInsert,
            this.tsSep6,
            this.lblSimilarity,
            this.tsSep8,
            this.lblPosition});
			this.ToolBar.Location = new System.Drawing.Point(0, 0);
			this.ToolBar.Name = "ToolBar";
			this.ToolBar.Size = new System.Drawing.Size(587, 25);
			this.ToolBar.TabIndex = 7;
			this.ToolBar.Text = "toolStrip1";
			// 
			// btnViewFile
			// 
			this.btnViewFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnViewFile.Image = global::Menees.Diffs.Controls.Properties.Resources.View;
			this.btnViewFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnViewFile.Name = "btnViewFile";
			this.btnViewFile.Size = new System.Drawing.Size(23, 22);
			this.btnViewFile.Text = "View File";
			this.btnViewFile.Click += new System.EventHandler(this.ViewFile_Click);
			// 
			// tsSep1
			// 
			this.tsSep1.Name = "tsSep1";
			this.tsSep1.Size = new System.Drawing.Size(6, 25);
			// 
			// btnCopy
			// 
			this.btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnCopy.Image = global::Menees.Diffs.Controls.Properties.Resources.Copy;
			this.btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnCopy.Name = "btnCopy";
			this.btnCopy.Size = new System.Drawing.Size(23, 22);
			this.btnCopy.Text = "Copy";
			this.btnCopy.Click += new System.EventHandler(this.Copy_Click);
			// 
			// btnTextDiff
			// 
			this.btnTextDiff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnTextDiff.Image = global::Menees.Diffs.Controls.Properties.Resources.ShowDifferences;
			this.btnTextDiff.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnTextDiff.Name = "btnTextDiff";
			this.btnTextDiff.Size = new System.Drawing.Size(23, 22);
			this.btnTextDiff.Text = "Compare Text";
			this.btnTextDiff.Click += new System.EventHandler(this.TextDiff_Click);
			// 
			// tsSep2
			// 
			this.tsSep2.Name = "tsSep2";
			this.tsSep2.Size = new System.Drawing.Size(6, 25);
			// 
			// btnFind
			// 
			this.btnFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnFind.Image = global::Menees.Diffs.Controls.Properties.Resources.Find;
			this.btnFind.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnFind.Name = "btnFind";
			this.btnFind.Size = new System.Drawing.Size(23, 22);
			this.btnFind.Text = "Find";
			this.btnFind.Click += new System.EventHandler(this.Find_Click);
			// 
			// btnFindNext
			// 
			this.btnFindNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnFindNext.Image = global::Menees.Diffs.Controls.Properties.Resources.FindNext;
			this.btnFindNext.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnFindNext.Name = "btnFindNext";
			this.btnFindNext.Size = new System.Drawing.Size(23, 22);
			this.btnFindNext.Text = "Find Next";
			this.btnFindNext.Click += new System.EventHandler(this.FindNext_Click);
			// 
			// btnFindPrevious
			// 
			this.btnFindPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnFindPrevious.Image = global::Menees.Diffs.Controls.Properties.Resources.FindPrev;
			this.btnFindPrevious.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnFindPrevious.Name = "btnFindPrevious";
			this.btnFindPrevious.Size = new System.Drawing.Size(23, 22);
			this.btnFindPrevious.Text = "Find Previous";
			this.btnFindPrevious.Click += new System.EventHandler(this.FindPrevious_Click);
			// 
			// tsSep3
			// 
			this.tsSep3.Name = "tsSep3";
			this.tsSep3.Size = new System.Drawing.Size(6, 25);
			// 
			// btnFirstDiff
			// 
			this.btnFirstDiff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnFirstDiff.Image = global::Menees.Diffs.Controls.Properties.Resources.FirstDiff;
			this.btnFirstDiff.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnFirstDiff.Name = "btnFirstDiff";
			this.btnFirstDiff.Size = new System.Drawing.Size(23, 22);
			this.btnFirstDiff.Text = "First Difference";
			this.btnFirstDiff.ToolTipText = "First Difference";
			this.btnFirstDiff.Click += new System.EventHandler(this.FirstDiff_Click);
			// 
			// btnPrevDiff
			// 
			this.btnPrevDiff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnPrevDiff.Image = global::Menees.Diffs.Controls.Properties.Resources.PrevDiff;
			this.btnPrevDiff.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnPrevDiff.Name = "btnPrevDiff";
			this.btnPrevDiff.Size = new System.Drawing.Size(23, 22);
			this.btnPrevDiff.Text = "Previous Difference";
			this.btnPrevDiff.Click += new System.EventHandler(this.PrevDiff_Click);
			// 
			// btnNextDiff
			// 
			this.btnNextDiff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnNextDiff.Image = global::Menees.Diffs.Controls.Properties.Resources.NextDiff;
			this.btnNextDiff.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnNextDiff.Name = "btnNextDiff";
			this.btnNextDiff.Size = new System.Drawing.Size(23, 22);
			this.btnNextDiff.Text = "Next Difference";
			this.btnNextDiff.Click += new System.EventHandler(this.NextDiff_Click);
			// 
			// btnLastDiff
			// 
			this.btnLastDiff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnLastDiff.Image = global::Menees.Diffs.Controls.Properties.Resources.LastDiff;
			this.btnLastDiff.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnLastDiff.Name = "btnLastDiff";
			this.btnLastDiff.Size = new System.Drawing.Size(23, 22);
			this.btnLastDiff.Text = "Last Difference";
			this.btnLastDiff.Click += new System.EventHandler(this.LastDiff_Click);
			// 
			// tsSep4
			// 
			this.tsSep4.Name = "tsSep4";
			this.tsSep4.Size = new System.Drawing.Size(6, 25);
			// 
			// btnGotoLine
			// 
			this.btnGotoLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnGotoLine.Image = global::Menees.Diffs.Controls.Properties.Resources.GotoLine;
			this.btnGotoLine.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnGotoLine.Name = "btnGotoLine";
			this.btnGotoLine.Size = new System.Drawing.Size(23, 22);
			this.btnGotoLine.Text = "Go To Line";
			this.btnGotoLine.Click += new System.EventHandler(this.GotoLine_Click);
			// 
			// btnRecompare
			// 
			this.btnRecompare.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnRecompare.Image = global::Menees.Diffs.Controls.Properties.Resources.Recompare;
			this.btnRecompare.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnRecompare.Name = "btnRecompare";
			this.btnRecompare.Size = new System.Drawing.Size(23, 22);
			this.btnRecompare.Text = "Recompare";
			this.btnRecompare.Click += new System.EventHandler(this.Recompare_Click);
			// 
			// tsSep5
			// 
			this.tsSep5.Name = "tsSep5";
			this.tsSep5.Size = new System.Drawing.Size(6, 25);
			// 
			// lblDelete
			// 
			this.lblDelete.AutoSize = false;
			this.lblDelete.BackColor = System.Drawing.Color.Pink;
			this.lblDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.lblDelete.Image = global::Menees.Diffs.Controls.Properties.Resources.Deleted;
			this.lblDelete.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.lblDelete.Name = "lblDelete";
			this.lblDelete.Size = new System.Drawing.Size(22, 22);
			this.lblDelete.Text = "Deleted";
			this.lblDelete.ToolTipText = "Deleted";
			this.lblDelete.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorLegend_Paint);
			// 
			// lblChange
			// 
			this.lblChange.AutoSize = false;
			this.lblChange.BackColor = System.Drawing.Color.PaleGreen;
			this.lblChange.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.lblChange.Image = global::Menees.Diffs.Controls.Properties.Resources.Changed;
			this.lblChange.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.lblChange.Name = "lblChange";
			this.lblChange.Size = new System.Drawing.Size(22, 22);
			this.lblChange.Text = "Changed";
			this.lblChange.ToolTipText = "Changed";
			this.lblChange.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorLegend_Paint);
			// 
			// lblInsert
			// 
			this.lblInsert.AutoSize = false;
			this.lblInsert.BackColor = System.Drawing.Color.PaleTurquoise;
			this.lblInsert.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.lblInsert.Image = global::Menees.Diffs.Controls.Properties.Resources.Inserted;
			this.lblInsert.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.lblInsert.Name = "lblInsert";
			this.lblInsert.Size = new System.Drawing.Size(22, 22);
			this.lblInsert.Text = "Inserted";
			this.lblInsert.ToolTipText = "Inserted";
			this.lblInsert.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorLegend_Paint);
			// 
			// tsSep6
			// 
			this.tsSep6.Name = "tsSep6";
			this.tsSep6.Size = new System.Drawing.Size(6, 25);
			// 
			// lblPosition
			// 
			this.lblPosition.Name = "lblPosition";
			this.lblPosition.Size = new System.Drawing.Size(62, 22);
			this.lblPosition.Text = "Ln 1, Col 1";
			// 
			// tsSep8
			// 
			this.tsSep8.Name = "tsSep8";
			this.tsSep8.Size = new System.Drawing.Size(6, 25);
			// 
			// lblSimilarity
			// 
			this.lblSimilarity.Name = "lblSimilarity";
			this.lblSimilarity.Size = new System.Drawing.Size(26, 22);
			this.lblSimilarity.Text = "0 %";
			this.lblSimilarity.ToolTipText = "Similarity";
			// 
			// DiffControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.Controls.Add(this.pnlMiddle);
			this.Controls.Add(this.BottomSplitter);
			this.Controls.Add(this.pnlBottom);
			this.Controls.Add(this.ToolBar);
			this.Name = "DiffControl";
			this.Size = new System.Drawing.Size(587, 283);
			this.SizeChanged += new System.EventHandler(this.DiffControl_SizeChanged);
			this.CtxMenu.ResumeLayout(false);
			this.pnlMiddle.ResumeLayout(false);
			this.pnlRight.ResumeLayout(false);
			this.pnlRight.PerformLayout();
			this.pnlLeft.ResumeLayout(false);
			this.pnlLeft.PerformLayout();
			this.pnlBottom.ResumeLayout(false);
			this.ToolBar.ResumeLayout(false);
			this.ToolBar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}

				DiffOptions.OptionsChanged -= this.DiffOptionsChanged;
			}
			base.Dispose(disposing);
		}

		private System.Windows.Forms.Splitter BottomSplitter;
		private ToolStripButton btnCopy;
		private ToolStripButton btnFind;
		private ToolStripButton btnFindNext;
		private ToolStripButton btnFindPrevious;
		private ToolStripButton btnFirstDiff;
		private ToolStripButton btnGotoLine;
		private ToolStripButton btnLastDiff;
		private ToolStripButton btnNextDiff;
		private ToolStripButton btnPrevDiff;
		private ToolStripButton btnRecompare;
		private ToolStripButton btnTextDiff;
		private ToolStripButton btnViewFile;
		private IContainer components;
		private ContextMenuStrip CtxMenu;
		private System.Windows.Forms.TextBox edtLeft;
		private System.Windows.Forms.TextBox edtRight;
		private ToolStripLabel lblChange;
		private ToolStripLabel lblDelete;
		private ToolStripLabel lblInsert;
		private ToolStripLabel lblPosition;
		private System.Windows.Forms.Splitter MiddleSplitter;
		private ToolStripMenuItem mnuCopy;
		private ToolStripMenuItem mnuTextDiff;
		private ToolStripMenuItem mnuViewFile;
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Panel pnlLeft;
		private System.Windows.Forms.Panel pnlMiddle;
		private System.Windows.Forms.Panel pnlRight;
		private System.Windows.Forms.Panel pnlSeparator;
		private ToolStrip ToolBar;
		private ToolStripSeparator tsSep1;
		private ToolStripSeparator tsSep2;
		private ToolStripSeparator tsSep3;
		private ToolStripSeparator tsSep4;
		private ToolStripSeparator tsSep5;
		private ToolStripSeparator tsSep6;
		private ToolStripSeparator tsSep7;
		private Menees.Diffs.Controls.DiffOverview Overview;
		private Menees.Diffs.Controls.DiffView ViewA;
		private Menees.Diffs.Controls.DiffView ViewB;
		private Menees.Diffs.Controls.DiffView ViewLineDiff;
		private ToolStripSeparator tsSep8;
		private ToolStripLabel lblSimilarity;
	}
}

