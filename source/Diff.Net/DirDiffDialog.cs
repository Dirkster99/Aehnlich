namespace Diff.Net
{
	#region Using Directives

	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.IO;
	using System.Windows.Forms;
    using DiffLib.Dir;
    using Menees;
	using Menees.Diffs;
	using Menees.Windows.Forms;

	#endregion

	internal sealed partial class DirDiffDialog : ExtendedForm, IDifferenceDialog
	{
		#region Constructors

		public DirDiffDialog()
		{
			this.InitializeComponent();

			this.UpdateButtons();
		}

		#endregion

		#region Public Properties

		public string NameA
		{
			get
			{
				return this.edtLeft.Text;
			}

			set
			{
				this.edtLeft.Text = value;
				this.UpdateButtons();
			}
		}

		public string NameB
		{
			get
			{
				return this.edtRight.Text;
			}

			set
			{
				this.edtRight.Text = value;
				this.UpdateButtons();
			}
		}

		public bool OnlyShowIfShiftPressed => Options.OnlyShowDirDialogIfShiftPressed;

		public bool RequiresInput
		{
			get
			{
				this.UpdateButtons();
				return !this.btnOK.Enabled;
			}
		}

		public bool ShowShiftCheck
		{
			set
			{
				this.chkOnlyIfShift.Visible = value;
			}
		}

		#endregion

		#region Private Methods

		private void Left_Click(object sender, EventArgs e)
		{
			this.GetDirectory(this.edtLeft, "Select Left Directory");
		}

		private void OK_Click(object sender, EventArgs e)
		{
			Options.ShowOnlyInA = this.chkShowOnlyInA.Checked;
			Options.ShowOnlyInB = this.chkShowOnlyInB.Checked;
			Options.ShowDifferent = this.chkShowDifferent.Checked;
			Options.ShowSame = this.chkShowSame.Checked;
			Options.Recursive = this.chkRecursive.Checked;
			Options.IgnoreDirectoryComparison = this.chkIgnoreDirectoryComparison.Checked;
			Options.OnlyShowDirDialogIfShiftPressed = this.chkOnlyIfShift.Checked;

			Options.FileFilter = null;
			string filters = this.cbFilters.Text.Trim();
			if (filters.Length > 0)
			{
				Options.FileFilter = new DirectoryDiffFileFilter(filters, this.rbInclude.Checked);
				Options.AddCustomFilter(filters);
			}
		}

		private void Right_Click(object sender, EventArgs e)
		{
			this.GetDirectory(this.edtRight, "Select Right Directory");
		}

		private void Swap_Click(object sender, EventArgs e)
		{
			string temp = this.edtLeft.Text;
			this.edtLeft.Text = this.edtRight.Text;
			this.edtRight.Text = temp;
		}

		private void Filters_TextChanged(object sender, EventArgs e)
		{
			this.UpdateButtons();
		}

		private void DirDiffDlg_DragDrop(object sender, DragEventArgs e)
		{
			MainForm frmMain = (MainForm)this.Owner;
			frmMain.HandleDragDrop(e);
		}

		private void DirDiffDlg_DragEnter(object sender, DragEventArgs e)
		{
			MainForm.HandleDragEnter(e);
		}

		private void DirDiffDlg_Load(object sender, EventArgs e)
		{
			this.chkShowOnlyInA.Checked = Options.ShowOnlyInA;
			this.chkShowOnlyInB.Checked = Options.ShowOnlyInB;
			this.chkShowDifferent.Checked = Options.ShowDifferent;
			this.chkShowSame.Checked = Options.ShowSame;
			this.chkRecursive.Checked = Options.Recursive;
			this.chkIgnoreDirectoryComparison.Checked = Options.IgnoreDirectoryComparison;
			this.chkOnlyIfShift.Checked = Options.OnlyShowDirDialogIfShiftPressed;

			this.cbFilters.Items.AddRange(Options.GetCustomFilters());
		}

		private void Edit_TextChanged(object sender, EventArgs e)
		{
			this.UpdateButtons();
		}

		private void GetDirectory(TextBox edit, string title)
		{
			string selectedFolder = WindowsUtility.SelectFolder(this, title, edit.Text);
			if (!string.IsNullOrEmpty(selectedFolder))
			{
				edit.Text = selectedFolder;
			}
		}

		private void UpdateButtons()
		{
			try
			{
				this.btnOK.Enabled = this.edtLeft.TextLength > 0 &&
					this.edtRight.TextLength > 0 &&
					Options.DirExists(this.edtLeft.Text) &&
					Options.DirExists(this.edtRight.Text);
			}
			catch (Exception)
			{
				this.btnOK.Enabled = false;
				throw;
			}

			bool enable = this.cbFilters.Text.Length > 0;
			this.rbInclude.Enabled = enable;
			this.rbExclude.Enabled = enable;
		}

		#endregion
	}
}
