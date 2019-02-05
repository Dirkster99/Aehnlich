namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Windows.Forms;

	#endregion

	internal sealed class FolderBrowser
	{
		#region Private Data Members

		private string selectedPath;

		#endregion

		#region Constructors

		private FolderBrowser()
		{
		}

		#endregion

		#region Public Methods

		public static bool SelectFolder(IWin32Window owner, string caption, ref string selectedPath)
		{
			FolderBrowser dlg = new FolderBrowser();
			bool result = dlg.InternalSelectFolder(owner, caption, ref selectedPath);
			return result;
		}

		#endregion

		#region Private Methods

		private static string GetPathFromPidl(IntPtr pidl)
		{
			string result = null;
			if (pidl != IntPtr.Zero)
			{
				const int MAX_PATH = 260; // From WinDef.h
				StringBuilder sb = new StringBuilder(MAX_PATH);
				if (NativeMethods.SHGetPathFromIDList(pidl, sb) != 0)
				{
					result = sb.ToString();
				}
			}

			return result;
		}

		private bool InternalSelectFolder(IWin32Window owner, string caption, ref string selectedPath)
		{
			Conditions.RequireState(
				Application.OleRequired() == System.Threading.ApartmentState.STA,
				"The FolderBrowserDialog can only be invoked on an STA thread.");

			// Declarations from ShlObj.h
			const uint BIF_RETURNONLYFSDIRS = 0x0001;  // For finding a folder to start document searching
			const uint BIF_EDITBOX = 0x0010;   // Add an editbox to the dialog
			const uint BIF_VALIDATE = 0x0020;   // insist on valid result (or CANCEL)
			const uint BIF_NEWDIALOGSTYLE = 0x0040;   // Use the new dialog layout with the ability to resize
			// Caller needs to call OleInitialize() before using this API
			// const uint BIF_USENEWUI = BIF_NEWDIALOGSTYLE + BIF_EDITBOX; // (0x0040 | 0x0010);
			const uint BIF_SHAREABLE = 0x8000;  // sharable resources displayed (remote shares, requires BIF_USENEWUI)
			const int MAX_PATH = 260; // From WinDef.h

			NativeMethods.BROWSEINFO bi = new NativeMethods.BROWSEINFO();
			bi.hwndOwner = owner != null ? owner.Handle : IntPtr.Zero;
			bi.pidlRoot = IntPtr.Zero;
			bi.pszDisplayName = Marshal.AllocHGlobal(MAX_PATH * Marshal.SystemDefaultCharSize);
			bi.lpszTitle = caption;
			bi.ulFlags = BIF_EDITBOX | BIF_NEWDIALOGSTYLE | BIF_SHAREABLE | BIF_VALIDATE | BIF_RETURNONLYFSDIRS;
			bi.lpfn = this.OnBrowseEvent;
			bi.lParam = IntPtr.Zero;
			bi.imageIndex = 0;

			try
			{
				// We have to store this so the callback can set the initial selection.
				this.selectedPath = selectedPath;

				IntPtr pidl = NativeMethods.SHBrowseForFolder(ref bi);
				bool result = pidl != IntPtr.Zero;
				if (result)
				{
					try
					{
						selectedPath = GetPathFromPidl(pidl);
					}
					finally
					{
						Marshal.FreeCoTaskMem(pidl);
					}
				}

				return result;
			}
			finally
			{
				Marshal.FreeHGlobal(bi.pszDisplayName);
			}
		}

		private int OnBrowseEvent(IntPtr hwnd, uint msg, IntPtr lparam, IntPtr lpdata)
		{
			int result = 0;
			string enteredPath = null;

			// Declarations from ShlObj.h
			const uint BFFM_INITIALIZED = 1;
			const uint BFFM_SELCHANGED = 2;
			const uint BFFM_VALIDATEFAILEDA = 3;
			const uint BFFM_VALIDATEFAILEDW = 4;
			const int WM_USER = 1024; // From WinUser.h
			const int BFFM_ENABLEOK = WM_USER + 101;
			const int BFFM_SETSELECTION = WM_USER + 103;

			switch (msg)
			{
				case BFFM_INITIALIZED:
					if (!string.IsNullOrEmpty(this.selectedPath))
					{
						NativeMethods.SendMessage(new HandleRef(null, hwnd), BFFM_SETSELECTION, (IntPtr)1, this.selectedPath);
					}

					break;

				case BFFM_SELCHANGED:
					IntPtr pidl = lparam;
					if (pidl != IntPtr.Zero)
					{
						string path = GetPathFromPidl(pidl);
						NativeMethods.SendMessage(new HandleRef(null, hwnd), BFFM_ENABLEOK, IntPtr.Zero, string.IsNullOrEmpty(path) ? IntPtr.Zero : (IntPtr)1);
					}

					break;

				case BFFM_VALIDATEFAILEDA: // We usually get this even on Unicode systems!
					enteredPath = Marshal.PtrToStringAnsi(lparam);
					goto case BFFM_VALIDATEFAILEDW; // Fall through to the next label.

				case BFFM_VALIDATEFAILEDW:
					// The path the user typed in doesn't exist, so we'll display a message
					// and keep the dialog open (by returning 1).
					if (enteredPath == null)
					{
						enteredPath = Marshal.PtrToStringUni(lparam);
					}

					MessageBox.Show(
						Win32Window.FromHandle(hwnd),
						"The entered folder name (" + enteredPath + ") is invalid or does not exist.",
						"Invalid Folder",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					result = 1;
					break;
			}

			return result;
		}

		#endregion
	}
}
