namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	#endregion

	internal partial class InputDialog : ExtendedForm
	{
		#region Private Data Members

		private Func<string, string> validate;

		#endregion

		#region Constructors

		public InputDialog()
		{
			this.InitializeComponent();
		}

		#endregion

		#region Public Methods

		public string Execute(
			IWin32Window owner,
			string prompt,
			string defaultValue,
			int? maxLength,
			Func<string, string> validate)
		{
			this.prompt.Text = prompt;
			this.value.Text = defaultValue;
			this.validate = validate;
			if (maxLength != null)
			{
				this.value.MaxLength = maxLength.Value;
			}

			if (owner == null)
			{
				// If there's no owner window, then this dialog should be centered on the screen.
				this.StartPosition = FormStartPosition.CenterScreen;
				this.ShowInTaskbar = true;
			}

			string result = null;
			if (this.ShowDialog(owner) == DialogResult.OK)
			{
				result = this.value.Text;
			}

			return result;
		}

		#endregion

		#region Private Methods

		private void OkayButton_Click(object sender, EventArgs e)
		{
			if (this.validate != null)
			{
				string error = this.validate(this.value.Text);
				this.errorProvider.SetError(this.value, error);

				// Don't close the dialog if there was an error.
				if (!string.IsNullOrEmpty(error))
				{
					this.DialogResult = DialogResult.None;
				}
			}
		}

		#endregion
	}
}