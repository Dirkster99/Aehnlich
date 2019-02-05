namespace Diff.Net
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using Menees;
	using Menees.Shell;

	#endregion

	internal static class CommandLineProcessor
	{
		#region Private Data Members

		private static DialogDisplay displayDirDialog = DialogDisplay.UseOption;
		private static DialogDisplay displayFileDialog = DialogDisplay.UseOption;
		private static string[] names = GetNames();

		#endregion

		#region Public Properties

		public static DialogDisplay DisplayDirDialog => displayDirDialog;

		public static DialogDisplay DisplayFileDialog => displayFileDialog;

		public static string[] Names => names;

		#endregion

		#region Private Methods

		private static string[] GetNames()
		{
			// Note: This method is called from a static initializer, so it shouldn't depend on other static fields.
			List<string> result = new List<string>(2);

			CommandLine cmdLine = new CommandLine(false);
			cmdLine.AddSwitch("f", string.Empty, (value, errors) => ProcessFileOption(value), CommandLineSwitchOptions.AllowMultiple);
			cmdLine.AddSwitch("d", string.Empty, (value, errors) => ProcessDirOption(value), CommandLineSwitchOptions.AllowMultiple);
			cmdLine.AddValueHandler((value, errors) => result.Add(TextUtility.StripQuotes(value)));
			cmdLine.Parse();

			return result.ToArray();
		}

		private static char GetArgOption(string arg, out string data)
		{
			data = string.Empty;
			string option = arg;
			if (!string.IsNullOrEmpty(option))
			{
				data = option.Substring(1);
				option = option.Substring(0, 1);
			}

			return char.ToLower(option[0]);
		}

		private static bool GetBool(string data)
		{
			if (string.IsNullOrEmpty(data))
			{
				return true;
			}
			else
			{
				char ch = char.ToLower(data[0]);
				return ch == '+' || ch == 'y' || ch == '1';
			}
		}

		private static void ProcessDirOption(string arg)
		{
			string data;
			char option = GetArgOption(arg, out data);
			switch (option)
			{
				case 'l':
					Options.ShowOnlyInA = GetBool(data);
					break;

				case 'r':
					Options.ShowOnlyInB = GetBool(data);
					break;

				case 'd':
					Options.ShowDifferent = GetBool(data);
					break;

				case 's':
					Options.ShowSame = GetBool(data);
					break;

				case 'c':
					Options.Recursive = GetBool(data);
					break;

				case 'g':
					displayDirDialog = GetBool(data) ? DialogDisplay.Always : DialogDisplay.OnlyIfNecessary;
					break;
			}
		}

		private static void ProcessFileOption(string arg)
		{
			string data;
			char option = GetArgOption(arg, out data);
			switch (option)
			{
				case 'c':
					Options.IgnoreCase = GetBool(data);
					break;

				case 'x':
					// Use Text (for /f:x-) instead of Auto for backward compatibility.
					// The preferred method now is to use /f:t[atxb]
					Options.CompareType = GetBool(data) ? CompareType.Xml : CompareType.Text;
					break;

				case 'w':
					Options.IgnoreTextWhitespace = GetBool(data);
					break;

				case 'g':
					displayFileDialog = GetBool(data) ? DialogDisplay.Always : DialogDisplay.OnlyIfNecessary;
					break;

				case 't':
					switch ((data ?? string.Empty).ToLower())
					{
						case "a":
							Options.CompareType = CompareType.Auto;
							break;

						case "t":
							Options.CompareType = CompareType.Text;
							break;

						case "x":
							Options.CompareType = CompareType.Xml;
							break;

						case "b":
							Options.CompareType = CompareType.Binary;
							break;
					}

					break;
			}
		}

		#endregion
	}
}
