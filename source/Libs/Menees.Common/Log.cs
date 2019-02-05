namespace Menees
{
	#region Using Directives

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using System.Xml;
	using Menees.Diagnostics;

	#endregion

	/// <summary>
	/// Handles all logging operations.
	/// </summary>
	[DebuggerDisplay("CategoryName = {CategoryName}")]
	public sealed partial class Log
	{
		#region Public Constants

		/// <summary>
		/// The key name for an <see cref="int"/> event ID value passed in a "contextProperties"
		/// parameter to <see cref="Write(Diagnostics.LogLevel, object, Exception, IDictionary{string, object})"/>.
		/// </summary>
		/// <remarks>
		/// If a key with this name is present in the properties dictionary of an event, then the event ID
		/// will be passed as the "id" parameter to <see cref="TraceSource"/>'s Trace methods.
		/// </remarks>
		public const string EventIdPropertyName = "EventId";

		#endregion

		#region Private Data Members

		private static readonly ConcurrentDictionary<string, Log> LogCache = new ConcurrentDictionary<string, Log>(StringComparer.Ordinal);
		private static readonly GlobalLogContext GlobalContextValue = new GlobalLogContext();
		private static readonly ThreadLogContext ThreadContextValue = new ThreadLogContext();

		// Good articles on TraceSource, which is the .NET 2.0+ recommended way to trace (although it's still weaker than log4net).
		// http://msdn.microsoft.com/en-us/library/ms228993.aspx
		// http://olondono.blogspot.com/2008/01/about-trace-listeners.html
		// http://weblogs.asp.net/ralfw/archive/2007/10/31/code-instrumentation-with-tracesource-my-personal-vade-mecum.aspx
		// http://eeichinger.blogspot.com/2009/01/thoughts-on-systemdiagnostics-trace-vs.html
		private TraceSource traceSource;
		private string category;

		#endregion

		#region Constructors

		private Log(string category)
		{
			this.category = category;
			this.traceSource = new TraceSource(category, ApplicationInfo.IsDebugBuild ? SourceLevels.All : SourceLevels.Information);

			// If this trace source only has the default listener, then use the global trace listeners instead.
			// This makes it much easier to configure global listeners that apply to all categories/sources.
			// It also makes unit testing easier because we don't have to expose our TraceSource's listeners
			// programmatically, and unit test results automatically display the output of anything sent to the
			// global trace listeners.
			TraceListenerCollection listeners = this.traceSource.Listeners;
			if (listeners.Count == 1 && listeners[0] is DefaultTraceListener)
			{
				listeners.Clear();
				listeners.AddRange(Trace.Listeners);
			}
		}

		#endregion

		#region Public Static Properties

		/// <summary>
		/// Gets the global context for all logged messages.
		/// </summary>
		/// <remarks>
		/// Key/value pairs added to this context will be recorded in all logged messages.
		/// </remarks>
		public static GlobalLogContext GlobalContext => GlobalContextValue;

		/// <summary>
		/// Gets the current thread's context for logged messages.
		/// </summary>
		/// <remarks>
		/// Key/value pairs added to this context will be recorded in all messages logged
		/// from the current thread.
		/// </remarks>
		public static ThreadLogContext ThreadContext => ThreadContextValue;

		#endregion

		#region Public Instance Properties

		/// <summary>
		/// Gets whether Debug logging is enabled for the current log category.
		/// </summary>
		public bool IsDebugEnabled => this.IsWriteEnabled(LogLevel.Debug);

		/// <summary>
		/// Gets whether Info logging is enabled for the current log category.
		/// </summary>
		public bool IsInfoEnabled => this.IsWriteEnabled(LogLevel.Info);

		/// <summary>
		/// Gets whether Warning logging is enabled for the current log category.
		/// </summary>
		public bool IsWarningEnabled => this.IsWriteEnabled(LogLevel.Warning);

		/// <summary>
		/// Gets whether Error logging is enabled for the current log category.
		/// </summary>
		public bool IsErrorEnabled => this.IsWriteEnabled(LogLevel.Error);

		/// <summary>
		/// Gets whether Fatal logging is enabled for the current log category.
		/// </summary>
		public bool IsFatalEnabled => this.IsWriteEnabled(LogLevel.Fatal);

		/// <summary>
		/// Gets the category name for the current log instance.
		/// </summary>
		public string CategoryName => this.category;

		/// <summary>
		/// Gets the collection of trace listeners for the current log instance.
		/// </summary>
		public TraceListenerCollection Listeners => this.traceSource.Listeners;

		#endregion

		#region Public General Static Methods

		/// <summary>
		/// Gets the shared log instance where the category equals the type's full name.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.</param>
		/// <returns>A new or existing log instance for the specified category.</returns>
		public static Log GetLog(Type category)
		{
			Conditions.RequireReference(category, "category");
			Log result = InternalGetLog(category.FullName);
			return result;
		}

		/// <summary>
		/// Gets the shared log instance for the specified category.
		/// </summary>
		/// <param name="category">A log category name.  Use '.'-separated identifiers to create a
		/// hierarchy of log categories.</param>
		/// <returns>A new or existing log instance for the specified category.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Log GetLog(string category)
		{
			Conditions.RequireString(category, "category");
			Log result = InternalGetLog(category);
			return result;
		}

		#endregion

		#region Public Static "Debug" Methods

		/// <summary>
		/// Logs a Debug message with the specified category and data.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		public static void Debug(Type category, object messageData)
		{
			Debug(category, messageData, null, null);
		}

		/// <summary>
		/// Logs a Debug message with the specified category, data, and exception.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		public static void Debug(Type category, object messageData, Exception ex)
		{
			Debug(category, messageData, ex, null);
		}

		/// <summary>
		/// Logs a Debug message with the specified category, data, and additional context properties.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public static void Debug(Type category, object messageData, IDictionary<string, object> contextProperties)
		{
			Debug(category, messageData, null, contextProperties);
		}

		/// <summary>
		/// Logs a Debug message with the specified category, data, exception, and additional context properties.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public static void Debug(Type category, object messageData, Exception ex, IDictionary<string, object> contextProperties)
		{
			Log log = GetLog(category);
			log.Debug(messageData, ex, contextProperties);
		}

		/// <summary>
		/// Logs a formatted Debug message for the specified category.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="format">A format string.</param>
		/// <param name="args">The parameters to substitute into the format string.</param>
		public static void DebugFormat(Type category, string format, params object[] args)
		{
			string msg = string.Format(format, args);
			Debug(category, msg);
		}

		#endregion

		#region Public Static "Info" Methods

		/// <summary>
		/// Logs an Info message with the specified category and data.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		public static void Info(Type category, object messageData)
		{
			Info(category, messageData, null, null);
		}

		/// <summary>
		/// Logs an Info message with the specified category, data, and exception.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		public static void Info(Type category, object messageData, Exception ex)
		{
			Info(category, messageData, ex, null);
		}

		/// <summary>
		/// Logs an Info message with the specified category, data, and additional context properties.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public static void Info(Type category, object messageData, IDictionary<string, object> contextProperties)
		{
			Info(category, messageData, null, contextProperties);
		}

		/// <summary>
		/// Logs an Info message with the specified category, data, exception, and additional context properties.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public static void Info(Type category, object messageData, Exception ex, IDictionary<string, object> contextProperties)
		{
			Log log = GetLog(category);
			log.Info(messageData, ex, contextProperties);
		}

		/// <summary>
		/// Logs a formatted Info message for the specified category.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="format">A format string.</param>
		/// <param name="args">The parameters to substitute into the format string.</param>
		public static void InfoFormat(Type category, string format, params object[] args)
		{
			string msg = string.Format(format, args);
			Info(category, msg);
		}

		#endregion

		#region Public Static "Warning" Methods

		/// <summary>
		/// Logs a Warning message with the specified category and data.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		public static void Warning(Type category, object messageData)
		{
			Warning(category, messageData, null, null);
		}

		/// <summary>
		/// Logs a Warning message with the specified category, data, and exception.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		public static void Warning(Type category, object messageData, Exception ex)
		{
			Warning(category, messageData, ex, null);
		}

		/// <summary>
		/// Logs a Warning message with the specified category, data, and additional context properties.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public static void Warning(Type category, object messageData, IDictionary<string, object> contextProperties)
		{
			Warning(category, messageData, null, contextProperties);
		}

		/// <summary>
		/// Logs a Warning message with the specified category, data, exception, and additional context properties.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public static void Warning(Type category, object messageData, Exception ex, IDictionary<string, object> contextProperties)
		{
			Log log = GetLog(category);
			log.Warning(messageData, ex, contextProperties);
		}

		/// <summary>
		/// Logs a formatted Warning message for the specified category.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="format">A format string.</param>
		/// <param name="args">The parameters to substitute into the format string.</param>
		public static void WarningFormat(Type category, string format, params object[] args)
		{
			string msg = string.Format(format, args);
			Warning(category, msg);
		}

		#endregion

		#region Public Static "Error" Methods

		/// <summary>
		/// Logs an Error message with the specified category and data.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		public static void Error(Type category, object messageData)
		{
			Error(category, messageData, null, null);
		}

		/// <summary>
		/// Logs an Error message with the specified category, data, and exception.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		public static void Error(Type category, object messageData, Exception ex)
		{
			Error(category, messageData, ex, null);
		}

		/// <summary>
		/// Logs an Error message with the specified category, data, and additional context properties.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public static void Error(Type category, object messageData, IDictionary<string, object> contextProperties)
		{
			Error(category, messageData, null, contextProperties);
		}

		/// <summary>
		/// Logs an Error message with the specified category, data, exception, and additional context properties.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public static void Error(Type category, object messageData, Exception ex, IDictionary<string, object> contextProperties)
		{
			Log log = GetLog(category);
			log.Error(messageData, ex, contextProperties);
		}

		/// <summary>
		/// Logs a formatted Error message for the specified category.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="format">A format string.</param>
		/// <param name="args">The parameters to substitute into the format string.</param>
		public static void ErrorFormat(Type category, string format, params object[] args)
		{
			string msg = string.Format(format, args);
			Error(category, msg);
		}

		#endregion

		#region Public Static "Fatal" Methods

		/// <summary>
		/// Logs a Fatal message with the specified category and data.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		public static void Fatal(Type category, object messageData)
		{
			Fatal(category, messageData, null, null);
		}

		/// <summary>
		/// Logs a Fatal message with the specified category, data, and exception.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		public static void Fatal(Type category, object messageData, Exception ex)
		{
			Fatal(category, messageData, ex, null);
		}

		/// <summary>
		/// Logs a Fatal message with the specified category, data, and additional context properties.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public static void Fatal(Type category, object messageData, IDictionary<string, object> contextProperties)
		{
			Fatal(category, messageData, null, contextProperties);
		}

		/// <summary>
		/// Logs a Fatal message with the specified category, data, exception, and additional context properties.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public static void Fatal(Type category, object messageData, Exception ex, IDictionary<string, object> contextProperties)
		{
			Log log = GetLog(category);
			log.Fatal(messageData, ex, contextProperties);
		}

		/// <summary>
		/// Logs a formatted Fatal message for the specified category.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="format">A format string.</param>
		/// <param name="args">The parameters to substitute into the format string.</param>
		public static void FatalFormat(Type category, string format, params object[] args)
		{
			string msg = string.Format(format, args);
			Fatal(category, msg);
		}

		#endregion

		#region Public Static "Write" Methods

		/// <summary>
		/// Logs a message with the specified category, level, and data.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="level">The severity level of the message.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		public static void Write(Type category, LogLevel level, object messageData)
		{
			Write(category, level, messageData, null, null);
		}

		/// <summary>
		/// Logs a message with the specified category, level, data, and exception.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="level">The severity level of the message.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		public static void Write(Type category, LogLevel level, object messageData, Exception ex)
		{
			Write(category, level, messageData, ex, null);
		}

		/// <summary>
		/// Logs a message with the specified category, level, data, and additional context properties.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="level">The severity level of the message.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public static void Write(Type category, LogLevel level, object messageData, IDictionary<string, object> contextProperties)
		{
			Write(category, level, messageData, null, contextProperties);
		}

		/// <summary>
		/// Logs a message with the specified category, level, data, exception, and additional context properties.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="level">The severity level of the message.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public static void Write(Type category, LogLevel level, object messageData, Exception ex, IDictionary<string, object> contextProperties)
		{
			Log log = GetLog(category);
			log.Write(level, messageData, ex, contextProperties);
		}

		/// <summary>
		/// Logs a formatted message for the specified category and level.
		/// </summary>
		/// <param name="category">A type whose full name will be used as the log category.  This must be non-null.</param>
		/// <param name="level">The severity level of the message.</param>
		/// <param name="format">A format string.</param>
		/// <param name="args">The parameters to substitute into the format string.</param>
		public static void WriteFormat(Type category, LogLevel level, string format, params object[] args)
		{
			string msg = string.Format(format, args);
			Write(category, level, msg);
		}

		#endregion

		#region Public Instance "Debug" Methods

		/// <summary>
		/// Logs a Debug message with the specified data.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		public void Debug(object messageData)
		{
			this.Debug(messageData, null, null);
		}

		/// <summary>
		/// Logs a Debug message with the specified data and exception.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		public void Debug(object messageData, Exception ex)
		{
			this.Debug(messageData, ex, null);
		}

		/// <summary>
		/// Logs a Debug message with the specified data and additional context properties.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public void Debug(object messageData, IDictionary<string, object> contextProperties)
		{
			this.Debug(messageData, null, contextProperties);
		}

		/// <summary>
		/// Logs a Debug message with the specified data, exception, and additional context properties.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public void Debug(object messageData, Exception ex, IDictionary<string, object> contextProperties)
		{
			this.Write(LogLevel.Debug, messageData, ex, contextProperties);
		}

		/// <summary>
		/// Logs a formatted Debug message.
		/// </summary>
		/// <param name="format">A format string.</param>
		/// <param name="args">The parameters to substitute into the format string.</param>
		public void DebugFormat(string format, params object[] args)
		{
			string msg = string.Format(format, args);
			this.Debug(msg);
		}

		#endregion

		#region Public Instance "Info" Methods

		/// <summary>
		/// Logs an Info message with the specified data.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		public void Info(object messageData)
		{
			this.Info(messageData, null, null);
		}

		/// <summary>
		/// Logs an Info message with the specified data and exception.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		public void Info(object messageData, Exception ex)
		{
			this.Info(messageData, ex, null);
		}

		/// <summary>
		/// Logs an Info message with the specified data and additional context properties.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public void Info(object messageData, IDictionary<string, object> contextProperties)
		{
			this.Info(messageData, null, contextProperties);
		}

		/// <summary>
		/// Logs an Info message with the specified data, exception, and additional context properties.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public void Info(object messageData, Exception ex, IDictionary<string, object> contextProperties)
		{
			this.Write(LogLevel.Info, messageData, ex, contextProperties);
		}

		/// <summary>
		/// Logs a formatted Info message.
		/// </summary>
		/// <param name="format">A format string.</param>
		/// <param name="args">The parameters to substitute into the format string.</param>
		public void InfoFormat(string format, params object[] args)
		{
			string msg = string.Format(format, args);
			this.Info(msg);
		}

		#endregion

		#region Public Instance "Warning" Methods

		/// <summary>
		/// Logs a Warning message with the specified data.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		public void Warning(object messageData)
		{
			this.Warning(messageData, null, null);
		}

		/// <summary>
		/// Logs a Warning message with the specified data and exception.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		public void Warning(object messageData, Exception ex)
		{
			this.Warning(messageData, ex, null);
		}

		/// <summary>
		/// Logs a Warning message with the specified data and additional context properties.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public void Warning(object messageData, IDictionary<string, object> contextProperties)
		{
			this.Warning(messageData, null, contextProperties);
		}

		/// <summary>
		/// Logs a Warning message with the specified data, exception, and additional context properties.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public void Warning(object messageData, Exception ex, IDictionary<string, object> contextProperties)
		{
			this.Write(LogLevel.Warning, messageData, ex, contextProperties);
		}

		/// <summary>
		/// Logs a formatted Warning message.
		/// </summary>
		/// <param name="format">A format string.</param>
		/// <param name="args">The parameters to substitute into the format string.</param>
		public void WarningFormat(string format, params object[] args)
		{
			string msg = string.Format(format, args);
			this.Warning(msg);
		}

		#endregion

		#region Public Instance "Error" Methods

		/// <summary>
		/// Logs an Error message with the specified data.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		public void Error(object messageData)
		{
			this.Error(messageData, null, null);
		}

		/// <summary>
		/// Logs an Error message with the specified data and exception.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		public void Error(object messageData, Exception ex)
		{
			this.Error(messageData, ex, null);
		}

		/// <summary>
		/// Logs an Error message with the specified data and additional context properties.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public void Error(object messageData, IDictionary<string, object> contextProperties)
		{
			this.Error(messageData, null, contextProperties);
		}

		/// <summary>
		/// Logs an Error message with the specified data, exception, and additional context properties.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public void Error(object messageData, Exception ex, IDictionary<string, object> contextProperties)
		{
			this.Write(LogLevel.Error, messageData, ex, contextProperties);
		}

		/// <summary>
		/// Logs a formatted Error message.
		/// </summary>
		/// <param name="format">A format string.</param>
		/// <param name="args">The parameters to substitute into the format string.</param>
		public void ErrorFormat(string format, params object[] args)
		{
			string msg = string.Format(format, args);
			this.Error(msg);
		}

		#endregion

		#region Public Instance "Fatal" Methods

		/// <summary>
		/// Logs a Fatal message with the specified data.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		public void Fatal(object messageData)
		{
			this.Fatal(messageData, null, null);
		}

		/// <summary>
		/// Logs a Fatal message with the specified data and exception.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		public void Fatal(object messageData, Exception ex)
		{
			this.Fatal(messageData, ex, null);
		}

		/// <summary>
		/// Logs a Fatal message with the specified data and additional context properties.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public void Fatal(object messageData, IDictionary<string, object> contextProperties)
		{
			this.Fatal(messageData, null, contextProperties);
		}

		/// <summary>
		/// Logs a Fatal message with the specified data, exception, and additional context properties.
		/// </summary>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public void Fatal(object messageData, Exception ex, IDictionary<string, object> contextProperties)
		{
			this.Write(LogLevel.Fatal, messageData, ex, contextProperties);
		}

		/// <summary>
		/// Logs a formatted Fatal message.
		/// </summary>
		/// <param name="format">A format string.</param>
		/// <param name="args">The parameters to substitute into the format string.</param>
		public void FatalFormat(string format, params object[] args)
		{
			string msg = string.Format(format, args);
			this.Fatal(msg);
		}

		#endregion

		#region Public Instance "Write" Methods

		/// <summary>
		/// Logs a message with the specified level and data.
		/// </summary>
		/// <param name="level">The severity level of the message.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		public void Write(LogLevel level, object messageData)
		{
			this.Write(level, messageData, null, null);
		}

		/// <summary>
		/// Logs a message with the specified level, data, and exception.
		/// </summary>
		/// <param name="level">The severity level of the message.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		public void Write(LogLevel level, object messageData, Exception ex)
		{
			this.Write(level, messageData, ex, null);
		}

		/// <summary>
		/// Logs a message with the specified level, data, and additional context properties.
		/// </summary>
		/// <param name="level">The severity level of the message.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		public void Write(LogLevel level, object messageData, IDictionary<string, object> contextProperties)
		{
			this.Write(level, messageData, null, contextProperties);
		}

		/// <summary>
		/// Logs a message with the specified level, data, exception, and additional context properties.
		/// </summary>
		/// <param name="level">The severity level of the message.</param>
		/// <param name="messageData">The message data.  This can be null.  If specified, this will be
		/// rendered as text if necessary.</param>
		/// <param name="ex">The exception associated with the message.  This can be null.</param>
		/// <param name="contextProperties">Message-specific context properties that will be
		/// logged and merged with the <see cref="GlobalContext"/> and <see cref="ThreadContext"/>
		/// properties.  This can be null.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "This is a core logging method, which most catch blocks call back in to.")]
		public void Write(LogLevel level, object messageData, Exception ex, IDictionary<string, object> contextProperties)
		{
			try
			{
				TraceEventType eventType;
				if (this.IsWriteEnabled(level, out eventType))
				{
					Dictionary<string, object> eventProperties = GetEventProperties(contextProperties);

					// For errors, try to add diagnostic info like the calling method or the full stack trace.
					AddSourceProperty(level, eventProperties);

					// Now merge in any context-level properties.
					if (contextProperties != null)
					{
						foreach (var pair in contextProperties)
						{
							eventProperties[pair.Key] = pair.Value;
						}
					}

					// See if anything has specified a custom event ID.
					int eventId = GetEventId(eventProperties);

					object[] eventData = GetEventData(messageData, ex, eventProperties);
					this.traceSource.TraceData(eventType, eventId, eventData);
				}
			}
			catch (Exception newException)
			{
				// We can't let exceptions propagate out of the logging methods
				// because most catches will immediately call back into the logging
				// methods.
				Debugger.Log((int)LogLevel.Error, this.category, newException.ToString());
			}
		}

		/// <summary>
		/// Logs a formatted message with the specified level.
		/// </summary>
		/// <param name="level">The severity level of the message.</param>
		/// <param name="format">A format string.</param>
		/// <param name="args">The parameters to substitute into the format string.</param>
		public void WriteFormat(LogLevel level, string format, params object[] args)
		{
			string msg = string.Format(format, args);
			this.Write(level, msg);
		}

		/// <summary>
		/// Gets whether the specified logging level is enabled for the current log category.
		/// </summary>
		public bool IsWriteEnabled(LogLevel level)
		{
			TraceEventType eventType;
			bool result = this.IsWriteEnabled(level, out eventType);
			return result;
		}

		#endregion

		#region Private Methods

		private static Log InternalGetLog(string category)
		{
			// .NET's ConcurrentDictionary class handles thread-safety here.
			// The GetOrAdd method will either return an existing Log instance
			// or add a new one using the factory function we provided.
			Log result = LogCache.GetOrAdd(category, n => new Log(category));
			return result;
		}

		private static void AddSourceProperty(LogLevel level, IDictionary<string, object> properties)
		{
			// For fatal errors, try to report the entire stack trace.
			// For normal errors, try to report the source method name.
			// For other levels don't do anything because it's slow to walk up the stack.
			switch (level)
			{
				case LogLevel.Fatal:
					string stackTrace = StackTraceUtility.CaptureSourceStackTrace(string.Empty);
					if (!string.IsNullOrEmpty(stackTrace))
					{
						properties["Call Stack"] = stackTrace;
					}

					break;

				case LogLevel.Error:
					string method = StackTraceUtility.FindSourceMethodName();
					if (!string.IsNullOrEmpty(method))
					{
						properties["Source Method"] = method;
					}

					break;
			}
		}

		private static Dictionary<string, object> GetEventProperties(IDictionary<string, object> contextProperties)
		{
			Dictionary<string, object> entryProperties;
			if (contextProperties != null)
			{
				const int AllowanceForGlobalAndThreadEntries = 4;
				entryProperties = new Dictionary<string, object>(contextProperties.Count + AllowanceForGlobalAndThreadEntries);
			}
			else
			{
				entryProperties = new Dictionary<string, object>();
			}

			GlobalContextValue.MergeEntriesInto(entryProperties);
			ThreadContextValue.MergeEntriesInto(entryProperties);

			return entryProperties;
		}

		private static int GetEventId(IDictionary<string, object> eventProperties)
		{
			int result = 0;

			object eventIdValue;
			if (eventProperties.TryGetValue(EventIdPropertyName, out eventIdValue))
			{
				try
				{
					result = (int)eventIdValue;
					eventProperties.Remove(EventIdPropertyName);
				}
				catch (InvalidCastException)
				{
					// If it's not an int, then it's a valid event ID.
				}
			}

			return result;
		}

		private static object[] GetEventData(object messageData, Exception ex, IDictionary<string, object> eventProperties)
		{
			List<object> result = new List<object>();

			if (messageData != null)
			{
				result.Add(messageData);
			}

			if (ex != null)
			{
				result.Add(ex);
			}

			if (eventProperties != null)
			{
				const string Prefix = "{";
				StringBuilder sb = new StringBuilder(Prefix);
				foreach (var pair in eventProperties)
				{
					if (sb.Length > Prefix.Length)
					{
						sb.Append(", ");
					}

					sb.AppendFormat("{0}={1}", pair.Key, pair.Value);
				}

				sb.Append("}");
				result.Add(sb.ToString());
			}

			return result.ToArray();
		}

		private bool IsWriteEnabled(LogLevel level, out TraceEventType eventType)
		{
			switch (level)
			{
				case LogLevel.Debug:
					eventType = TraceEventType.Verbose;
					break;

				case LogLevel.Error:
					eventType = TraceEventType.Error;
					break;

				case LogLevel.Fatal:
					eventType = TraceEventType.Critical;
					break;

				case LogLevel.Info:
					eventType = TraceEventType.Information;
					break;

				case LogLevel.Warning:
					eventType = TraceEventType.Warning;
					break;

				default: // None
					// TraceEventType doesn't define a "None" field or a named 0-valued field,
					// so we have to refer to it using C#'s default keyword.
					eventType = default(TraceEventType);
					break;
			}

			// TraceEventType's Critical = 1, Error = 2, Warning = 4, etc.
			bool result = false;
			if (eventType >= TraceEventType.Critical)
			{
				result = this.traceSource.Switch.ShouldTrace(eventType);
			}

			return result;
		}

		#endregion
	}
}
