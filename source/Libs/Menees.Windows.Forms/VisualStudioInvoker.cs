namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Threading;
	using Menees.Windows.Forms;

	#endregion

	/// <summary>
	/// Used to open a file in Visual Studio at a specific line number.
	/// </summary>
	/// <remarks>
	/// Using WindowsUtility.ShellExecute, it's very easy to open a file that's associated
	/// with Visual Studio.  But to force a file open in Visual Studio and then
	/// jump to a specific line number is a lot more work.
	/// <para/>
	/// The bulk of the code for this class was shamelessly pulled from FxCop 1.35's
	/// Microsoft.FxCop.UI.FxCopUI class (in FxCop.exe) using Reflector and the
	/// FileDisassembler add-in.  Then it was updated to use C# 4's dynamic keyword
	/// to make the run-time invocation easier.
	/// </remarks>
	internal static class VisualStudioInvoker
	{
		#region Public Methods

		/// <summary>
		/// Opens a file in Visual Studio.
		/// </summary>
		/// <param name="fileName">The full path to a file to open.</param>
		/// <param name="fileLineNumber">The 1-based line number to go to in the file.</param>
		/// <returns>True if it was successful.  False if the file couldn't be opened in Visual Studio.</returns>
		public static bool OpenFile(string fileName, string fileLineNumber)
		{
			bool result = false;

			try
			{
				// Use late-bound COM to open the file in Visual Studio
				// so we can jump to a specific line number.  This also
				// allows us to reuse an open instance of VS.
				//
				// We could execute Visual Studio by command-line and
				// run the GotoLn command (like MegaBuild does), but that
				// requires starting a new instance of VS for each file opened.
				dynamic dte = GetVisualStudioInstance();
				OpenInVisualStudio(dte, fileName, fileLineNumber);
				result = dte != null;
			}
			catch (COMException)
			{
			}
			catch (ArgumentException)
			{
			}

			return result;
		}

		#endregion

		#region Private Methods

		private static object GetVisualStudioInstance()
		{
			object result = null;

			const string VisualStudioProgId = "VisualStudio.DTE";
			try
			{
				// See if there's a running instance of VS.
				result = Marshal.GetActiveObject(VisualStudioProgId);
			}
			catch (COMException)
			{
				// See if VS is registered.
				Type dteType = Type.GetTypeFromProgID(VisualStudioProgId, false);
				if (dteType != null)
				{
					// VS is registered, so we need to start a new instance.
					result = Activator.CreateInstance(dteType);
				}
			}

			return result;
		}

		private static void OpenInVisualStudio(dynamic dte, string fileName, string line)
		{
			dte.ExecuteCommand("File.OpenFile", TextUtility.EnsureQuotes(fileName));
			dte.ExecuteCommand("Edit.Goto", line);

			dynamic mainWindow = dte.MainWindow;
			IntPtr mainWindowHandle = (IntPtr)Convert.ToInt64(mainWindow.HWnd);

			NativeMethods.BringWindowForward(mainWindowHandle);
			const int MillisecondsPerSecond = 1000;
			Thread.Sleep(MillisecondsPerSecond);

			mainWindow.Activate();
			mainWindow.Visible = true;
		}

		#endregion
	}
}
