namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Windows.Forms;

	#endregion

	/// <summary>
	/// An object that can be used to specify a "wait" cursor during long operations.
	/// </summary>
	public sealed class WaitCursor : IDisposable
	{
		#region Private Data Members

		private Cursor previous;
		private Control control;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance for the specified control.
		/// </summary>
		/// <param name="control">The control whose Cursor property should be changed.</param>
		public WaitCursor(Control control)
		{
			this.control = control;

			// Try to find the highest-level control (i.e., Form) that we can.
			if (this.control != null)
			{
				Form frm = this.control.FindForm();
				if (frm == null)
				{
					frm = Form.ActiveForm;
				}

				if (frm != null)
				{
					this.control = frm;
				}
			}
			else
			{
				this.control = Form.ActiveForm;
			}

			if (this.control == null)
			{
				this.previous = Cursor.Current;
			}
			else
			{
				this.previous = this.control.Cursor;
			}

			this.Refresh();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Closes the cursor and returns it to its previous state.
		/// </summary>
		public void Close()
		{
			this.SetCursor(this.previous);
		}

		/// <summary>
		/// Calls <see cref="Close"/>.
		/// </summary>
		public void Dispose()
		{
			this.Close();
		}

		/// <summary>
		/// Re-sets the wait cursor.
		/// </summary>
		public void Refresh()
		{
			this.SetCursor(Cursors.WaitCursor);
		}

		/// <summary>
		/// Changes the cursor back to the default cursor regardless of nested WaitCursor levels.
		/// </summary>
		public void ShowDefaultCursor()
		{
			this.SetCursor(Cursors.Default);
		}

		#endregion

		#region Private Methods

		private void SetCursor(Cursor newCursor)
		{
			bool useWaitCursor = newCursor == Cursors.WaitCursor;

			if (this.control != null)
			{
				this.control.Cursor = newCursor;
				this.control.UseWaitCursor = useWaitCursor;
			}

			Application.UseWaitCursor = useWaitCursor;
			Cursor.Current = newCursor;
		}

		#endregion
	}
}
