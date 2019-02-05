namespace Menees.Diffs.Controls
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;

	#endregion

	[DebuggerDisplay("Start = {Start}, Length = {Length}")]
	internal struct Segment
	{
		#region Constructors

		public Segment(int start, int length)
			: this()
		{
			this.Start = start;
			this.Length = length;
		}

		#endregion

		#region Public Properties

		public int Length { get; }

		public int Start { get; }

		#endregion
	}
}
