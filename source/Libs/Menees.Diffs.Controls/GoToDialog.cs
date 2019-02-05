namespace Menees.Diffs.Controls
{
	#region Using Directives

	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;
	using Menees.Windows.Forms;

	#endregion

	internal sealed partial class GoToDialog : ExtendedForm
	{
		#region Constructors

		public GoToDialog()
		{
			this.InitializeComponent();
		}

		#endregion

		#region Public Methods

		public bool Execute(IWin32Window owner, int maxLineNumber, out int line)
		{
			this.lblLineNumber.Text = string.Format("&Line Number (1-{0}):", maxLineNumber);
			this.edtLineNumber.Maximum = maxLineNumber;
			if (this.ShowDialog(owner) == DialogResult.OK)
			{
				line = (int)this.edtLineNumber.Value;
				return true;
			}

			line = 0;
			return false;
		}

		#endregion
	}
}
