namespace Menees.Diffs.Controls
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;

	#endregion

	public sealed class DifferenceEventArgs : EventArgs
	{
		#region Constructors

		internal DifferenceEventArgs(string itemA, string itemB)
		{
			this.ItemA = itemA;
			this.ItemB = itemB;
		}

		#endregion

		#region Public Properties

		public string ItemA { get; }

		public string ItemB { get; }

		#endregion
	}
}
