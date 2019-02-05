namespace Diff.Net
{
	#region Using Directives

	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;
    using DiffLib.Dir;
    using Menees;
	using Menees.Diffs;
	using Menees.Diffs.Controls;
	using Menees.Windows.Forms;

	#endregion

	internal sealed partial class DirDiffForm : ExtendedForm, IDifferenceForm
	{
		#region Private Data Members

		private ShowDiffArgs lastDiffArgs;

		#endregion

		#region Constructors

		public DirDiffForm()
		{
			this.InitializeComponent();
		}

		#endregion

		#region Public Properties

		public string ToolTipText
		{
			get
			{
				string result = null;

				if (this.lastDiffArgs != null)
				{
					result = this.lastDiffArgs.A + Environment.NewLine + this.lastDiffArgs.B;
				}

				return result;
			}
		}

		#endregion

		#region Public Methods

		public void ShowDifferences(ShowDiffArgs e)
		{
			DirectoryDiff diff = new DirectoryDiff(
				Options.ShowOnlyInA,
				Options.ShowOnlyInB,
				Options.ShowDifferent,
				Options.ShowSame,
				Options.Recursive,
				Options.IgnoreDirectoryComparison,
				Options.FileFilter);

			DirectoryDiffResults results = diff.Execute(e.A, e.B);

			this.DiffCtrl.SetData(results);

			this.Text = string.Format("{0} : {1}", results.DirectoryA.Name, results.DirectoryB.Name);

			this.Show();

			this.lastDiffArgs = e;
		}

		public void UpdateUI()
		{
			this.mnuView.Enabled = this.DiffCtrl.CanView;
			this.mnuShowDifferences.Enabled = this.DiffCtrl.CanShowDifferences;
			this.mnuRecompare.Enabled = this.DiffCtrl.CanRecompare;
		}

		#endregion

		#region Private Methods

		private void DiffCtrl_RecompareNeeded(object sender, EventArgs e)
		{
			if (this.lastDiffArgs != null)
			{
				using (WaitCursor wc = new WaitCursor(this))
				{
					this.ShowDifferences(this.lastDiffArgs);
				}
			}
		}

		private void DiffCtrl_ShowFileDifferences(object sender, DifferenceEventArgs e)
		{
			MainForm frmMain = (MainForm)this.MdiParent;
			frmMain.ShowFileDifferences(e.ItemA, e.ItemB, DialogDisplay.UseOption);
		}

		private void DirDiffForm_Load(object sender, EventArgs e)
		{
			// http://stackoverflow.com/questions/888865/problem-with-icon-on-creating-new-maximized-mdi-child-form-in-net
			this.Icon = (Icon)this.Icon.Clone();
		}

		private void MainMenu_ItemRemoved(object sender, ToolStripItemEventArgs e)
		{
			// This is needed in .NET 2.0 to make the empty, merged menustrip disappear.
			this.MainMenu.Visible = this.MainMenu.Items.Count > 0;
		}

		private void Recompare_Click(object sender, EventArgs e)
		{
			this.DiffCtrl.Recompare();
		}

		private void ShowDifferences_Click(object sender, EventArgs e)
		{
			this.DiffCtrl.ShowDifferences();
		}

		private void View_Click(object sender, EventArgs e)
		{
			this.DiffCtrl.View();
		}

		#endregion
	}
}
