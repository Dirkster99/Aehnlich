namespace Menees.Shell
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using System.Text;

	#endregion

	/// <summary>
	/// Methods for working with the Windows shell.
	/// </summary>
	public static class ShellUtility
	{
		#region Public Methods

		/// <summary>
		/// Gets the assembly's copyright in a displayable format (e.g., for an About box).
		/// </summary>
		/// <param name="assembly">The assembly to get the copyright from.</param>
		/// <returns>User-friendly copyright information.</returns>
		/// <remarks>
		/// If a copyright isn't found in the passed-in assembly or if it is null or empty,
		/// then the copyright information from the current assembly will be returned.
		/// </remarks>
		public static string GetCopyrightInfo(Assembly assembly)
		{
			string result = null;

			if (assembly != null)
			{
				result = ReflectionUtility.GetCopyright(assembly);
			}

			if (string.IsNullOrEmpty(result))
			{
				Assembly current = Assembly.GetExecutingAssembly();
				if (current != assembly)
				{
					result = GetCopyrightInfo(current);
				}
			}

			return result;
		}

		/// <summary>
		/// Gets the shell's file type name and icon.
		/// </summary>
		/// <param name="fileName">A file name.  If <paramref name="useExistingFile"/> is true, then this should be
		/// the full path to an existing file.  Otherwise, this can just be an extension (with leading period) to get
		/// generic file type information.</param>
		/// <param name="useExistingFile">If true, then the file specified by <paramref name="fileName"/> must exist,
		/// and its icon will be extracted, which can be unique for some file types like .exe and .ico.  If false, then the
		/// <paramref name="fileName"/> can just be an extension, and the file type's standard icon will be used.</param>
		/// <param name="iconOptions">Options that determine the size and style of icon to get.</param>
		/// <param name="useIconHandle">If <paramref name="iconOptions"/> is None, then this should be null.
		/// Otherwise, this should be a delegate that converts the passed-in HICON into the desired icon type.
		/// In Windows Forms, this can be done with a delegate like:
		/// <c>hIcon => icon = (Icon)Icon.FromHandle(hIcon).Clone()</c>
		/// In WPF, the icon handle can be converted using the Imaging.CreateBitmapSourceFromHIcon method.
		/// <para/>
		/// Note: The icon handle will be destroyed automatically when this method returns, so the delegate
		/// must copy/clone the icon if it needs to use it later.
		/// </param>
		/// <returns>The shell's file type name (e.g., "Visual C# Source file" for a ".cs" file).</returns>
		public static string GetFileTypeInfo(string fileName, bool useExistingFile, IconOptions iconOptions, Action<IntPtr> useIconHandle)
			=> NativeMethods.GetShellFileTypeAndIcon(fileName, useExistingFile, iconOptions, useIconHandle);

		/// <summary>
		/// Gets the assembly's version information in a displayable format (e.g., for an About box).
		/// </summary>
		/// <param name="assembly">The assembly to get the version information from.</param>
		/// <returns>User-friendly version information.</returns>
		public static string GetVersionInfo(Assembly assembly)
		{
			Conditions.RequireReference(assembly, "assembly");

			StringBuilder sb = new StringBuilder("Version ");

			// Show at least Major.Minor, but only show Build and Revision if they're non-zero.
			Version displayVersion = assembly.GetName(false).Version;
			const int MaxVersionFields = 4;
			int versionFieldsToDisplay = MaxVersionFields;
			if (displayVersion.Revision == 0)
			{
				versionFieldsToDisplay--;
				if (displayVersion.Build == 0)
				{
					versionFieldsToDisplay--;
				}
			}

			sb.Append(displayVersion.ToString(versionFieldsToDisplay));

			DateTime? built = ReflectionUtility.GetBuildTime(assembly);
			if (built != null)
			{
				sb.Append(" – ").AppendFormat("{0:d}", built.Value.ToLocalTime());
			}

			sb.Append(Environment.Is64BitProcess ? " – 64-bit" : " – 32-bit");

			// On Vista or later, show whether the user is running as an administrator.
			OperatingSystem os = Environment.OSVersion;
			const int WindowsVistaMajorVersion = 6;
			if (os.Version >= new Version(WindowsVistaMajorVersion, 0) && ApplicationInfo.IsUserRunningAsAdministrator)
			{
				sb.Append(" – Administrator");
			}

			return sb.ToString();
		}

		/// <summary>
		/// Selects a file system path and allows the user to type in a path if necessary.
		/// </summary>
		/// <param name="ownerHandle">The handle of the owner of the displayed modal dialog.</param>
		/// <param name="title">A short description of the path being selected.</param>
		/// <param name="initialFolder">The initial path to select.</param>
		/// <returns>The path the user selected if they pressed OK.  Null otherwise (e.g., the user canceled).</returns>
		public static string SelectFolder(IntPtr? ownerHandle, string title, string initialFolder)
			=> NativeMethods.SelectFolder(ownerHandle, title, initialFolder);

		/// <summary>
		/// Executes the default action on the specified file using the Windows shell.
		/// </summary>
		/// <param name="ownerHandle">The parent window handle to use for any error dialogs.</param>
		/// <param name="fileName">The text or filename to execute.</param>
		/// <returns>The process started by executing the file.</returns>
		/// <exception cref="Win32Exception">An error occurred when opening the associated file.</exception>
		public static Process ShellExecute(IntPtr? ownerHandle, string fileName) => ShellExecute(ownerHandle, fileName, string.Empty);

		/// <summary>
		/// Executes an action on the specified file using the Windows shell.
		/// </summary>
		/// <param name="ownerHandle">The parent window handle to use for any error dialogs.</param>
		/// <param name="fileName">The text or filename to execute.</param>
		/// <param name="verb">The shell action that should be taken.  Pass an empty string for the default action.</param>
		/// <returns>The process started by executing the file.</returns>
		/// <exception cref="Win32Exception">An error occurred when opening the associated file.</exception>
		public static Process ShellExecute(IntPtr? ownerHandle, string fileName, string verb)
		{
			Conditions.RequireString(fileName, "fileName");

			ProcessStartInfo startInfo = new ProcessStartInfo();

			startInfo.ErrorDialog = true;
			if (ownerHandle != null)
			{
				startInfo.ErrorDialogParentHandle = ownerHandle.Value;
			}

			startInfo.FileName = fileName;
			startInfo.UseShellExecute = true;
			startInfo.Verb = verb;

			Process result = Process.Start(startInfo);
			return result;
		}

		#endregion
	}
}
