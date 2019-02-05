using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Menees.Diffs.Controls;
using Menees.Windows.Forms;
namespace Diff.Net
{
	partial class MainForm
	{
		private System.ComponentModel.IContainer components;
		private MenuStrip MainMenu;
		private ToolStripMenuItem mnuAbout;
		private ToolStripMenuItem mnuCascade;
		private ToolStripMenuItem mnuCloseAll;
		private ToolStripMenuItem mnuCompareDirectories;
		private ToolStripMenuItem mnuCompareFiles;
		private ToolStripMenuItem mnuCompareText;
		private ToolStripMenuItem mnuExit;
		private ToolStripMenuItem mnuFile;
		private ToolStripMenuItem mnuHelp;
		private ToolStripMenuItem mnuOptions;
		private ToolStripMenuItem mnuRecentDirectories;
		private ToolStripMenuItem mnuRecentFiles;
		private ToolStripMenuItem mnuTileHorizontally;
		private ToolStripMenuItem mnuTileVertically;
		private ToolStripMenuItem mnuTools;
		private ToolStripMenuItem mnuWindow;
		private ToolStripSeparator tsFileSep1;
		private ToolStripSeparator tsFileSep2;
		private ToolStripSeparator tsWindowSep1;
		private FormSaver FormSave;
		private RecentItemList RecentDirs;
		private RecentItemList RecentFiles;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.FormSave = new Menees.Windows.Forms.FormSaver(this.components);
			this.RecentFiles = new Menees.Windows.Forms.RecentItemList(this.components);
			this.mnuRecentFiles = new System.Windows.Forms.ToolStripMenuItem();
			this.RecentDirs = new Menees.Windows.Forms.RecentItemList(this.components);
			this.mnuRecentDirectories = new System.Windows.Forms.ToolStripMenuItem();
			this.MainMenu = new System.Windows.Forms.MenuStrip();
			this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuCompareFiles = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuCompareDirectories = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuCompareText = new System.Windows.Forms.ToolStripMenuItem();
			this.tsFileSep1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsFileSep2 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuTools = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuWindow = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuTileVertically = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuTileHorizontally = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuCascade = new System.Windows.Forms.ToolStripMenuItem();
			this.tsWindowSep1 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuCloseAll = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.mdiTabStrip = new Menees.Windows.Forms.MdiTabStrip();
			this.mdiTabContext = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.closeTab = new System.Windows.Forms.ToolStripMenuItem();
			this.closeAllButThisTab = new System.Windows.Forms.ToolStripMenuItem();
			this.MainMenu.SuspendLayout();
			this.mdiTabContext.SuspendLayout();
			this.SuspendLayout();
			// 
			// FormSave
			// 
			this.FormSave.ContainerControl = this;
			this.FormSave.LoadSettings += new System.EventHandler<Menees.SettingsEventArgs>(this.FormSave_LoadSettings);
			this.FormSave.SaveSettings += new System.EventHandler<Menees.SettingsEventArgs>(this.FormSave_SaveSettings);
			// 
			// RecentFiles
			// 
			this.RecentFiles.FormSaver = this.FormSave;
			this.RecentFiles.Items = new string[0];
			this.RecentFiles.MenuItem = this.mnuRecentFiles;
			this.RecentFiles.SettingsNodeName = "Recent Files";
			this.RecentFiles.ItemClick += new System.EventHandler<Menees.Windows.Forms.RecentItemClickEventArgs>(this.RecentFiles_ItemClick);
			// 
			// mnuRecentFiles
			// 
			this.mnuRecentFiles.Name = "mnuRecentFiles";
			this.mnuRecentFiles.Size = new System.Drawing.Size(265, 22);
			this.mnuRecentFiles.Text = "Recent Files";
			// 
			// RecentDirs
			// 
			this.RecentDirs.FormSaver = this.FormSave;
			this.RecentDirs.Items = new string[0];
			this.RecentDirs.MenuItem = this.mnuRecentDirectories;
			this.RecentDirs.SettingsNodeName = "Recent Directories";
			this.RecentDirs.ItemClick += new System.EventHandler<Menees.Windows.Forms.RecentItemClickEventArgs>(this.RecentDirs_ItemClick);
			// 
			// mnuRecentDirectories
			// 
			this.mnuRecentDirectories.Name = "mnuRecentDirectories";
			this.mnuRecentDirectories.Size = new System.Drawing.Size(265, 22);
			this.mnuRecentDirectories.Text = "Recent Directories";
			// 
			// MainMenu
			// 
			this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuTools,
            this.mnuWindow,
            this.mnuHelp});
			this.MainMenu.Location = new System.Drawing.Point(0, 0);
			this.MainMenu.MdiWindowListItem = this.mnuWindow;
			this.MainMenu.Name = "MainMenu";
			this.MainMenu.Size = new System.Drawing.Size(492, 24);
			this.MainMenu.TabIndex = 1;
			// 
			// mnuFile
			// 
			this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCompareFiles,
            this.mnuCompareDirectories,
            this.mnuCompareText,
            this.tsFileSep1,
            this.mnuRecentFiles,
            this.mnuRecentDirectories,
            this.tsFileSep2,
            this.mnuExit});
			this.mnuFile.MergeIndex = 0;
			this.mnuFile.Name = "mnuFile";
			this.mnuFile.Size = new System.Drawing.Size(37, 20);
			this.mnuFile.Text = "&File";
			// 
			// mnuCompareFiles
			// 
			this.mnuCompareFiles.Image = global::Diff.Net.Properties.Resources.FileDiff;
			this.mnuCompareFiles.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuCompareFiles.Name = "mnuCompareFiles";
			this.mnuCompareFiles.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F)));
			this.mnuCompareFiles.Size = new System.Drawing.Size(265, 22);
			this.mnuCompareFiles.Text = "Compare &Files...";
			this.mnuCompareFiles.Click += new System.EventHandler(this.CompareFiles_Click);
			// 
			// mnuCompareDirectories
			// 
			this.mnuCompareDirectories.Image = global::Diff.Net.Properties.Resources.DirDiff;
			this.mnuCompareDirectories.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuCompareDirectories.Name = "mnuCompareDirectories";
			this.mnuCompareDirectories.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D)));
			this.mnuCompareDirectories.Size = new System.Drawing.Size(265, 22);
			this.mnuCompareDirectories.Text = "Compare &Directories...";
			this.mnuCompareDirectories.Click += new System.EventHandler(this.CompareDirectories_Click);
			// 
			// mnuCompareText
			// 
			this.mnuCompareText.Image = global::Diff.Net.Properties.Resources.TextDiff;
			this.mnuCompareText.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuCompareText.Name = "mnuCompareText";
			this.mnuCompareText.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.T)));
			this.mnuCompareText.Size = new System.Drawing.Size(265, 22);
			this.mnuCompareText.Text = "Compare &Text...";
			this.mnuCompareText.Click += new System.EventHandler(this.CompareText_Click);
			// 
			// tsFileSep1
			// 
			this.tsFileSep1.Name = "tsFileSep1";
			this.tsFileSep1.Size = new System.Drawing.Size(262, 6);
			// 
			// tsFileSep2
			// 
			this.tsFileSep2.Name = "tsFileSep2";
			this.tsFileSep2.Size = new System.Drawing.Size(262, 6);
			// 
			// mnuExit
			// 
			this.mnuExit.Image = global::Diff.Net.Properties.Resources.Exit;
			this.mnuExit.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuExit.Name = "mnuExit";
			this.mnuExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.mnuExit.Size = new System.Drawing.Size(265, 22);
			this.mnuExit.Text = "E&xit";
			this.mnuExit.Click += new System.EventHandler(this.Exit_Click);
			// 
			// mnuTools
			// 
			this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOptions});
			this.mnuTools.MergeIndex = 7;
			this.mnuTools.Name = "mnuTools";
			this.mnuTools.Size = new System.Drawing.Size(48, 20);
			this.mnuTools.Text = "&Tools";
			// 
			// mnuOptions
			// 
			this.mnuOptions.Image = global::Diff.Net.Properties.Resources.Options;
			this.mnuOptions.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuOptions.Name = "mnuOptions";
			this.mnuOptions.Size = new System.Drawing.Size(125, 22);
			this.mnuOptions.Text = "&Options...";
			this.mnuOptions.Click += new System.EventHandler(this.Options_Click);
			// 
			// mnuWindow
			// 
			this.mnuWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuTileVertically,
            this.mnuTileHorizontally,
            this.mnuCascade,
            this.tsWindowSep1,
            this.mnuCloseAll});
			this.mnuWindow.MergeIndex = 8;
			this.mnuWindow.Name = "mnuWindow";
			this.mnuWindow.Size = new System.Drawing.Size(63, 20);
			this.mnuWindow.Text = "&Window";
			// 
			// mnuTileVertically
			// 
			this.mnuTileVertically.Image = global::Diff.Net.Properties.Resources.TileVertically;
			this.mnuTileVertically.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuTileVertically.Name = "mnuTileVertically";
			this.mnuTileVertically.Size = new System.Drawing.Size(160, 22);
			this.mnuTileVertically.Text = "Tile &Vertically";
			this.mnuTileVertically.Click += new System.EventHandler(this.TileVertically_Click);
			// 
			// mnuTileHorizontally
			// 
			this.mnuTileHorizontally.Image = global::Diff.Net.Properties.Resources.TileHorizontally;
			this.mnuTileHorizontally.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuTileHorizontally.Name = "mnuTileHorizontally";
			this.mnuTileHorizontally.Size = new System.Drawing.Size(160, 22);
			this.mnuTileHorizontally.Text = "Tile &Horizontally";
			this.mnuTileHorizontally.Click += new System.EventHandler(this.TileHorizontally_Click);
			// 
			// mnuCascade
			// 
			this.mnuCascade.Image = global::Diff.Net.Properties.Resources.Cascade;
			this.mnuCascade.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuCascade.Name = "mnuCascade";
			this.mnuCascade.Size = new System.Drawing.Size(160, 22);
			this.mnuCascade.Text = "&Cascade";
			this.mnuCascade.Click += new System.EventHandler(this.Cascade_Click);
			// 
			// tsWindowSep1
			// 
			this.tsWindowSep1.Name = "tsWindowSep1";
			this.tsWindowSep1.Size = new System.Drawing.Size(157, 6);
			// 
			// mnuCloseAll
			// 
			this.mnuCloseAll.Name = "mnuCloseAll";
			this.mnuCloseAll.Size = new System.Drawing.Size(160, 22);
			this.mnuCloseAll.Text = "Close &All";
			this.mnuCloseAll.Click += new System.EventHandler(this.CloseAll_Click);
			// 
			// mnuHelp
			// 
			this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAbout});
			this.mnuHelp.MergeIndex = 9;
			this.mnuHelp.Name = "mnuHelp";
			this.mnuHelp.Size = new System.Drawing.Size(44, 20);
			this.mnuHelp.Text = "&Help";
			// 
			// mnuAbout
			// 
			this.mnuAbout.Image = global::Diff.Net.Properties.Resources.Help;
			this.mnuAbout.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuAbout.Name = "mnuAbout";
			this.mnuAbout.Size = new System.Drawing.Size(116, 22);
			this.mnuAbout.Text = "&About...";
			this.mnuAbout.Click += new System.EventHandler(this.About_Click);
			// 
			// mdiTabStrip
			// 
			this.mdiTabStrip.ContextMenuStrip = this.mdiTabContext;
			this.mdiTabStrip.Location = new System.Drawing.Point(0, 24);
			this.mdiTabStrip.Name = "mdiTabStrip";
			this.mdiTabStrip.Size = new System.Drawing.Size(492, 25);
			this.mdiTabStrip.TabIndex = 3;
			this.mdiTabStrip.Text = "mdiTabStrip";
			// 
			// mdiTabContext
			// 
			this.mdiTabContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeTab,
            this.closeAllButThisTab});
			this.mdiTabContext.Name = "contextMenuStrip1";
			this.mdiTabContext.Size = new System.Drawing.Size(167, 48);
			this.mdiTabContext.Opening += new System.ComponentModel.CancelEventHandler(this.MdiTabContext_Opening);
			// 
			// closeTab
			// 
			this.closeTab.Name = "closeTab";
			this.closeTab.Size = new System.Drawing.Size(166, 22);
			this.closeTab.Text = "&Close";
			this.closeTab.Click += new System.EventHandler(this.CloseTab_Click);
			// 
			// closeAllButThisTab
			// 
			this.closeAllButThisTab.Name = "closeAllButThisTab";
			this.closeAllButThisTab.Size = new System.Drawing.Size(166, 22);
			this.closeAllButThisTab.Text = "Close &All But This";
			this.closeAllButThisTab.Click += new System.EventHandler(this.CloseAllButThisTab_Click);
			// 
			// MainForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.ClientSize = new System.Drawing.Size(492, 273);
			this.Controls.Add(this.mdiTabStrip);
			this.Controls.Add(this.MainMenu);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.IsMdiContainer = true;
			this.MainMenuStrip = this.MainMenu;
			this.Name = "MainForm";
			this.Text = "Diff.Net";
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
			this.MainMenu.ResumeLayout(false);
			this.MainMenu.PerformLayout();
			this.mdiTabContext.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private MdiTabStrip mdiTabStrip;
		private ContextMenuStrip mdiTabContext;
		private ToolStripMenuItem closeTab;
		private ToolStripMenuItem closeAllButThisTab;
	}
}

