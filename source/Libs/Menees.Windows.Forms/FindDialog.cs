namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	#endregion

	/// <summary>
	/// A standard Find dialog.
	/// </summary>
	public partial class FindDialog : ExtendedForm, IFindDialog
	{
		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public FindDialog()
		{
			this.InitializeComponent();
		}

		#endregion

		#region IFindDialog Members

		/// <summary>
		/// Used to display the Find dialog with the given data.
		/// </summary>
		/// <param name="owner">The dialog owner.</param>
		/// <param name="data">The find data.</param>
		/// <returns>True if OK was pressed.</returns>
		public bool Execute(IWin32Window owner, FindData data)
		{
			this.Text = data.Caption;
			this.findText.Text = data.Text;
			this.matchCase.Checked = data.MatchCase;
			this.searchUp.Checked = data.SearchUp;

			bool result = false;
			if (this.ShowDialog(owner) == DialogResult.OK)
			{
				data.Text = this.findText.Text;
				data.MatchCase = this.matchCase.Checked;
				data.SearchUp = this.searchUp.Checked;
				result = true;
			}

			return result;
		}

		#endregion

		#region Private Methods

		private void FindText_TextChanged(object sender, EventArgs e)
		{
			this.okayButton.Enabled = this.findText.TextLength > 0;
		}

		#endregion
	}
}
