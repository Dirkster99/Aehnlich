namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	#endregion

	/// <summary>
	/// Exposes standard find data.
	/// </summary>
	/// <remarks>
	/// This class isn't sealed, so you can inherit from it
	/// and add your own data members.  Then you can use a
	/// custom IFindDialog implementation to edit your data.
	/// </remarks>
	public class FindData
	{
		#region Private Data Members

		private static readonly Func<IFindDialog> CreateStandardFindDialog = () => new FindDialog();

		private Func<IFindDialog> createFindDialog;
		private string caption;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public FindData()
		{
			this.Text = string.Empty;
			this.ShowMessageIfNotFound = true;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the text to find.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Gets or sets whether the search should be case-sensitive.
		/// This defaults to false.
		/// </summary>
		public bool MatchCase { get; set; }

		/// <summary>
		/// Gets or sets whether the search should go down or up.
		/// This defaults to false.
		/// </summary>
		public bool SearchUp { get; set; }

		/// <summary>
		/// Gets or sets whether a message box should be shown if <see cref="Text"/> isn't found.
		/// This defaults to true.
		/// </summary>
		public bool ShowMessageIfNotFound { get; set; }

		/// <summary>
		/// Gets or sets a function that should be used to create a custom find dialog.
		/// If this is null, then a standard <see cref="FindDialog"/> is created.
		/// </summary>
		public Func<IFindDialog> CreateFindDialog
		{
			get { return this.createFindDialog ?? CreateStandardFindDialog; }
			set { this.createFindDialog = value; }
		}

		/// <summary>
		/// Gets or sets the caption to use for dialogs.
		/// This defaults to "Find".
		/// </summary>
		public string Caption
		{
			get { return string.IsNullOrWhiteSpace(this.caption) ? "Find" : this.caption; }
			set { this.caption = value; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets whether the current <see cref="Text"/> is found in <paramref name="value"/> using <see cref="MatchCase"/>.
		/// </summary>
		/// <param name="value">A text value to check.</param>
		/// <returns>True if <paramref name="value"/> contains <see cref="Text"/> when using <see cref="MatchCase"/>.</returns>
		public virtual bool IsFoundIn(string value)
		{
			bool result = !string.IsNullOrEmpty(value) &&
				value.IndexOf(this.Text, this.MatchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase) >= 0;
			return result;
		}

		/// <summary>
		/// If <see cref="ShowMessageIfNotFound"/> is true, then this
		/// reports that the find <see cref="Text"/> was not found.
		/// </summary>
		/// <param name="owner">The owner window for the message box.</param>
		public void ReportNotFound(IWin32Window owner)
		{
			if (this.ShowMessageIfNotFound)
			{
				string message = string.Format("'{0}' was not found.", this.Text);
				MessageBox.Show(owner, message, this.Caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		#endregion
	}
}
