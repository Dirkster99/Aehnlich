namespace Diff.Net
{
	#region Using Directives

	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;
	using System.Xml;
	using Menees;
	using Menees.Diffs;
	using Menees.Diffs.Controls;
	using Menees.Windows.Forms;

	#endregion

	internal sealed partial class TextDiffForm : ExtendedForm
	{
		#region Constructors

		public TextDiffForm()
		{
			this.InitializeComponent();

			Options.OptionsChanged += this.OptionsChanged;

			this.UpdateButtons();
			this.ApplyOptions();
		}

		#endregion

		#region Private Methods

		private void ApplyOptions()
		{
			this.txtA.Font = Options.ViewFont;
			this.txtA.TabSpaces = DiffOptions.SpacesPerTab;
			this.txtB.Font = Options.ViewFont;
			this.txtB.TabSpaces = DiffOptions.SpacesPerTab;
		}

		private void Cancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void OK_Click(object sender, EventArgs e)
		{
			this.optionsControl.SaveOptions();

			// We have to grab everything from the main form before closing.
			MainForm frmMain = (MainForm)this.MdiParent;
			string left = this.txtA.Text;
			string right = this.txtB.Text;

			// Close this form before showing the next one.
			this.Close();
			frmMain.ShowTextDifferences(left, right);
		}

		private void Swap_Click(object sender, EventArgs e)
		{
			string left = this.txtA.Text;
			this.txtA.Text = this.txtB.Text;
			this.txtB.Text = left;
		}

		private void OptionsChanged(object sender, EventArgs e)
		{
			this.ApplyOptions();
		}

		private void TextPanel_Resize(object sender, EventArgs e)
		{
			this.txtA.Width = (this.pnlText.ClientSize.Width - this.Split.Width) / 2;
		}

		private void TextBox_Changed(object sender, EventArgs e)
		{
			this.UpdateButtons();
		}

		private void TextDiffDlg_Load(object sender, EventArgs e)
		{
			this.txtA.Text = Options.LastTextA;
			this.txtB.Text = Options.LastTextB;
			this.txtA.SelectionLength = 0;

			// http://stackoverflow.com/questions/888865/problem-with-icon-on-creating-new-maximized-mdi-child-form-in-net
			this.Icon = (Icon)this.Icon.Clone();
		}

		private void TextDiffForm_Closed(object sender, EventArgs e)
		{
			Options.OptionsChanged -= this.OptionsChanged;
		}

		private void UpdateButtons()
		{
			this.btnOK.Enabled = this.txtA.TextLength > 0 && this.txtB.TextLength > 0;
		}

		#endregion
	}
}
