using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Menees;
using Menees.Diffs;
using Menees.Diffs.Controls;
using Menees.Windows.Forms;
namespace Diff.Net
{
	partial class DirDiffForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private MenuStrip MainMenu;
		private ToolStripMenuItem mnuEdit;
		private ToolStripMenuItem mnuRecompare;
		private ToolStripMenuItem mnuShowDifferences;
		private ToolStripMenuItem mnuView;
		private ToolStripSeparator toolStripMenuItem1;
		private Menees.Diffs.Controls.DirectoryDiffControl DiffCtrl;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DirDiffForm));
			this.DiffCtrl = new Menees.Diffs.Controls.DirectoryDiffControl();
			this.MainMenu = new System.Windows.Forms.MenuStrip();
			this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuShowDifferences = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuRecompare = new System.Windows.Forms.ToolStripMenuItem();
			this.MainMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// DiffCtrl
			// 
			this.DiffCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DiffCtrl.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.DiffCtrl.Location = new System.Drawing.Point(0, 24);
			this.DiffCtrl.Name = "DiffCtrl";
			this.DiffCtrl.Size = new System.Drawing.Size(624, 417);
			this.DiffCtrl.TabIndex = 0;
			this.DiffCtrl.RecompareNeeded += new System.EventHandler(this.DiffCtrl_RecompareNeeded);
			this.DiffCtrl.ShowFileDifferences += new System.EventHandler<Menees.Diffs.Controls.DifferenceEventArgs>(this.DiffCtrl_ShowFileDifferences);
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
            this.mnuView,
            this.mnuShowDifferences,
            this.toolStripMenuItem1,
            this.mnuRecompare});
			this.mnuEdit.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.mnuEdit.MergeIndex = 1;
			this.mnuEdit.Name = "mnuEdit";
			this.mnuEdit.Size = new System.Drawing.Size(39, 20);
			this.mnuEdit.Text = "&Edit";
			// 
			// mnuView
			// 
			this.mnuView.Image = global::Diff.Net.Properties.Resources.View;
			this.mnuView.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuView.Name = "mnuView";
			this.mnuView.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.mnuView.Size = new System.Drawing.Size(207, 22);
			this.mnuView.Text = "&View";
			this.mnuView.Click += new System.EventHandler(this.View_Click);
			// 
			// mnuShowDifferences
			// 
			this.mnuShowDifferences.Image = global::Diff.Net.Properties.Resources.ShowDifferences;
			this.mnuShowDifferences.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuShowDifferences.Name = "mnuShowDifferences";
			this.mnuShowDifferences.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
			this.mnuShowDifferences.Size = new System.Drawing.Size(207, 22);
			this.mnuShowDifferences.Text = "Show &Differences";
			this.mnuShowDifferences.Click += new System.EventHandler(this.ShowDifferences_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(204, 6);
			// 
			// mnuRecompare
			// 
			this.mnuRecompare.Image = global::Diff.Net.Properties.Resources.Recompare;
			this.mnuRecompare.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuRecompare.Name = "mnuRecompare";
			this.mnuRecompare.Size = new System.Drawing.Size(207, 22);
			this.mnuRecompare.Text = "&Recompare";
			this.mnuRecompare.Click += new System.EventHandler(this.Recompare_Click);
			// 
			// DirDiffForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.ClientSize = new System.Drawing.Size(624, 441);
			this.Controls.Add(this.DiffCtrl);
			this.Controls.Add(this.MainMenu);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.MainMenu;
			this.Name = "DirDiffForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
			this.Text = "Directory Comparison";
			this.Load += new System.EventHandler(this.DirDiffForm_Load);
			this.MainMenu.ResumeLayout(false);
			this.MainMenu.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
	}
}

