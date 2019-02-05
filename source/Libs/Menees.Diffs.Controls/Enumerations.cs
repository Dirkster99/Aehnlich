namespace Menees.Diffs.Controls
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;

	#endregion

	#region internal ChangeDiffOptions

	[Flags]
	internal enum ChangeDiffOptions
	{
		None = 0,
		IgnoreCase = 1,
		IgnoreWhitespace = 2,
		IgnoreBinaryPrefix = 4
	}

	#endregion
}
