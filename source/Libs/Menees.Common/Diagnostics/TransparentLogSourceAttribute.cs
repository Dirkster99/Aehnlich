namespace Menees.Diagnostics
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;

	#endregion

	/// <summary>
	/// Indicates that the target should be ignored and skipped when the logging
	/// system is searching up the stack for the source of a message.
	/// </summary>
	/// <remarks>
	/// This attribute should only be applied to classes that encapsulate various
	/// logging functionality (e.g., <see cref="Exceptions"/>).
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class)]
	internal sealed class TransparentLogSourceAttribute : Attribute
	{
		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public TransparentLogSourceAttribute()
		{
		}

		#endregion
	}
}
