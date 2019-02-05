namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Windows.Forms;
	using Menees.Windows.Forms;

	#endregion

	/// <summary>
	/// Used as a base class for a generic "find" operation that supports FindNext and FindPrevious.
	/// </summary>
	public abstract class Finder
	{
		#region Private Data Members

		// Store a reference to FindData so an external FindData
		// instance can be shared with multiple Finder instances.
		private FindData findData;

		#endregion

		#region Public Properties

		/// <summary>
		/// The find data.
		/// </summary>
		public FindData Data
		{
			get
			{
				if (this.findData == null)
				{
					this.findData = new FindData();
				}

				return this.findData;
			}

			set
			{
				this.findData = value;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Starts a find operation using the specified find data.
		/// </summary>
		/// <param name="owner">The dialog owner.</param>
		/// <param name="findData">The data to find.</param>
		/// <param name="findMode">Whether to find next, previous, or display a dialog.</param>
		/// <returns>True if the find text was found and selected.  False otherwise.</returns>
		public bool Find(IWin32Window owner, FindData findData, FindMode findMode)
		{
			this.Data = findData;

			// Use this.Data instead of findData in case the user passed in null.
			using (IFindDialog dlg = this.Data.CreateFindDialog())
			{
				bool result;
				switch (findMode)
				{
					case FindMode.ShowDialog:
						result = this.Find(owner, dlg);
						break;

					case FindMode.FindPrevious:
						result = this.FindPrevious(owner, dlg);
						break;

					default:
						result = this.FindNext(owner, dlg);
						break;
				}

				return result;
			}
		}

		#endregion

		#region Protected Overridable Methods

		/// <summary>
		/// Called to execute the dialog.  This allows you to do custom actions before
		/// and after the dialog is executed.
		/// </summary>
		/// <param name="findDialog">The dialog to display.</param>
		/// <param name="owner">The dialog owner.</param>
		/// <param name="findData">The find data.</param>
		/// <returns>True if OK was pressed.</returns>
		protected virtual bool OnDialogExecute(IFindDialog findDialog, IWin32Window owner, FindData findData)
		{
			bool result = findDialog.Execute(owner, findData);
			return result;
		}

		/// <summary>
		/// Override to handle find next.
		/// </summary>
		protected abstract bool OnFindNext();

		/// <summary>
		/// Override to handle find previous.
		/// </summary>
		protected abstract bool OnFindPrevious();

		#endregion

		#region Private Methods

		private bool Find(IWin32Window owner, IFindDialog findDialog)
		{
			Conditions.RequireReference(findDialog, "findDialog");

			bool result = false;

			if (this.OnDialogExecute(findDialog, owner, this.Data))
			{
				if (this.Data.SearchUp)
				{
					result = this.FindPrevious(owner, findDialog);
				}
				else
				{
					result = this.FindNext(owner, findDialog);
				}
			}

			return result;
		}

		private bool FindNext(IWin32Window owner, IFindDialog findDialog)
		{
			bool result;

			if (string.IsNullOrEmpty(this.Data.Text))
			{
				this.Data.SearchUp = false;
				result = this.Find(owner, findDialog);
			}
			else
			{
				using (new WaitCursor(owner as Control))
				{
					result = this.OnFindNext();
				}
			}

			return result;
		}

		private bool FindPrevious(IWin32Window owner, IFindDialog findDialog)
		{
			bool result;

			if (string.IsNullOrEmpty(this.Data.Text))
			{
				this.Data.SearchUp = true;
				result = this.Find(owner, findDialog);
			}
			else
			{
				using (new WaitCursor(owner as Control))
				{
					result = this.OnFindPrevious();
				}
			}

			return result;
		}

		#endregion
	}
}
