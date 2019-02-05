namespace Menees
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;
	using System.Text;
	using Menees.Shell;

	#endregion

	internal static class NativeMethods
	{
		#region Private Data Members

		private const string CLSID_FileOpenDialog = "DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7";
		private const string IID_IFileDialog = "42F85136-DB7E-439C-85F1-E4075D135FC8";
		private const string IID_IShellItem = "43826D1E-E718-42EE-BC55-A1E261C37BFE";
		private const uint S_OK = 0;

		#endregion

		#region Private Enums

		[Flags]
		private enum ErrorModes : uint // Base as uint since SetErrorMode takes a UINT.
		{
			SYSTEM_DEFAULT = 0x0,
			SEM_FAILCRITICALERRORS = 0x0001,
			SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
			SEM_NOGPFAULTERRORBOX = 0x0002,
			SEM_NOOPENFILEERRORBOX = 0x8000,
		}

		[Flags]
		private enum SHGFI : int
		{
			Icon = 0x000000100,
			DisplayName = 0x000000200,
			TypeName = 0x000000400,
			Attributes = 0x000000800,
			IconLocation = 0x000001000,
			ExeType = 0x000002000,
			SysIconIndex = 0x000004000,
			LinkOverlay = 0x000008000,
			Selected = 0x000010000,
			Attr_Specified = 0x000020000,
			LargeIcon = 0x000000000,
			SmallIcon = 0x000000001,
			OpenIcon = 0x000000002,
			ShellIconSize = 0x000000004,
			PIDL = 0x000000008,
			UseFileAttributes = 0x000000010,
			AddOverlays = 0x000000020,
			OverlayIndex = 0x000000040,
		}

		#endregion

		#region Private Interfaces

		#region IFileDialog

		[ComImport]
		[Guid(IID_IFileDialog)]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IFileDialog
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[PreserveSig] // Need this in case it returns ERROR_CANCELLED, which shouldn't be an exception.
			uint Show([In, Optional] IntPtr hwndOwner); // From IModalWindow

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetFileTypes([In] uint cFileTypes, [In, MarshalAs(UnmanagedType.LPArray)] IntPtr rgFilterSpec);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetFileTypeIndex([In] uint iFileType);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetFileTypeIndex(out uint piFileType);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint Advise([In, MarshalAs(UnmanagedType.Interface)] IntPtr pfde, out uint pdwCookie);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint Unadvise([In] uint dwCookie);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetOptions([In] uint fos);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetOptions(out uint fos);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, uint fdap);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint Close([MarshalAs(UnmanagedType.Error)] uint hr);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetClientGuid([In] ref Guid guid);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint ClearClientData();

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
		}

		#endregion

		#region IShellItem

		[ComImport]
		[Guid(IID_IShellItem)]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IShellItem
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint BindToHandler([In] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out IntPtr ppvOut);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetDisplayName([In] uint sigdnName, out IntPtr ppszName);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint Compare([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);
		}

		#endregion

		#endregion

		#region Internal Properties

		[SuppressMessage(
			"Microsoft.Usage",
			"CA1806:DoNotIgnoreMethodResults",
			MessageId = "Menees.NativeMethods.GetWindowThreadProcessId(System.IntPtr,System.Int32@)",
			Justification = "GetWindowThreadProcessId returns a thread ID not an HRESULT.  We check Marshal.GetLastWin32Error().")]
		internal static bool IsApplicationActivated
		{
			get
			{
				// This property is thread-safe, not WinForm or WPF specific, and works even if child windows have focus in a process.
				// http://stackoverflow.com/questions/7162834/determine-if-current-application-is-activated-has-focus/7162873#7162873
				bool result = false;

				// GetForegroundWindow will return null if no window is currently activated
				// (e.g., the screen is locked or a remote desktop is connected but minimized).
				IntPtr activatedHandle = GetForegroundWindow();
				if (Marshal.GetLastWin32Error() == 0 && activatedHandle != IntPtr.Zero)
				{
					int activatedProcessId;
					GetWindowThreadProcessId(activatedHandle, out activatedProcessId);
					result = Marshal.GetLastWin32Error() == 0 && activatedProcessId == ApplicationInfo.ProcessId;
				}

				return result;
			}
		}

		#endregion

		#region Internal Methods

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DeleteFile(string fileName);

		internal static uint DisableShellModalErrorDialogs()
		{
			const ErrorModes newErrorMode = ErrorModes.SEM_FAILCRITICALERRORS |
				ErrorModes.SEM_NOGPFAULTERRORBOX |
				ErrorModes.SEM_NOOPENFILEERRORBOX;
			ErrorModes oldErrorMode = SetErrorMode(newErrorMode);
			return (uint)oldErrorMode;
		}

		internal static string GetModuleFileName(IntPtr module)
		{
			const int BufferSize = 32768;
			StringBuilder sb = new StringBuilder(BufferSize);
			uint result = GetModuleFileName(module, sb, sb.Capacity);
			if (result == 0)
			{
				throw Exceptions.Log(new Win32Exception(Marshal.GetLastWin32Error(), "Error calling GetModuleFileName for the specified HMODULE."));
			}
			else
			{
				string fileName = sb.ToString();

				// See the docs for GetModuleFileName and the "Naming a File" MSDN topic.
				const string longUncPrefix = @"\\?\UNC\";
				const string longPrefix = @"\\?\";
				if (fileName.StartsWith(longUncPrefix))
				{
					fileName = fileName.Substring(longUncPrefix.Length);
				}
				else if (fileName.StartsWith(longPrefix))
				{
					fileName = fileName.Substring(longPrefix.Length);
				}

				return fileName;
			}
		}

		internal static string[] SplitCommandLine(string commandLine)
		{
			// This logic came from http://www.pinvoke.net/default.aspx/shell32/CommandLineToArgvW.html
			// and also from http://stackoverflow.com/questions/298830/split-string-containing-command-line-parameters-into-string-in-c-sharp.
			// Use CommandLineToArgvW because it handles a variety of special quote and backslash cases such as:
			// 		/c:"1 2" is parsed as a single arg: /c:1 2
			// 		Double double quotes ("") become a single escaped double quote usually.
			// 		Backslash double quote may be parsed as an escaped double quote.
			// See: "What's up with the strange treatment of quotation marks and backslashes by CommandLineToArgvW"
			// http://blogs.msdn.com/b/oldnewthing/archive/2010/09/17/10063629.aspx
			// Also: "How is the CommandLineToArgvW function intended to be used?"
			// http://blogs.msdn.com/b/oldnewthing/archive/2010/09/16/10062818.aspx
			// And: "The first word on the command line is the program name only by convention"
			// http://blogs.msdn.com/b/oldnewthing/archive/2006/05/15/597984.aspx
			int numberOfArgs;
			IntPtr ptrToSplitArgs = CommandLineToArgvW(commandLine, out numberOfArgs);

			// CommandLineToArgvW returns NULL upon failure.
			if (ptrToSplitArgs == IntPtr.Zero)
			{
				// The inner Win32Exception will use the last error code set by CommandLineToArgvW.
				throw Exceptions.Log(new ArgumentException("Unable to split command line: " + commandLine, new Win32Exception()));
			}

			// Make sure the memory ptrToSplitArgs to is freed, even upon failure.
			try
			{
				string[] splitArgs = new string[numberOfArgs];

				// ptrToSplitArgs is an array of pointers to null terminated Unicode strings.
				// Copy each of these strings into our split argument array.
				for (int i = 0; i < numberOfArgs; i++)
				{
					IntPtr lpwstr = Marshal.ReadIntPtr(ptrToSplitArgs, i * IntPtr.Size);
					splitArgs[i] = Marshal.PtrToStringUni(lpwstr);
				}

				return splitArgs;
			}
			finally
			{
				// Free memory obtained by CommandLineToArgW.
				// In .NET the LocalFree API is exposed by Marshal.FreeHGlobal.
				Marshal.FreeHGlobal(ptrToSplitArgs);
			}
		}

		[SuppressMessage(
			"Microsoft.Usage",
			"CA2201:DoNotRaiseReservedExceptionTypes",
			Justification = "We have to use PreserveSig, check for S_OK or ERROR_CANCELLED, and throw otherwise.")]
		internal static string SelectFolder(IntPtr? ownerHandle, string title, string initialFolder)
		{
			// This uses the "new" IFileDialog implementation rather than the old, awful SHBrowseForFolder dialog.
			// This implementation was pieced together from several C# and C++ examples:
			// http://www.jmedved.com/2011/12/openfolderdialog/
			// http://stackoverflow.com/a/15386992/1882616
			// http://stackoverflow.com/questions/600346/using-openfiledialog-for-directory-not-folderbrowserdialog
			// http://msdn.microsoft.com/en-us/library/windows/desktop/bb776913(v=vs.85).aspx
			//
			// Note: Because we're using strongly-typed interfaces here for IFileDialog and IShellItem,
			// we don't have to mess with Marshal.ReleaseComObject in this method.  .NET will manage
			// the lifetimes of the RCWs that it creates.
			IFileDialog dialog = (IFileDialog)new FileOpenDialog();

			// The MSDN examples say we should always do GetOptions before SetOptions to avoid overriding default options.
			uint options;
			dialog.GetOptions(out options);

			const uint FOS_PICKFOLDERS = 0x00000020;
			const uint FOS_FORCEFILESYSTEM = 0x00000040;
			const uint FOS_NOVALIDATE = 0x00000100;
			const uint FOS_NOTESTFILECREATE = 0x00010000;
			const uint FOS_DONTADDTORECENT = 0x02000000;
			options |= FOS_PICKFOLDERS | FOS_FORCEFILESYSTEM | FOS_NOVALIDATE | FOS_NOTESTFILECREATE | FOS_DONTADDTORECENT;
			dialog.SetOptions(options);

			// Default to the current directory in case the initial folder is missing or invalid.
			// Windows only uses this for the first usage of the dialog with the current title
			// in the current process's path.  For subsequent calls, Windows remembers
			// and starts from the previous path.
			IShellItem currentDirectoryItem = GetShellItemForPath(Environment.CurrentDirectory);
			if (currentDirectoryItem != null)
			{
				dialog.SetDefaultFolder(currentDirectoryItem);
			}

			if (!string.IsNullOrEmpty(initialFolder))
			{
				IShellItem initialFolderItem = GetShellItemForPath(initialFolder);
				if (initialFolderItem != null)
				{
					dialog.SetFolder(initialFolderItem);
				}
			}

			if (!string.IsNullOrEmpty(title))
			{
				dialog.SetTitle(title);
			}

			uint showResult = dialog.Show(ownerHandle.GetValueOrDefault());
			const uint ERROR_CANCELLED = 0x800704C7; // For when the user clicks Cancel.
			string result = null;
			if (showResult == S_OK)
			{
				IShellItem shellItem;
				if (dialog.GetResult(out shellItem) == S_OK)
				{
					const uint SIGDN_FILESYSPATH = 0x80058000;
					IntPtr pszString;
					if (shellItem.GetDisplayName(SIGDN_FILESYSPATH, out pszString) == S_OK)
					{
						if (pszString != IntPtr.Zero)
						{
							try
							{
								result = Marshal.PtrToStringAuto(pszString);
							}
							finally
							{
								Marshal.FreeCoTaskMem(pszString);
							}
						}
					}
				}
			}
			else if (showResult != ERROR_CANCELLED)
			{
				throw new COMException("An error occurred while showing the Select Folder dialog.", unchecked((int)showResult));
			}

			return result;
		}

		internal static string GetShellFileTypeAndIcon(string fileName, bool useExistingFile, IconOptions iconOptions, Action<IntPtr> useIconHandle)
		{
			Conditions.RequireString(fileName, () => fileName);
			Conditions.RequireArgument(
				(iconOptions == IconOptions.None && useIconHandle == null) || (iconOptions != IconOptions.None && useIconHandle != null),
				"The iconOptions and useIconHandle arguments must be compatible.");

			SHFILEINFO info = default(SHFILEINFO);
			int cbFileInfo = Marshal.SizeOf(info);
			SHGFI flags = SHGFI.TypeName;

			int fileAttributes = 0;
			if (!useExistingFile)
			{
				flags |= SHGFI.UseFileAttributes;
				const int FILE_ATTRIBUTE_NORMAL = 128;
				const int FILE_ATTRIBUTE_DIRECTORY = 16;

				// http://stackoverflow.com/questions/1599235/how-do-i-fetch-the-folder-icon-on-windows-7-using-shell32-shgetfileinfo
				fileAttributes = iconOptions.HasFlag(IconOptions.Folder) ? FILE_ATTRIBUTE_DIRECTORY : FILE_ATTRIBUTE_NORMAL;
			}

			bool useIcon = iconOptions != IconOptions.None;
			if (useIcon)
			{
				flags |= SHGFI.Icon;

				// The Small and Large options are mutually exclusive.  If they specify neither or both, then we'll default to small.
				if (iconOptions.HasFlag(IconOptions.Small) || !iconOptions.HasFlag(IconOptions.Large))
				{
					flags |= SHGFI.SmallIcon;
				}
				else
				{
					flags |= SHGFI.LargeIcon;
				}

				if (iconOptions.HasFlag(IconOptions.ShellSize))
				{
					flags |= SHGFI.ShellIconSize;
				}

				if (iconOptions.HasFlag(IconOptions.Open))
				{
					flags |= SHGFI.OpenIcon;
				}

				if (iconOptions.HasFlag(IconOptions.Shortcut))
				{
					flags |= SHGFI.LinkOverlay;
				}

				if (iconOptions.HasFlag(IconOptions.Selected))
				{
					flags |= SHGFI.Selected;
				}
			}

			string result = null;
			if (SHGetFileInfo(fileName, fileAttributes, out info, (uint)cbFileInfo, flags) != IntPtr.Zero)
			{
				result = info.szTypeName;
				if (useIcon && useIconHandle != null)
				{
					// The caller has to make a copy (e.g., Icon.FromHandle(hIcon).Clone()).
					useIconHandle(info.hIcon);
					DestroyIcon(info.hIcon);
				}
			}

			return result;
		}

		#endregion

		#region Private Methods

		private static IShellItem GetShellItemForPath(string path)
		{
			IShellItem result;
			var riid = new Guid(IID_IShellItem);
			if (SHCreateItemFromParsingName(path, IntPtr.Zero, ref riid, out result) != S_OK)
			{
				// If the user types an invalid "path" into an input box, we don't want to raise an exception.
				result = null;
			}

			return result;
		}

		#endregion

		#region Private Extern Methods

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern uint GetModuleFileName([In] IntPtr module, [Out] StringBuilder fileNameBuffer, [In, MarshalAs(UnmanagedType.U4)] int bufferSize);

		[DllImport("kernel32.dll")]
		private static extern ErrorModes SetErrorMode(ErrorModes uMode);

		[DllImport("shell32.dll", SetLastError = true)]
		private static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

		[DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern int SHCreateItemFromParsingName(
			[MarshalAs(UnmanagedType.LPWStr)] string pszPath,
			IntPtr pbc,
			ref Guid riid,
			[MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);

		[DllImport("shell32.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr SHGetFileInfo(string pszPath, int dwFileAttributes, out SHFILEINFO psfi, uint cbfileInfo, SHGFI uFlags);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DestroyIcon(IntPtr handle);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

		#endregion

		#region Private Types

		#region SHFILEINFO

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct SHFILEINFO
		{
#pragma warning disable CC0074 // Make field readonly
			public IntPtr hIcon;
			public int iIcon;
			public uint dwAttributes;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] // MAX_PATH
			public string szDisplayName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
#pragma warning restore CC0074 // Make field readonly
		}

		#endregion

		#region FileOpenDialog

		// Microsoft defines this coclass.  We can't declare it as sealed because then the compiler won't allow us to cast it to IFileDialog.
		[Guid(CLSID_FileOpenDialog)]
		[ComImport]
		[ClassInterface(ClassInterfaceType.None)]
		[TypeLibType(TypeLibTypeFlags.FCanCreate)]
		private class FileOpenDialog
		{
		}

		#endregion

		#endregion
	}
}