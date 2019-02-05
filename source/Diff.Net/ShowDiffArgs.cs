namespace Diff.Net
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Text;

	#endregion

	internal sealed class ShowDiffArgs
	{
		#region Constructors

		public ShowDiffArgs(string itemA, string itemB, DiffType diffType)
		{
			this.A = itemA;
			this.B = itemB;
			this.DiffType = diffType;
		}

		#endregion

		#region Public Properties

		public string A { get; }

		public string B { get; }

		public DiffType DiffType { get; }

		#endregion
	}
}
