namespace Menees.Diagnostics
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;

	#endregion

	#region LogLevel

	/// <summary>
	/// The supported logging levels for <see cref="Menees.Log.Write(LogLevel, object, Exception, IDictionary{string, object})"/>.
	/// </summary>
	/// <devnote>
	/// This is similar to .NET's TraceEventType, but the associated literal values are different.
	/// </devnote>
	public enum LogLevel
	{
		/// <summary>
		/// Non-logged message.
		/// </summary>
		None,

		/// <summary>
		/// Debug trace message.
		/// </summary>
		Debug,

		/// <summary>
		/// Informational message.
		/// </summary>
		Info,

		/// <summary>
		/// Warning message about a non-critical problem.
		/// </summary>
		Warning,

		/// <summary>
		/// Recoverable error.
		/// </summary>
		Error,

		/// <summary>
		/// Non-recoverable error (e.g., application crash).
		/// </summary>
		Fatal
	}

	#endregion
}
