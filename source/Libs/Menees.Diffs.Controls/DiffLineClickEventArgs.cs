namespace Menees.Diffs.Controls
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;

	#endregion

	public sealed class DiffLineClickEventArgs : EventArgs
	{
		#region Constructors

		internal DiffLineClickEventArgs(int line)
		{
			this.Line = line;
		}

		#endregion

		#region Public Properties

		public int Line { get; }

		#endregion
	}
}
