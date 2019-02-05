namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;

	#endregion

	/// <summary>
	/// The arguments for the <see cref="RecentItemList"/>'s <see cref="RecentItemList.ItemClick"/> event.
	/// </summary>
	public sealed class RecentItemClickEventArgs : EventArgs
	{
		#region Private Data Members

		private string item;
		private IEnumerable<string> values;

		#endregion

		#region Constructors

		internal RecentItemClickEventArgs(string item, IEnumerable<string> values)
		{
			this.item = item;
			this.values = values;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the item that was clicked.
		/// </summary>
		public string Item => this.item;

		/// <summary>
		/// Gets the values associated with the clicked item.
		/// </summary>
		public IEnumerable<string> Values => this.values;

		#endregion
	}
}
