namespace Diff.Net
{
	#region Using Directives

	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.IO;
	using System.Windows.Forms;
	using System.Xml;
	using Menees;
	using Menees.Diffs;
	using Menees.Windows.Forms;

	#endregion

	internal sealed partial class FileDiffDialog : ExtendedForm, IDifferenceDialog
	{
		#region Constructors

		public FileDiffDialog()
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

		public bool OnlyShowIfShiftPressed => Options.OnlyShowFileDialogIfShiftPressed;

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
			this.GetFileName(this.edtLeft, "Get Left File Name");
		}

		private void OK_Click(object sender, EventArgs e)
		{
			this.optionsControl.SaveOptions();

			Options.OnlyShowFileDialogIfShiftPressed = this.chkOnlyIfShift.Checked;
		}

		private void Right_Click(object sender, EventArgs e)
		{
			this.GetFileName(this.edtRight, "Get Right File Name");
		}

		private void Swap_Click(object sender, EventArgs e)
		{
			string temp = this.edtLeft.Text;
			this.edtLeft.Text = this.edtRight.Text;
			this.edtRight.Text = temp;
		}

		private void Edit_TextChanged(object sender, EventArgs e)
		{
			this.UpdateButtons();
		}

		private void FileDiffDlg_DragDrop(object sender, DragEventArgs e)
		{
			MainForm frmMain = (MainForm)this.Owner;
			frmMain.HandleDragDrop(e);
		}

		private void FileDiffDlg_DragEnter(object sender, DragEventArgs e)
		{
			MainForm.HandleDragEnter(e);
		}

		private void FileDiffDlg_Load(object sender, EventArgs e)
		{
			this.chkOnlyIfShift.Checked = Options.OnlyShowFileDialogIfShiftPressed;
		}

		private void GetFileName(TextBox edit, string title)
		{
			string fileName = edit.Text.Trim();
			this.OpenDlg.FileName = fileName;

			// If the file name contains a directory path, make the dialog open in that directory.
			this.OpenDlg.InitialDirectory = string.Empty;
			if (!string.IsNullOrEmpty(fileName))
			{
				// Try to make sure GetDirectoryName won't blow up.
				if (fileName.IndexOfAny(Path.GetInvalidPathChars()) < 0)
				{
					string directoryName = Path.GetDirectoryName(fileName);
					if (!string.IsNullOrEmpty(directoryName))
					{
						this.OpenDlg.InitialDirectory = directoryName;
					}
				}
			}

			this.OpenDlg.Title = title;
			if (this.OpenDlg.ShowDialog(this) == DialogResult.OK)
			{
				edit.Text = this.OpenDlg.FileName;
			}
		}

		private void UpdateButtons()
		{
			try
			{
				this.btnOK.Enabled = this.edtLeft.TextLength > 0 &&
					this.edtRight.TextLength > 0 &&
					Options.FileExists(this.edtLeft.Text) &&
					Options.FileExists(this.edtRight.Text);
			}
			catch (Exception)
			{
				this.btnOK.Enabled = false;
				throw;
			}
		}

		#endregion
	}
}
