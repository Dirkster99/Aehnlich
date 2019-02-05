namespace Diff.Net
{
	#region Using Directives

	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;
	using Menees;
	using Menees.Diffs.Controls;
	using Menees.Windows.Forms;

	#endregion

	internal sealed partial class OptionsDialog : ExtendedForm
	{
		#region Constructors

		public OptionsDialog()
		{
			this.InitializeComponent();
		}

		#endregion

		#region Private Methods

		private void Changed_Click(object sender, EventArgs e)
		{
			this.GetColor(this.lblChange);
		}

		private void Deleted_Click(object sender, EventArgs e)
		{
			this.GetColor(this.lblDelete);
		}

		private void Font_Click(object sender, EventArgs e)
		{
			this.FontDlg.Font = this.lblFont.Font;
			if (this.FontDlg.ShowDialog(this) == DialogResult.OK)
			{
				this.lblFont.Font = this.FontDlg.Font;
			}
		}

		private void Inserted_Click(object sender, EventArgs e)
		{
			this.GetColor(this.lblInsert);
		}

		private void OK_Click(object sender, EventArgs e)
		{
			DiffOptions.BeginUpdate();
			Options.BeginUpdate();
			try
			{
				DiffOptions.DeletedColor = this.lblDelete.BackColor;
				DiffOptions.ChangedColor = this.lblChange.BackColor;
				DiffOptions.InsertedColor = this.lblInsert.BackColor;

				Options.ViewFont = this.lblFont.Font;
				Options.ShowWSInMainDiff = this.chkShowWSInMainDiff.Checked;
				Options.ShowWSInLineDiff = this.chkShowWSInLineDiff.Checked;
				Options.GoToFirstDiff = this.chkGoToFirstDiff.Checked;
				DiffOptions.SpacesPerTab = (int)this.edtSpacesPerTab.Value;
				Options.ShowChangeAsDeleteInsert = this.chkShowChangeAsDeleteInsert.Checked;
				Options.ShowMdiTabs = this.showMdiTabs.Checked;
			}
			finally
			{
				DiffOptions.EndUpdate();
				Options.EndUpdate();
			}
		}

		private void ResetChanged_Click(object sender, EventArgs e)
		{
			this.lblChange.BackColor = DiffOptions.DefaultChangedColor;
		}

		private void ResetDeleted_Click(object sender, EventArgs e)
		{
			this.lblDelete.BackColor = DiffOptions.DefaultDeletedColor;
		}

		private void ResetInserted_Click(object sender, EventArgs e)
		{
			this.lblInsert.BackColor = DiffOptions.DefaultInsertedColor;
		}

		private void GetColor(Label label)
		{
			this.ColorDlg.Color = label.BackColor;
			if (this.ColorDlg.ShowDialog(this) == DialogResult.OK)
			{
				label.BackColor = this.ColorDlg.Color;
			}
		}

		private void OptionsDlg_Load(object sender, EventArgs e)
		{
			this.lblDelete.BackColor = DiffOptions.DeletedColor;
			this.lblChange.BackColor = DiffOptions.ChangedColor;
			this.lblInsert.BackColor = DiffOptions.InsertedColor;

			this.lblFont.Font = Options.ViewFont;
			this.chkShowWSInMainDiff.Checked = Options.ShowWSInMainDiff;
			this.chkShowWSInLineDiff.Checked = Options.ShowWSInLineDiff;
			this.chkGoToFirstDiff.Checked = Options.GoToFirstDiff;
			this.edtSpacesPerTab.Value = DiffOptions.SpacesPerTab;
			this.chkShowChangeAsDeleteInsert.Checked = Options.ShowChangeAsDeleteInsert;
			this.showMdiTabs.Checked = Options.ShowMdiTabs;
		}

		#endregion
	}
}
