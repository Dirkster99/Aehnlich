namespace Menees.Shell
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Text;

	#endregion

	#region public enum CommandLineParseResult

	/// <summary>
	/// Defines the return states for <see cref="CommandLine.Parse(IEnumerable{string})"/>.
	/// </summary>
	public enum CommandLineParseResult
	{
		/// <summary>
		/// Unknown or unparsed.
		/// </summary>
		Unknown,

		/// <summary>
		/// All arguments were valid.
		/// </summary>
		Valid,

		/// <summary>
		/// One or more arguments were invalid.
		/// </summary>
		Invalid,

		/// <summary>
		/// The user requested help.
		/// </summary>
		HelpRequested,
	}

	#endregion

	#region public enum CommandLineSwitchOptions

	/// <summary>
	/// Defines the options for <see cref="CommandLine"/>'s AddSwitch methods.
	/// </summary>
	[Flags]
	public enum CommandLineSwitchOptions
	{
		/// <summary>
		/// Use the default switch behavior (i.e., optional and single use).
		/// </summary>
		None = 0,

		/// <summary>
		/// The switch is required.
		/// </summary>
		Required = 1,

		/// <summary>
		/// Allow multiple uses of the switch.
		/// </summary>
		AllowMultiple = 2,
	}

	#endregion

	#region public enum CommandLineWriteOptions

	/// <summary>
	/// Defines the options for <see cref="CommandLine.WriteMessage(TextWriter, CommandLineWriteOptions)"/>.
	/// </summary>
	[Flags]
	public enum CommandLineWriteOptions
	{
		/// <summary>
		/// Don't write any output.
		/// </summary>
		None = 0,

		/// <summary>
		/// Write out the header.
		/// </summary>
		Header = 1,

		/// <summary>
		/// Write out the error message.
		/// </summary>
		Error = 2,

		/// <summary>
		/// Write out the help.
		/// </summary>
		Help = 4,
	}

	#endregion

	#region public enum IconOptions

	/// <summary>
	/// Defines the options for <see cref="ShellUtility.GetFileTypeInfo"/>.
	/// </summary>
	/// <remarks>
	/// For more information see the Win32 SHGetFileInfo function documentation at
	/// http://msdn.microsoft.com/en-us/library/windows/desktop/bb762179.aspx
	/// </remarks>
	[Flags]
	public enum IconOptions
	{
		/// <summary>
		/// No icon should be returned.
		/// </summary>
		None = 0,

		/// <summary>
		/// A small icon should be returned.  If the <see cref="ShellSize"/> option isn't specified,
		/// then its size will equal <c>SystemInformation.SmallIconSize</c>, which is typically 16x16.
		/// </summary>
		Small = 1,

		/// <summary>
		/// A large icon should be returned.  If the <see cref="ShellSize"/> option isn't specified,
		/// then its size will equal <c>SystemInformation.IconSize</c>, which is typically 32x32.
		/// </summary>
		Large = 2,

		/// <summary>
		/// The returned icon should be sized based on the shell settings rather than the
		/// <c>SystemInformation</c> settings.
		/// </summary>
		ShellSize = 4,

		/// <summary>
		/// The returned icon should be for the type's "Open" state, which can be different
		/// from the default "Closed" state for container types (e.g., folders).
		/// </summary>
		/// <seealso cref="Folder"/>
		Open = 8,

		/// <summary>
		/// The returned icon should have a shortcut link overlay applied to it.
		/// </summary>
		Shortcut = 16,

		/// <summary>
		/// The returned icon should be blended with the system's highlight color.
		/// </summary>
		Selected = 32,

		/// <summary>
		/// The returned icon should be for a folder instead of a file.  This is only used if the
		/// "useExistingFile" parameter of <see cref="ShellUtility.GetFileTypeInfo"/> is false.
		/// </summary>
		Folder = 64,
	}

	#endregion
}
