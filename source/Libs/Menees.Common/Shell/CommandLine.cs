namespace Menees.Shell
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Text;

	#endregion

	/// <summary>
	/// A general-purpose processor for command line arguments.
	/// </summary>
	/// <remarks>
	/// This can be used from console applications, windows applications, and windows services.  It supports:
	/// <list type="bullet">
	/// <item><description>Flag switches (e.g., /b for true, /b- for false)</description></item>
	/// <item><description>Associated value switches (e.g., /k:v, /k=v, /k v) with custom validation</description></item>
	/// <item><description>Non-switched value arguments (e.g., source and destination for XCopy) with custom validation</description></item>
	/// <item><description>Post-parsing custom validation logic</description></item>
	/// <item><description>Using lambda expressions or anonymous delegates to define argument handling actions</description></item>
	/// <item><description>Switches with multiple aliases (e.g., /Help and /?)</description></item>
	/// <item><description>Switches that are required</description></item>
	/// <item><description>Switches that allow multiple usage (e.g., specifying /target: multiple times)</description></item>
	/// <item><description>Switch prefixes of '/' and '-'</description></item>
	/// <item><description>Partial matching of switch names (e.g., /b for /binary if it is unambiguous)</description></item>
	/// <item><description>Automatic generation of a formatted help message</description></item>
	/// <item><description>Special handling for formatted output when the <see cref="Console"/> is being used</description></item>
	/// </list>
	/// <example>
	/// How to use a CommandLine instance to process a collection of arguments.
	/// <code>
	/// bool isBinary = false;
	/// string source = null;
	/// List&lt;string> targets = new List&lt;string>();
	///
	/// CommandLine cmdLine = new CommandLine(useConsole: true);
	/// cmdLine.AddHeader(string.Format(CultureInfo.CurrentCulture, "Usage: {0} [/B] source /T:target [/T:...]", CommandLine.ExecutableFileName));
	/// cmdLine.AddSwitch("Binary", "Indicates a binary file (or a text file if the flag is disabled).", flag => isBinary = flag);
	/// cmdLine.AddSwitch("Target", "A target location to copy the source into.  Multiple target locations can be specified.",
	///     (value, errors) =>
	///     {
	///         if (!Directory.Exists(value))
	///         {
	///             errors.Add("Target location does not exist: " + value);
	///         }
	///         else
	///         {
	///             targets.Add(value);
	///         }
	///     },
	///     CommandLineSwitchOptions.Required | CommandLineSwitchOptions.AllowMultiple);
	/// cmdLine.AddValueHandler((value, errors) =>
	///     {
	///         if (!string.IsNullOrEmpty(source))
	///         {
	///             errors.Add("Only a single source file can be specified.");
	///         }
	///         else if (!File.Exists(value))
	///         {
	///             errors.Add("Source does not exist: " + value);
	///         }
	///         else
	///         {
	///             source = value;
	///         }
	///     });
	/// cmdLine.AddFinalValidation(errors =>
	///     {
	///         // Because we have a required, unnamed arg (source), we have to manually check for it.
	///         if (string.IsNullOrEmpty(source))
	///         {
	///             errors.Add("A source file is required.");
	///         }
	///     });
	///
	/// var parseResult = cmdLine.Parse(args);
	/// if (parseResult != CommandLineParseResult.Valid)
	/// {
	///     // Let it write out the help or error message to the console.
	///     cmdLine.WriteMessage();
	/// }
	/// else
	/// {
	///     // It parsed successfully, so use the options.
	/// }
	/// </code>
	/// </example>
	/// </remarks>
	public sealed class CommandLine
	{
		#region Public Constants

		/// <summary>
		/// Gets the maximum length of a command line on the current version of Windows.
		/// </summary>
		/// <remarks>
		/// On Windows Vista and earlier, the length of the arguments plus the length of the full path to the
		/// executable must be less than 2080.  On Windows 7 and later, the length must be less than 32699.
		/// See http://msdn.microsoft.com/en-us/library/system.diagnostics.processstartinfo.arguments.aspx
		/// </remarks>
		public static readonly int MaxLength = Environment.OSVersion.Version >= new Version(6, 1) ? 32698 : 2079;

		#endregion

		#region Private Data Members

		private static readonly char[] SwitchPrefixes = new[] { '/', '-' };
		private static readonly char[] SwitchValueSeparators = new[] { ':', '=' };
		private static readonly char[] QuotableCharacters = new[] { ' ', '\t', '\r', '\n', '\v', '"' };

		private bool useConsole;
		private Dictionary<string, Switch> nameToSwitchMap;
		private List<Switch> distinctSwitches;
		private Action<string, IList<string>> valueValidationHandler;
		private List<KeyValuePair<string, string>> argHelp;
		private Action<IList<string>> finalValidationHandler;
		private List<string> errors;
		private State currentState;
		private List<string> header;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance with <see cref="UseConsole"/> set based on whether
		/// the current process can interact with the user and using the current culture's
		/// case-insensitive comparer.
		/// </summary>
		public CommandLine()
			: this(Environment.UserInteractive)
		{
		}

		/// <summary>
		/// Creates a new instance using the specified <see cref="UseConsole"/> value
		/// and using the current culture's case-insensitive comparer.
		/// </summary>
		/// <param name="useConsole">The value for the <see cref="UseConsole"/> property.</param>
		public CommandLine(bool useConsole)
			: this(useConsole, StringComparer.CurrentCultureIgnoreCase)
		{
		}

		/// <summary>
		/// Creates a new instance using the specified <see cref="UseConsole"/> value
		/// and using the specified comparer.
		/// </summary>
		/// <param name="useConsole">The value for the <see cref="UseConsole"/> property.</param>
		/// <param name="comparer">The comparer to use with matching switches.</param>
		public CommandLine(bool useConsole, StringComparer comparer)
		{
			this.useConsole = useConsole;
			this.nameToSwitchMap = new Dictionary<string, Switch>(comparer);
			this.distinctSwitches = new List<Switch>();
		}

		#endregion

		#region Private Enums

		#region enum State

		private enum State
		{
			Created,
			Configured,
			Parsed,
			Invalid,

			// Special state because if help is requested, programs usually don't need to continue.
			// It's higher priority than Invalid because if a user explicitly requests help, they probably
			// don't know the required args and other valid syntax, so there's no point in showing
			// them errors about it.
			HelpRequested,
		}

		#endregion

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the name of the .exe file (without the path) that started the current process.
		/// </summary>
		public static string ExecutableFileName
		{
			get
			{
				// See comments in the Arguments property about why we don't trust
				// Environment.CommandLine or Environment.GetCommandLineArgs().
				string result = Path.GetFileName(ApplicationInfo.ExecutableFile);
				return result;
			}
		}

		/// <summary>
		/// Gets the command line arguments that were passed to the current process
		/// (not including the <see cref="ExecutableFileName"/>).
		/// </summary>
		public static IEnumerable<string> Arguments
		{
			get
			{
				// In VS 2015 and/or .NET 4.6, sometimes the unit test process will be launched with a command line that
				// doesn't quote the path correctly, and then Environment.GetCommandLineArgs() can't parse it correctly.
				// It will report separate args like "C:\Program", "Files", etc.  To work around that, we'll try to manually
				// handle it smarter since the main module provides the full file name.
				string commandLine = Environment.CommandLine;
				string executable = ApplicationInfo.ExecutableFile;
				if (commandLine.StartsWith(executable) && executable.Contains(' '))
				{
					// Force the executable path to be quoted, so the standard Windows command line parser can handle it.
					commandLine = TextUtility.EnsureQuotes(executable) + commandLine.Substring(executable.Length);
				}

				// The first element is the executable file name, so skip it.
				IEnumerable<string> result = Split(commandLine, false);
				return result;
			}
		}

		/// <summary>
		/// Gets whether the <see cref="Console"/> should be used to determine
		/// <see cref="WriteMessage(TextWriter, CommandLineWriteOptions)"/>
		/// output width and as the default target for writing.
		/// </summary>
		public bool UseConsole => this.useConsole;

		/// <summary>
		/// Gets the comparer used to check switches for equality.
		/// </summary>
		public IEqualityComparer<string> Comparer => this.nameToSwitchMap.Comparer;

		#endregion

		#region Public Generic Parsing Methods

		/// <summary>
		/// Parses the given collection of command line arguments into a list of values and a dictionary of switches.
		/// </summary>
		/// <remarks>
		/// This method doesn't do any validation of the arguments.  It only categorizes them as values or switches
		/// (i.e., unnamed or named arguments).
		/// </remarks>
		/// <param name="args">The arguments to parse</param>
		/// <param name="values">The collection to add "unnamed" values to.</param>
		/// <param name="switches">The collection to add "named" switch key/value pairs to.</param>
		public static void Parse(IEnumerable<string> args, IList<string> values, IDictionary<string, string> switches)
		{
			Conditions.RequireReference(args, "args");
			Conditions.RequireReference(values, "values");
			Conditions.RequireReference(switches, "switches");

			string currentName = null;
			foreach (string rawArg in args)
			{
				string arg = (rawArg ?? string.Empty).Trim();

				// See if they've given us a switch.
				char start = arg[0];
				if (SwitchPrefixes.Contains(start))
				{
					// If we currently have a switch pending, then we need to add it.  That means we have a
					// named argument that has no associated value.  So it's just a switch.
					if (currentName != null)
					{
						switches[currentName] = string.Empty;
					}

					// Strip off the prefix character.
					currentName = arg.Substring(1);

					// Ignore an arg that's just "/" or "-" followed by whitespace.
					if (string.IsNullOrWhiteSpace(currentName))
					{
						currentName = null;
					}

					// If the arg is like n:v or n=v, then we can split and add it now.
					if (HandleCombinedNameValuePair(currentName, switches))
					{
						currentName = null;
					}
				}
				else
				{
					// If we have a current name, then this argument
					// is the associated value.  Otherwise, we just
					// have an unnamed argument.
					if (currentName != null)
					{
						switches[currentName] = arg;
						currentName = null;
					}
					else
					{
						values.Add(arg);
					}
				}
			}

			// If we finished with a current name pending, then it's
			// a named argument with no associated value.
			if (currentName != null)
			{
				switches[currentName] = string.Empty;
			}
		}

		/// <summary>
		/// Splits a full command line (including the program name) into a collection of arguments.
		/// </summary>
		/// <param name="commandLine">A full command line where the program name is the first
		/// argument.  This can be obtained from Windows's GetCommandLineW API or from WMI's
		/// Win32_Process class's CommandLine field.
		/// </param>
		/// <param name="includeProgramName">Whether the program name should be included
		/// in the result collection.  Typically, this should be false since none of the other
		/// <see cref="CommandLine"/> methods expect it.</param>
		/// <returns>A collection of arguments that optionally includes the program name.</returns>
		public static IEnumerable<string> Split(string commandLine, bool includeProgramName)
		{
			IEnumerable<string> result = null;

			if (!string.IsNullOrWhiteSpace(commandLine))
			{
				result = NativeMethods.SplitCommandLine(commandLine);
				if (!includeProgramName)
				{
					result = result.Skip(1);
				}
			}

			return result ?? Enumerable.Empty<string>();
		}

		#endregion

		#region Public Generic Building Methods

		/// <summary>
		/// Encodes a value using command line quoting and escaping rules.
		/// </summary>
		/// <param name="value">The value to encode.</param>
		/// <returns>The encoded value.</returns>
		public static string EncodeValue(object value)
		{
			string result = null;

			if (value != null)
			{
				result = Convert.ToString(value);
				if (!string.IsNullOrEmpty(result))
				{
					StringBuilder builder = new StringBuilder(result.Length);
					AppendValue(builder, result);
					result = builder.ToString();
				}
			}

			return result;
		}

		/// <summary>
		/// Encodes a name/value switch using command line quoting and escaping rules.
		/// </summary>
		/// <param name="name">The name of the switch.  This must be non-empty.
		/// If it doesn't begin with a switch prefix character, then one will be added.</param>
		/// <param name="value">The value for the switch.  This can be null.</param>
		/// <returns>The encoded name and value.</returns>
		public static string EncodeSwitch(string name, object value)
		{
			Conditions.RequireString(name, "name");

			StringBuilder builder = new StringBuilder(2 * name.Length);
			AppendSwitch(builder, name, value);
			return builder.ToString();
		}

		/// <summary>
		/// Builds a command line after encoding each argument using the proper quoting and escaping rules.
		/// </summary>
		/// <param name="arguments">The arguments to encode.  Tuple&lt;string, object> and
		/// KeyValuePair&lt;string, object> arguments will be encoded using <see cref="EncodeSwitch"/>.
		/// All other arguments will be encoded using <see cref="EncodeValue"/>.</param>
		/// <returns>The command line with space-separated arguments properly encoded.</returns>
		public static string Build(params object[] arguments)
		{
			string result = null;

			if (arguments != null && arguments.Length > 0)
			{
				StringBuilder builder = new StringBuilder();
				foreach (object arg in arguments)
				{
					string name = null;
					object value = arg;

					KeyValuePair<string, object>? pair = arg as KeyValuePair<string, object>?;
					if (pair != null)
					{
						name = pair.Value.Key;
						value = pair.Value.Value;
					}
					else
					{
						Tuple<string, object> tuple = arg as Tuple<string, object>;
						if (tuple != null)
						{
							name = tuple.Item1;
							value = tuple.Item2;
						}
					}

					if (!string.IsNullOrEmpty(name))
					{
						if (builder.Length > 0)
						{
							builder.Append(' ');
						}

						AppendSwitch(builder, name, value);
					}
					else
					{
						string textValue = Convert.ToString(value);
						if (!string.IsNullOrEmpty(textValue))
						{
							if (builder.Length > 0)
							{
								builder.Append(' ');
							}

							AppendValue(builder, textValue);
						}
					}
				}

				result = builder.ToString();
			}

			return result;
		}

		#endregion

		#region Public Configuration Methods

		/// <summary>
		/// Adds a flag switch.
		/// </summary>
		/// <param name="name">The name of the switch.</param>
		/// <param name="description">The description of the switch to display in the generated help.</param>
		/// <param name="handler">The action to execute to handle the switch value.</param>
		/// <returns>The current instance.</returns>
		public CommandLine AddSwitch(string name, string description, Action<bool> handler)
			=> this.InternalAddSwitch(new[] { name }, description, handler, CommandLineSwitchOptions.None);

		/// <summary>
		/// Adds a flag switch using the specified options.
		/// </summary>
		/// <param name="name">The name of the switch.</param>
		/// <param name="description">The description of the switch to display in the generated help.</param>
		/// <param name="handler">The action to execute to handle the switch value.</param>
		/// <param name="options">Options that affect the switch's behavior.</param>
		/// <returns>The current instance.</returns>
		public CommandLine AddSwitch(string name, string description, Action<bool> handler, CommandLineSwitchOptions options)
			=> this.InternalAddSwitch(new[] { name }, description, handler, options);

		/// <summary>
		/// Adds a flag switch with multiple names.
		/// </summary>
		/// <param name="names">The valid names (i.e., aliases) of the switch.</param>
		/// <param name="description">The description of the switch to display in the generated help.</param>
		/// <param name="handler">The action to execute to handle the switch value.</param>
		/// <returns>The current instance.</returns>
		public CommandLine AddSwitch(string[] names, string description, Action<bool> handler)
			=> this.InternalAddSwitch(names, description, handler, CommandLineSwitchOptions.None);

		/// <summary>
		/// Adds a flag switch with multiple names using the specified options.
		/// </summary>
		/// <param name="names">The valid names (i.e., aliases) of the switch.</param>
		/// <param name="description">The description of the switch to display in the generated help.</param>
		/// <param name="handler">The action to execute to handle the switch value.</param>
		/// <param name="options">Options that affect the switch's behavior.</param>
		/// <returns>The current instance.</returns>
		public CommandLine AddSwitch(string[] names, string description, Action<bool> handler, CommandLineSwitchOptions options)
			=> this.InternalAddSwitch(names, description, handler, options);

		/// <summary>
		/// Adds an associated value switch.
		/// </summary>
		/// <param name="name">The name of the switch.</param>
		/// <param name="description">The description of the switch to display in the generated help.</param>
		/// <param name="handler">The action to execute to handle the switch value.</param>
		/// <returns>The current instance.</returns>
		[SuppressMessage(
			"Microsoft.Design",
			"CA1006:DoNotNestGenericTypesInMemberSignatures",
			Justification = "The caller doesn't instantiate an Action and doesn't declare the argument types with lambda expressions.")]
		public CommandLine AddSwitch(string name, string description, Action<string, IList<string>> handler)
			=> this.InternalAddSwitch(new[] { name }, description, handler, CommandLineSwitchOptions.None);

		/// <summary>
		/// Adds an associated value switch using the specified options.
		/// </summary>
		/// <param name="name">The name of the switch.</param>
		/// <param name="description">The description of the switch to display in the generated help.</param>
		/// <param name="handler">The action to execute to handle the switch value.</param>
		/// <param name="options">Options that affect the switch's behavior.</param>
		/// <returns>The current instance.</returns>
		[SuppressMessage(
			"Microsoft.Design",
			"CA1006:DoNotNestGenericTypesInMemberSignatures",
			Justification = "The caller doesn't instantiate an Action and doesn't declare the argument types with lambda expressions.")]
		public CommandLine AddSwitch(string name, string description, Action<string, IList<string>> handler, CommandLineSwitchOptions options)
			=> this.InternalAddSwitch(new[] { name }, description, handler, options);

		/// <summary>
		/// Adds an associated value switch with multiple names.
		/// </summary>
		/// <param name="names">The valid names (i.e., aliases) of the switch.</param>
		/// <param name="description">The description of the switch to display in the generated help.</param>
		/// <param name="handler">The action to execute to handle the switch value.</param>
		/// <returns>The current instance.</returns>
		[SuppressMessage(
			"Microsoft.Design",
			"CA1006:DoNotNestGenericTypesInMemberSignatures",
			Justification = "The caller doesn't instantiate an Action and doesn't declare the argument types with lambda expressions.")]
		public CommandLine AddSwitch(string[] names, string description, Action<string, IList<string>> handler)
			=> this.InternalAddSwitch(names, description, handler, CommandLineSwitchOptions.None);

		/// <summary>
		/// Adds an associated value switch with multiple names using the specified options.
		/// </summary>
		/// <param name="names">The valid names (i.e., aliases) of the switch.</param>
		/// <param name="description">The description of the switch to display in the generated help.</param>
		/// <param name="handler">The action to execute to handle the switch value.</param>
		/// <param name="options">Options that affect the switch's behavior.</param>
		/// <returns>The current instance.</returns>
		[SuppressMessage(
			"Microsoft.Design",
			"CA1006:DoNotNestGenericTypesInMemberSignatures",
			Justification = "The caller doesn't instantiate an Action and doesn't declare the argument types with lambda expressions.")]
		public CommandLine AddSwitch(string[] names, string description, Action<string, IList<string>> handler, CommandLineSwitchOptions options)
			=> this.InternalAddSwitch(names, description, handler, options);

		/// <summary>
		/// Adds one or more header lines to the output of <see cref="WriteMessage(TextWriter, CommandLineWriteOptions)"/>.
		/// </summary>
		/// <param name="lines">The lines to add.</param>
		/// <returns>The current instance.</returns>
		public CommandLine AddHeader(params string[] lines)
		{
			// Note: I'm not calling RequireUnparsed because they can add header lines
			// after parsing if they want to (e.g., only if help needs to be displayed).
			if (this.header == null)
			{
				this.header = new List<string>();
			}

			this.header.AddRange(lines);
			return this;
		}

		/// <summary>
		/// Adds the handler for command line values that are not associated with a switch.
		/// </summary>
		/// <param name="handler">The action to execute to handle the value.</param>
		/// <param name="valueHelp">Value names and descriptions that should appear in the generated help message.</param>
		/// <returns>The current instance.</returns>
		[SuppressMessage(
			"Microsoft.Design",
			"CA1006:DoNotNestGenericTypesInMemberSignatures",
			Justification = "The caller doesn't instantiate an Action and doesn't declare the argument types with lambda expressions.")]
		public CommandLine AddValueHandler(Action<string, IList<string>> handler, params KeyValuePair<string, string>[] valueHelp)
		{
			this.RequireUnparsed();
			Conditions.RequireReference(handler, "handler");
			Conditions.RequireState(this.valueValidationHandler == null, "Only one extra argument handler can be added.");

			this.valueValidationHandler = handler;
			if (valueHelp != null)
			{
				foreach (var pair in valueHelp)
				{
					this.AddArgHelp(pair);
				}
			}

			this.currentState = State.Configured;
			return this;
		}

		/// <summary>
		/// Adds a handler to be called after <see cref="Parse(IEnumerable{string})"/> has finished all arguments.
		/// </summary>
		/// <param name="handler">The action to execute to perform final validation.</param>
		/// <returns>The current instance.</returns>
		[SuppressMessage(
			"Microsoft.Design",
			"CA1006:DoNotNestGenericTypesInMemberSignatures",
			Justification = "The caller doesn't instantiate an Action and doesn't declare the argument types with lambda expressions.")]
		public CommandLine AddFinalValidation(Action<IList<string>> handler)
		{
			this.RequireUnparsed();
			Conditions.RequireReference(handler, "handler");
			Conditions.RequireState(this.finalValidationHandler == null, "Only one final validation handler can be added.");

			this.finalValidationHandler = handler;
			this.currentState = State.Configured;
			return this;
		}

		#endregion

		#region Public Parsing Methods

		/// <summary>
		/// Parses the command line <see cref="Arguments"/> for the current process.
		/// </summary>
		/// <returns>The parsing result state.</returns>
		public CommandLineParseResult Parse() => this.Parse(Arguments);

		/// <summary>
		/// Parses the given collection of command line arguments.
		/// </summary>
		/// <returns>The parsing result state.</returns>
		public CommandLineParseResult Parse(IEnumerable<string> args)
		{
			this.RequireUnparsed();
			Conditions.RequireReference(args, "args");

			// Automatically support help.
			if (!this.nameToSwitchMap.ContainsKey("?"))
			{
				// Use a blank description, so this switch won't show up in the help message.
				this.AddSwitch(new[] { "?", "help" }, string.Empty, value => { this.currentState = State.HelpRequested; });
			}

			// Process the arguments we were given.
			this.ParseArguments(args);

			// Validate that all required switches were used.
			var missedSwitches = from sw in this.distinctSwitches
								where sw.IsRequired && sw.UsageCount == 0
								select sw.Names.First();
			foreach (string missedSwitch in missedSwitches)
			{
				this.AddError("/{0} is required.", missedSwitch);
			}

			// Give the caller a chance to perform cross-argument validation
			// (e.g., to look for required unnamed args, or to say switch /X is
			// only valid if /Y also appears)
			if (this.finalValidationHandler != null)
			{
				List<string> errors = new List<string>(0);
				this.finalValidationHandler(errors);
				this.AddErrors(errors);
			}

			// Go to the Parsed state if we're not already in HelpRequested or Invalid.
			if (this.currentState < State.Parsed)
			{
				this.currentState = State.Parsed;
			}

			// Give the caller some detail about our internal state.
			CommandLineParseResult result = CommandLineParseResult.Valid;
			switch (this.currentState)
			{
				case State.Invalid:
					result = CommandLineParseResult.Invalid;
					break;

				case State.HelpRequested:
					result = CommandLineParseResult.HelpRequested;
					break;
			}

			return result;
		}

		#endregion

		#region Public Post-Parsing Methods

		/// <summary>
		/// Calls <see cref="WriteMessage(TextWriter)"/> and returns the written text.
		/// </summary>
		/// <returns>The message written by <see cref="WriteMessage(TextWriter)"/>.</returns>
		public string CreateMessage()
		{
			using (StringWriter writer = new StringWriter())
			{
				this.WriteMessage(writer);
				string result = writer.ToString();
				return result;
			}
		}

		/// <summary>
		/// Writes the relevant message sections based on the last
		/// <see cref="Parse(IEnumerable{string})"/> result to the
		/// <see cref="Console"/>'s output or error stream.
		/// </summary>
		public void WriteMessage()
		{
			Conditions.RequireState(this.UseConsole, "UseConsole is false, so you must provide a TextWriter.");

			TextWriter writer = this.currentState == State.Invalid ? Console.Error : Console.Out;
			this.WriteMessage(writer);
		}

		/// <summary>
		/// Writes the relevant message sections based on the last
		/// <see cref="Parse(IEnumerable{string})"/> result to the
		/// specified writer.
		/// </summary>
		/// <param name="writer">The target to write to.</param>
		public void WriteMessage(TextWriter writer)
		{
			CommandLineWriteOptions options = CommandLineWriteOptions.None;

			if (this.header != null)
			{
				options |= CommandLineWriteOptions.Header;
			}

			if (this.currentState == State.Invalid)
			{
				options |= CommandLineWriteOptions.Error;
			}

			if (this.currentState == State.HelpRequested)
			{
				options |= CommandLineWriteOptions.Help;
			}

			this.WriteMessage(writer, options);
		}

		/// <summary>
		/// Writes one or more message sections to the specified writer.
		/// </summary>
		/// <param name="writer">The target to write to.</param>
		/// <param name="options">Determines which message sections to write.</param>
		public void WriteMessage(TextWriter writer, CommandLineWriteOptions options)
		{
			Conditions.RequireReference(writer, "writer");

			int windowWidth = this.UseConsole ? Console.WindowWidth : int.MaxValue;
			bool wroteText = false;

			if (options.HasFlag(CommandLineWriteOptions.Header) && this.header != null)
			{
				foreach (string line in this.header)
				{
					this.WriteLine(writer, line, windowWidth);
					wroteText = true;
				}
			}

			if (options.HasFlag(CommandLineWriteOptions.Help) && this.argHelp != null)
			{
				if (wroteText)
				{
					writer.WriteLine();
				}

				int maxDisplayNameLength = this.argHelp.Max(pair => pair.Key.Length);
				const string NamePadding = "  ";
				string displayNameFormat = "{0}{1,-" + maxDisplayNameLength + "}{0}";

				// Add space for padding on both sides.
				int leftMargin = maxDisplayNameLength + (2 * NamePadding.Length);

				foreach (var pair in this.argHelp)
				{
					writer.Write(displayNameFormat, NamePadding, pair.Key);
					this.WriteLine(writer, pair.Value, windowWidth, leftMargin);
					wroteText = true;
				}
			}

			// Show errors after showing help so they'll be at the end of the console output
			// and more easily visible in case the content scrolls.
			if (options.HasFlag(CommandLineWriteOptions.Error) && this.currentState == State.Invalid && this.errors != null)
			{
				if (wroteText)
				{
					writer.WriteLine();
				}

				foreach (string error in this.errors.Distinct(this.nameToSwitchMap.Comparer))
				{
					this.WriteLine(writer, error, windowWidth);
				}
			}
		}

		#endregion

		#region Private Methods

		private static List<string> WrapText(string text, int maxSegmentLength, string newLine)
		{
			// First, split text into lines based on existing hard line breaks.
			List<string> originalLines = new List<string>();
			int startPos = 0;
			while (startPos < text.Length)
			{
				int lineBreak = text.IndexOf(newLine, startPos, StringComparison.Ordinal);
				if (lineBreak >= 0)
				{
					originalLines.Add(text.Substring(startPos, lineBreak - startPos));
					startPos = lineBreak + newLine.Length;
				}
				else
				{
					originalLines.Add(text.Substring(startPos));
					startPos = text.Length;
				}
			}

			// Now, split each line into "words" and then wrap them appropriately.
			List<string> result = new List<string>();
			foreach (string originalLine in originalLines)
			{
				List<string> words = new List<string>();
				int wordStart = 0;
				for (int i = 0; i < originalLine.Length; i++)
				{
					// We have to handle long "words" (e.g., paths) that are bigger than the max width.
					if (char.IsWhiteSpace(originalLine[i]) || (i - wordStart) > maxSegmentLength)
					{
						words.Add(originalLine.Substring(wordStart, i - wordStart));
						wordStart = i;
					}
				}

				words.Add(originalLine.Substring(wordStart, originalLine.Length - wordStart));

				// Trim off leading whitespace from words and remove empty entries.
				words = words.Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();

				StringBuilder currentLine = new StringBuilder();
				foreach (string word in words)
				{
					// We have to add 1 to allow for the space we want to append before the word.
					if ((word.Length + 1) > (maxSegmentLength - currentLine.Length) && currentLine.Length > 0)
					{
						result.Add(currentLine.ToString());
						currentLine.Length = 0;
					}

					if (currentLine.Length > 0)
					{
						// This is a minor problem.  Instead of always using a space, I should actually use
						// whatever whitespace character originally followed this word.  But that's more to
						// keep up with, and this works well enough for typical console output messages.
						// If this were a general case word wrapper, I'd have to handle it though.
						currentLine.Append(' ');
					}

					currentLine.Append(word);
				}

				if (currentLine.Length > 0)
				{
					result.Add(currentLine.ToString());
				}
			}

			return result;
		}

		private static bool HandleCombinedNameValuePair(string arg, IDictionary<string, string> switches)
		{
			bool result = false;

			int separatorIndex = arg.IndexOfAny(SwitchValueSeparators);
			if (separatorIndex > 0 && (separatorIndex + 1) < arg.Length)
			{
				string name = arg.Substring(0, separatorIndex);
				string value = arg.Substring(separatorIndex + 1);
				switches[name] = value;
				result = true;
			}

			return result;
		}

		private static void AppendValue(StringBuilder builder, string value)
		{
			// All callers should have enforced this already.
			Debug.Assert(!string.IsNullOrEmpty(value), "The value must be non-empty.");

			// This logic came from "Everyone quotes command line arguments the wrong way"
			// http://blogs.msdn.com/b/twistylittlepassagesallalike/archive/2011/04/23/everyone-quotes-arguments-the-wrong-way.aspx
			// For a shorter description of the same logic, see "Parsing C++ Command-Line Arguments"
			// http://msdn.microsoft.com/en-us/library/17w5ykft.aspx
			//
			// Don't quote the value unless we have to.
			if (value.IndexOfAny(QuotableCharacters) < 0)
			{
				builder.Append(value);
			}
			else
			{
				builder.Append('"');

				int length = value.Length;
				for (int index = 0; /* The exit condition is checked inside the loop because we also modify the loop index inside the loop. */; index++)
				{
					int numberBackslashes = 0;
					while (index < length && value[index] == '\\')
					{
						index++;
						numberBackslashes++;
					}

					if (index >= length)
					{
						// Escape all backslashes, but let the terminating double quote
						// we add below be interpreted as a metacharacter.
						builder.Append('\\', 2 * numberBackslashes);
						break;
					}
					else if (value[index] == '"')
					{
						// Escape all backslashes and the following double quotation mark.
						builder.Append('\\', (2 * numberBackslashes) + 1);
						builder.Append('"');
					}
					else
					{
						// Backslashes aren't special here.
						builder.Append('\\', numberBackslashes);
						builder.Append(value[index]);
					}
				}

				builder.Append('"');
			}
		}

		private static void AppendSwitch(StringBuilder builder, string name, object value)
		{
			// All callers should have enforced this already, so we can safely check name[0].
			Debug.Assert(!string.IsNullOrEmpty(name), "Name must be non-empty.");

			if (!SwitchPrefixes.Contains(name[0]))
			{
				Conditions.RequireArgument(!string.IsNullOrWhiteSpace(name), "The switch name cannot be all whitespace.");
				builder.Append(SwitchPrefixes[0]);
			}
			else
			{
				Conditions.RequireArgument(
					name.Length > 1 && !string.IsNullOrWhiteSpace(name.Substring(1)),
					"The switch name must be more than just a prefix and whitespace.");
			}

			AppendValue(builder, name);
			if (value != null)
			{
				string textValue = Convert.ToString(value);
				if (!string.IsNullOrEmpty(textValue))
				{
					builder.Append('=');
					AppendValue(builder, textValue);
				}
			}
		}

		private void RequireUnparsed()
		{
			Conditions.RequireState(this.currentState < State.Parsed, "Parse has already been called.");
		}

		private CommandLine InternalAddSwitch(string[] names, string description, Delegate handler, CommandLineSwitchOptions options)
		{
			this.RequireUnparsed();

			Switch sw = new Switch(names, handler, options);

			// Add each name first to make sure there are no clashes.
			foreach (string name in names)
			{
				this.nameToSwitchMap.Add(name, sw);
			}

			this.distinctSwitches.Add(sw);

			// Add help if this switch has a description.  Some switches can be hidden (e.g., /?).
			if (!string.IsNullOrEmpty(description))
			{
				string displayName = string.Join(" | ", names.Select(n => "/" + n));
				this.AddArgHelp(new KeyValuePair<string, string>(displayName, description));
			}

			this.currentState = State.Configured;

			return this;
		}

		private void ParseArguments(IEnumerable<string> args)
		{
			Switch pendingSwitch = null;
			foreach (string arg in args)
			{
				// .NET's command line processing won't give us an empty string,
				// but some caller might slip one in unintentionally.  For example,
				// Visual Studio's Project Debug tab's "Command line arguments"
				// textbox lets you paste in newlines.
				if (!string.IsNullOrWhiteSpace(arg))
				{
					// See if we have a switch.
					char argStart = arg[0];
					if (SwitchPrefixes.Contains(argStart))
					{
						// If we currently have a pending switch, then we need to process it.
						// That means we have a switch with no associated value, but we'll
						// let the handler decide if that's ok.
						if (pendingSwitch != null)
						{
							pendingSwitch.SetValue(null, this);
						}

						pendingSwitch = this.ParseSwitchArg(arg);
					}
					else if (pendingSwitch != null)
					{
						// If we have a pending switch, then this argument is its associated value.
						pendingSwitch.SetValue(arg, this);
						pendingSwitch = null;
					}
					else if (this.valueValidationHandler != null)
					{
						List<string> errors = new List<string>(0);
						this.valueValidationHandler(arg, errors);
						this.AddErrors(errors);
					}
					else
					{
						this.AddError("Unsupported argument: {0}", arg);
					}
				}
			}

			// If we finished with a pending switch, then it has no associated value,
			// but we'll let the handler decide if that's ok.
			if (pendingSwitch != null)
			{
				pendingSwitch.SetValue(null, this);
			}
		}

		private Switch ParseSwitchArg(string originalArg)
		{
			Switch pendingSwitch = null;

			// Strip off the prefix character.
			char prefix = originalArg[0];
			string arg = originalArg.Substring(1);

			if (string.IsNullOrEmpty(arg))
			{
				this.AddError("Empty switches ({0}) are not supported.", originalArg);
			}
			else
			{
				// Look for associated value separators before bool suffixes because some programs
				// (e.g., Diff.Net) can use switch formats like /f:g-, so they need to parse the value.
				int separatorIndex = arg.IndexOfAny(SwitchValueSeparators);
				if (separatorIndex > 0)
				{
					if ((separatorIndex + 1) < arg.Length)
					{
						string argName = arg.Substring(0, separatorIndex);
						Switch sw = this.FindSwitch(argName, prefix);
						if (sw != null)
						{
							string argValue = arg.Substring(separatorIndex + 1);
							sw.SetValue(argValue, this);
						}
					}
					else
					{
						this.AddError("Unsupported switch format: {0}", originalArg);
					}
				}
				else if (arg.EndsWith("+", StringComparison.Ordinal) || arg.EndsWith("-", StringComparison.Ordinal))
				{
					Switch sw = this.FindSwitch(arg.Substring(0, arg.Length - 1), prefix);
					if (sw != null)
					{
						bool flagValue = !arg.EndsWith("-", StringComparison.Ordinal);
						sw.SetFlag(flagValue, this);
					}
				}
				else
				{
					Switch sw = this.FindSwitch(arg, prefix);
					if (sw != null)
					{
						if (sw.IsFlagSwitch)
						{
							sw.SetFlag(true, this);
						}
						else
						{
							// This is a value switch with no value, so we have to set it
							// as pending and hope the next arg is its value.
							pendingSwitch = sw;
						}
					}
				}
			}

			return pendingSwitch;
		}

		private Switch FindSwitch(string name, char prefix)
		{
			var matches = from sw in this.distinctSwitches
						from n in sw.Names
						where n.Length >= name.Length && this.nameToSwitchMap.Comparer.Equals(n.Substring(0, name.Length), name)
						select sw;

			Switch result = null;
			int matchCount = matches.Count();
			if (matchCount == 0)
			{
				this.AddError("Unsupported switch: {0}{1}", prefix, name);
			}
			else if (matchCount > 1)
			{
				this.AddError("Ambiguous switch: {0}{1}", prefix, name);
			}
			else
			{
				result = matches.First();
			}

			return result;
		}

		private void AddError(string errorFormat, params object[] formatArgs)
		{
			if (this.errors == null)
			{
				this.errors = new List<string>();
			}

			string error = string.Format(CultureInfo.CurrentCulture, errorFormat, formatArgs);
			this.errors.Add(error);

			if (this.currentState < State.HelpRequested)
			{
				this.currentState = State.Invalid;
			}
		}

		private void AddErrors(List<string> errors)
		{
			foreach (string error in errors)
			{
				this.AddError(error);
			}
		}

		private void AddArgHelp(KeyValuePair<string, string> entry)
		{
			if (this.argHelp == null)
			{
				this.argHelp = new List<KeyValuePair<string, string>>();
			}

			this.argHelp.Add(entry);
		}

		private void WriteLine(TextWriter writer, string text, int windowWidth, int leftMargin = 0)
		{
			int maxSegmentLength = windowWidth - leftMargin;
			string newLine = writer.NewLine ?? Environment.NewLine;

			if (leftMargin == 0 || string.IsNullOrEmpty(text) || (text.Length + newLine.Length) <= maxSegmentLength)
			{
				writer.WriteLine(text);
			}
			else
			{
				List<string> segments = WrapText(text, maxSegmentLength, newLine);
				string wrappedIndent = new string(' ', leftMargin);

				int segmentCount = segments.Count;
				for (int i = 0; i < segmentCount; i++)
				{
					if (i > 0)
					{
						writer.Write(wrappedIndent);
					}

					string segment = segments[i];
					writer.Write(segment);

					// If we're using the console, then it will automatically wrap if the line length is exactly the window width.
					if (!this.UseConsole || (leftMargin + segment.Length) != windowWidth)
					{
						writer.WriteLine();
					}
				}
			}
		}

		#endregion

		#region Private Types

		#region class Switch

		private sealed class Switch
		{
			#region Private Data Members

			private int usageCount;

			#endregion

			#region Constructors

			public Switch(string[] names, Delegate handler, CommandLineSwitchOptions options)
			{
				// It's ok if description is empty.  That just means we won't show help for this switch,
				// which is important for some switches (e.g., /? or "unadvertised" switches).
				Conditions.RequireArgument(names != null && names.Length > 0, "The switch names array must be non-empty.", "names");
				Conditions.RequireReference(handler, "handler");

				this.Names = names;
				this.Handler = handler;
				this.IsRequired = options.HasFlag(CommandLineSwitchOptions.Required);
				this.AllowMultiple = options.HasFlag(CommandLineSwitchOptions.AllowMultiple);

				Conditions.RequireArgument(!this.IsFlagSwitch || !this.AllowMultiple, "Flag switches should never use the AllowMultiple option.", "options");
			}

			#endregion

			#region Public Properties

			public IEnumerable<string> Names { get; }

			public Delegate Handler { get; }

			public bool IsRequired { get; }

			public bool AllowMultiple { get; }

			public bool IsFlagSwitch => this.Handler is Action<bool>;

			public int UsageCount => this.usageCount;

			#endregion

			#region Public Methods

			public void SetFlag(bool flagValue, CommandLine cmd)
			{
				this.usageCount++;

				if (!this.IsFlagSwitch)
				{
					cmd.AddError("/{0} is not a flag switch.  It requires an associated value.", this.Names.First());
				}
				else if (this.UsageCount > 1)
				{
					cmd.AddError("/{0} cannot be used multiple times.", this.Names.First());
				}
				else
				{
					var handler = (Action<bool>)this.Handler;
					handler(flagValue);
				}
			}

			public void SetValue(string argValue, CommandLine cmd)
			{
				this.usageCount++;

				if (this.IsFlagSwitch)
				{
					cmd.AddError("/{0} is a flag switch.  It does not support an associated value.", this.Names.First());
				}
				else if (!this.AllowMultiple && this.UsageCount > 1)
				{
					cmd.AddError("/{0} cannot be used multiple times.", this.Names.First());
				}
				else
				{
					var handler = (Action<string, IList<string>>)this.Handler;
					List<string> errors = new List<string>(0);
					handler(argValue, errors);
					cmd.AddErrors(errors);
				}
			}

			#endregion
		}

		#endregion

		#endregion
	}
}
