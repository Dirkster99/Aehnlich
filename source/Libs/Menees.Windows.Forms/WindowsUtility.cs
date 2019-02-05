namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Security.Principal;
	using System.Text;
	using System.Threading;
	using System.Windows.Forms;
	using System.Windows.Forms.VisualStyles;
	using Menees.Shell;

	#endregion

	/// <summary>
	/// Methods and properties for Windows applications.
	/// </summary>
	public static class WindowsUtility
	{
		#region Private Data Members

		private static readonly Size SmallIconSize = SystemInformation.SmallIconSize;
		private static int showingUnhandledExceptionErrorMessage;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets whether the clipboard currently contains text data.
		/// </summary>
		public static bool ClipboardContainsText
		{
			get
			{
				bool result = false;

				try
				{
					IDataObject data = Clipboard.GetDataObject();
					if (data != null)
					{
						result = data.GetDataPresent(DataFormats.Text);
					}
				}
				catch (ExternalException)
				{
					result = false;
				}

				return result;
			}
		}

		/// <summary>
		/// Gets whether visual styles are currently enabled.
		/// </summary>
		public static bool AreVisualStylesEnabled
		{
			get
			{
				bool result = VisualStyleInformation.IsEnabledByUser && Application.VisualStyleState != VisualStyleState.NoneEnabled;
				return result;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Initializes the application's name and sets up a handler to report uncaught
		/// exceptions.
		/// </summary>
		/// <param name="applicationName">The name of the application to pass to <see cref="ApplicationInfo.Initialize"/>.</param>
		/// <param name="showException">The action to call when an exception needs to be shown.  This can be null,
		/// which will cause <see cref="ShowError"/> to be called.</param>
		public static void InitializeApplication(string applicationName, Action<Exception> showException)
		{
			ApplicationInfo.Initialize(applicationName);

			Application.ThreadException += (sender, e) =>
			{
				Exception ex = e.Exception;
				Log.Error(typeof(WindowsUtility), "An unhandled exception occurred in a Windows Forms thread.", ex);

				// Only let the first thread in show a message box.
				int showing = Interlocked.Increment(ref showingUnhandledExceptionErrorMessage);
				try
				{
					if (showing == 1)
					{
						if (showException != null)
						{
							showException(ex);
						}
						else
						{
							StringBuilder sb = new StringBuilder();
							sb.AppendLine(Exceptions.GetMessage(ex));
							if (ApplicationInfo.IsDebugBuild)
							{
								// Show the root exception's type and call stack in debug builds.
								sb.AppendLine().AppendLine(ex.ToString());
							}

							ShowError(null, sb.ToString().Trim());
						}
					}
				}
				finally
				{
					Interlocked.Decrement(ref showingUnhandledExceptionErrorMessage);
				}
			};

			// Setup the current thread and any other Windows Forms threads to route unhandled
			// exceptions to the Application.ThreadException handler.
			try
			{
				Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic, true);
				Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic, false);
			}
			catch (InvalidOperationException)
			{
				// If the process is running in the Visual Studio hosting process (e.g., *.vshost.exe),
				// then on subsequent runs the mode can't be set again.
			}

			// Set some style settings that all Windows Forms apps should use now.
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
		}

		/// <summary>
		/// Selects a file system path and allows the user to type in a path if necessary.
		/// </summary>
		/// <param name="owner">The owner of the displayed modal dialog.</param>
		/// <param name="title">A short title for the path being selected.</param>
		/// <param name="initialFolder">The initial path to select.</param>
		/// <returns>The path the user selected if they pressed OK.  Null otherwise (e.g., the user canceled).</returns>
		public static string SelectFolder(IWin32Window owner, string title, string initialFolder)
		{
			IntPtr? ownerHandle = owner != null ? owner.Handle : (IntPtr?)null;
			string result = ShellUtility.SelectFolder(ownerHandle, title, initialFolder);
			return result;
		}

		/// <summary>
		/// Executes the default action on the specified file using the Windows shell.
		/// </summary>
		/// <param name="owner">The parent window for any error dialogs.</param>
		/// <param name="fileName">The text or filename to execute.</param>
		/// <returns>Whether the file was opened/executed successfully.</returns>
		public static bool ShellExecute(IWin32Window owner, string fileName)
		{
			bool result = false;

			try
			{
				using (Process process = ShellExecute(owner, fileName, string.Empty))
				{
					result = true;
				}
			}
			catch (Win32Exception)
			{
				// The core ShellExecute logic already displays an error dialog if a Win32Exception occurs.
			}

			return result;
		}

		/// <summary>
		/// Executes an action on the specified file using the Windows shell.
		/// </summary>
		/// <param name="owner">The parent window for any error dialogs.</param>
		/// <param name="fileName">The text or filename to execute.</param>
		/// <param name="verb">The shell action that should be taken.  Pass an empty string for the default action.</param>
		/// <returns>The process started by executing the file.</returns>
		public static Process ShellExecute(IWin32Window owner, string fileName, string verb)
		{
			IntPtr? ownerHandle = null;
			if (owner != null)
			{
				ownerHandle = owner.Handle;
			}

			Process result = ShellUtility.ShellExecute(ownerHandle, fileName, verb);
			return result;
		}

		/// <summary>
		/// Displays a standard "About" dialog for the current application assembly.
		/// </summary>
		/// <remarks>
		/// At application startup, you should call the <see cref="InitializeApplication"/>
		/// method to set the current application's name, which will be displayed in the
		/// About box.
		/// </remarks>
		/// <param name="owner">The owner of the displayed modal dialog.</param>
		/// <param name="mainAssembly">The main assembly for the application,
		/// which the version and copyright information will be read from.</param>
		public static void ShowAboutBox(IWin32Window owner, Assembly mainAssembly)
		{
			// If an assembly wasn't provided, then we want the version of the calling assembly not the current assembly.
			using (AboutBox dialog = new AboutBox(mainAssembly ?? Assembly.GetCallingAssembly()))
			{
				dialog.Execute(owner);
			}
		}

		/// <summary>
		/// Shows an input box for a single value.
		/// </summary>
		/// <param name="owner">The owner of the displayed modal dialog.</param>
		/// <param name="prompt">The message to prompt with (up to 4 lines long).</param>
		/// <returns>The user-entered value if they pressed OK, or null if Cancel was pressed.</returns>
		public static string ShowInputBox(IWin32Window owner, string prompt) => ShowInputBox(owner, prompt, null, string.Empty, null, null);

		/// <summary>
		/// Shows an input box for a single value with validation.
		/// </summary>
		/// <param name="owner">The owner of the displayed modal dialog.</param>
		/// <param name="prompt">The message to prompt with (up to 4 lines long).</param>
		/// <param name="title">The caption of the displayed dialog.  This can be null
		/// to use the application name as the title.</param>
		/// <param name="defaultValue">The initial value of the input field.</param>
		/// <param name="maxLength">The maximum length of the input field in characters.
		/// This can be null to use the default maximum length limit.</param>
		/// <param name="validate">An optional function to validate the input.  This can be null.
		/// The function should return a null if the input passes validation, and it should return an error
		/// message to display to the end user if the input fails validation.</param>
		/// <returns>The user-entered value if they pressed OK, or null if Cancel was pressed.</returns>
		public static string ShowInputBox(
			IWin32Window owner,
			string prompt,
			string title,
			string defaultValue,
			int? maxLength,
			Func<string, string> validate)
		{
			using (InputDialog dialog = new InputDialog())
			{
				if (string.IsNullOrEmpty(title))
				{
					dialog.Text = ApplicationInfo.ApplicationName;
				}
				else
				{
					dialog.Text = title;
				}

				string result = dialog.Execute(owner, prompt, defaultValue, maxLength, validate);
				return result;
			}
		}

		/// <summary>
		/// Displays an error message in a MessageBox.
		/// </summary>
		/// <param name="owner">The dialog owner window.  This can be null to use the desktop as the owner.</param>
		/// <param name="message">The message to display.</param>
		public static void ShowError(IWin32Window owner, string message)
		{
			MessageBox.Show(owner, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		/// <summary>
		/// Extracts a <see cref="SystemInformation.SmallIconSize"/> (typically 16x16) image from the specified icon.
		/// </summary>
		/// <param name="icon">An icon, which can contain multiple image sizes.</param>
		/// <returns>A small image either extracted from <paramref name="icon"/> or drawn scaled onto a new bitmap.</returns>
		public static Image GetSmallIconImage(Icon icon)
		{
			Conditions.RequireReference(icon, "icon");

			Image result = null;

			if (icon.Size == SmallIconSize)
			{
				result = icon.ToBitmap();
			}
			else
			{
				// See if the icon contains an image of the size we want.
				using (Icon extractedIcon = new Icon(icon, SmallIconSize))
				{
					if (extractedIcon.Size == SmallIconSize)
					{
						result = extractedIcon.ToBitmap();
					}
					else
					{
						// We have to manually scale the image to the small icon size, so get it from
						// the original icon instead of the extracted one (which could already be smaller).
						// http://stackoverflow.com/questions/463273/get-full-quality-16-x-16-icon-using-icon-extractassociatedicon-and-imagelist
						result = new Bitmap(SmallIconSize.Width, SmallIconSize.Height);
						using (Bitmap iconImage = icon.ToBitmap())
						using (Graphics g = Graphics.FromImage(result))
						{
							g.InterpolationMode = InterpolationMode.HighQualityBicubic;
							g.DrawImage(iconImage, new Rectangle(Point.Empty, SmallIconSize), new Rectangle(Point.Empty, iconImage.Size), GraphicsUnit.Pixel);
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Changes the border style for .NET's MDI client area.
		/// </summary>
		/// <param name="form">An MDI container form.</param>
		/// <param name="borderStyle">The desired border style.</param>
		public static void SetMdiClientBorderStyle(Form form, BorderStyle borderStyle)
		{
			Conditions.RequireReference(form, "form");
			Conditions.RequireState(form.IsMdiContainer, "The form must be an MDI container.");

			// The logic to do this came from:
			// http://www.codeproject.com/Articles/8489/Getting-a-quot-Handle-quot-on-the-MDI-Client
			//
			// In another article, Microsoft recommended changing the Dock style to get rid of the 3D border,
			// but that didn't work reliably, and it required lots of extra work to position the MdiClient.
			// http://bytes.com/topic/visual-basic-net/answers/607162-customize-windows-forms-mdiclient
			MdiClient mdiClient = form.Controls.OfType<MdiClient>().FirstOrDefault();
			if (mdiClient != null)
			{
				NativeMethods.SetBorderStyle(mdiClient, borderStyle);
			}
		}

		#endregion
	}
}
