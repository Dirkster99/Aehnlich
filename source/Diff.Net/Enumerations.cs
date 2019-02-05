namespace Diff.Net
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;

	#endregion

	#region Internal CompareType

	internal enum CompareType
	{
		Auto,
		Text,
		Xml,
		Binary
	}

	#endregion

	#region Internal DialogDisplay

	internal enum DialogDisplay
	{
		Always,
		UseOption,
		OnlyIfNecessary
	}

	#endregion

	#region Internal DiffType

	internal enum DiffType
	{
		File,
		Directory,
		Text
	}

	#endregion
}
