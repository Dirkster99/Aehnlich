namespace Menees
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Configuration;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Runtime;
	using System.Security.Principal;
	using System.Text;
	using System.Threading.Tasks;
	using System.Xml.Linq;
	using Menees.Diagnostics;

	#endregion

	/// <summary>
	/// Provides information about the current application.
	/// </summary>
	public static class ApplicationInfo
	{
		#region Public Read-Only Fields

		/// <summary>
		/// Gets whether the current build is a debug build (i.e., whether the "DEBUG" constant is defined).
		/// </summary>
		/// <devnote>
		/// This is a static readonly field instead of a const because code needs to check the run-time
		/// value instead of the compile-time value.  Visual Studio doesn't allow per-configuration assembly
		/// references, so all projects reference the debug assemblies.  If we made IsDebugBuild a const,
		/// then this would always be considered true by other assemblies, even if they were built in
		/// release mode because release mode builds still reference the debug version of this assembly.
		/// At run-time, release mode assemblies will load a release build of this assembly, and they can get
		/// the correct/expected value from this field.
		/// </devnote>
		[SuppressMessage(
			"Microsoft.Performance",
			"CA1802:UseLiteralsWhereAppropriate",
			Justification = "See the devnote comments above for a detailed explanation.")]
		public static readonly bool IsDebugBuild
#if DEBUG
			= true;
#else
			= false;
#endif

		#endregion

		#region Private Data Members

		private static readonly Lazy<int> LazyProcessId = new Lazy<int>(() =>
			{
				using (Process current = Process.GetCurrentProcess())
				{
					return current.Id;
				}
			});

		private static string applicationName;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the application name.
		/// </summary>
		public static string ApplicationName
		{
			get
			{
				string result = applicationName;

				if (string.IsNullOrEmpty(result))
				{
					result = GlobalLogContext.GetDefaultApplicationName();
				}

				return result;
			}
		}

		/// <summary>
		/// Gets the base directory for the current application.
		/// </summary>
		/// <remarks>
		/// This is usually the same as the <see cref="ExecutableFile"/>'s directory, but it can be
		/// different for applications using custom AppDomains (e.g., web apps running in IIS).
		/// </remarks>
		public static string BaseDirectory
		{
			get
			{
				string result = AppDomain.CurrentDomain.BaseDirectory;
				return result;
			}
		}

		/// <summary>
		/// Gets the full path for the executable file that started the application.
		/// </summary>
		/// <remarks>
		/// This is similar to System.Windows.Forms.Application.ExecutablePath, except this supports paths
		/// longer than MAX_PATH (260) and paths using a "\\?\" prefix (e.g., ASP.NET worker processes).
		/// </remarks>
		public static string ExecutableFile
		{
			get
			{
				string result = NativeMethods.GetModuleFileName(IntPtr.Zero);
				return result;
			}
		}

		/// <summary>
		/// Gets whether the current user is running in the Windows "Administrator" role.
		/// </summary>
		public static bool IsUserRunningAsAdministrator
		{
			get
			{
				using (WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent())
				{
					WindowsPrincipal currentPrincipal = new WindowsPrincipal(currentIdentity);
					bool result = currentPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
					return result;
				}
			}
		}

		/// <summary>
		/// Gets the current application's Windows process ID.
		/// </summary>
		public static int ProcessId => LazyProcessId.Value;

		/// <summary>
		/// Gets whether the current application is active (i.e., owns the foreground window).
		/// </summary>
		public static bool IsActivated
		{
			get
			{
				bool result = NativeMethods.IsApplicationActivated;
				return result;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a hierarchical store for loading and saving user-level settings for the current application.
		/// </summary>
		/// <returns>A new settings store.</returns>
		public static ISettingsStore CreateUserSettingsStore()
		{
			ISettingsStore result;

			// The registry has the advantage that it truly isolates each user's
			// settings from each other, so one (non-admin) user can't see or
			// edit another user's settings on disk.  However, the registry is also
			// shared between multiple instances of the same app on one machine
			// (e.g., if you're testing ClientA and ClientB on one box), and the
			// settings for the app hang around in the registry even if you delete
			// the application's folder.
			//
			// The file store has the advantage that by default everything is located
			// in the current application directory.  However, if the user doesn't
			// have permissions to write to the application directory, then it falls back
			// to using a user's shared folder (e.g., AppData\Local or %Temp%),
			// which means it would lose the side-by-side isolation ability.
			//
			// We'll default to the file store because it's the best for most purposes.
			if (Properties.Settings.Default.StoreUserSettingsInRegistry)
			{
				result = new RegistrySettingsStore();
			}
			else
			{
				result = new FileSettingsStore();
			}

			return result;
		}

		/// <summary>
		/// Used to initialize the application's name, error handling, etc.
		/// </summary>
		/// <param name="applicationName">Pass null to use the current AppDomain's friendly name.</param>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void Initialize(string applicationName)
		{
			// Try to log when unhandled or unobserved exceptions occur.  This info can be very useful if the process crashes.
			// Note: Windows Forms unhandled exceptions are logged via Menees.Windows.Forms.WindowsUtility.InitializeApplication.
			AppDomain.CurrentDomain.UnhandledException += (s, e) =>
			{
				LogLevel level = e.IsTerminating ? LogLevel.Fatal : LogLevel.Error;
				Log.Write(typeof(ApplicationInfo), level, "An unhandled exception occurred.", e.ExceptionObject as Exception);
			};

			TaskScheduler.UnobservedTaskException += (s, e) =>
			{
				Log.Error(typeof(ApplicationInfo), "A Task exception occurred, but it was never observed by the Task caller.", e.Exception);
			};

			// If the name is null or empty, then the property accessor will use the AppDomain's friendly name.
			ApplicationInfo.applicationName = applicationName;

			// Put it in the log's global context, so it will appear in every log entry.
			if (!string.IsNullOrEmpty(applicationName))
			{
				Log.GlobalContext.SetApplicationName(applicationName);
			}

			// Call SetErrorMode to disable the display of Windows Shell modal error dialogs for
			// file not found, Windows Error Reporting, and other errors.  From SetErrorMode docs
			// at http://msdn.microsoft.com/en-us/library/ms680621.aspx:
			// 		"Best practice is that all applications call the process-wide SetErrorMode
			// 		function with a parameter of SEM_FAILCRITICALERRORS at startup. This is
			// 		to prevent error mode dialogs from hanging the application."
			NativeMethods.DisableShellModalErrorDialogs();

			// Enable profile-guided optimizations (PGO or "Pogo") if we're not in unit tests, web apps, etc.
			// http://blogs.msdn.com/b/dotnet/archive/2012/10/18/an-easy-solution-for-improving-app-launch-performance.aspx
			if (AppDomain.CurrentDomain.IsDefaultAppDomain())
			{
				ProfileOptimization.SetProfileRoot(Path.GetTempPath());
				ProfileOptimization.StartProfile(ApplicationName + ".pgo");
			}
		}

		#endregion
	}
}
