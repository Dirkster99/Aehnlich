namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Drawing;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Windows.Forms;

	#endregion

	internal static class NativeMethods
	{
		#region Public Methods

		public static void BringWindowForward(IntPtr hWnd)
		{
			if (IsIconic(hWnd))
			{
				const int SW_RESTORE = 9;
				ShowWindowAsync(hWnd, SW_RESTORE);
			}

			SetForegroundWindow(hWnd);
		}

		public static Rectangle GetNormalWindowBounds(IWin32Window window)
		{
			WINDOWPLACEMENT placement = default(WINDOWPLACEMENT);
			placement.length = Marshal.SizeOf(placement);
			if (!GetWindowPlacement(new HandleRef(null, window.Handle), ref placement))
			{
				throw CreateExceptionForLastError();
			}

			// See the notes in our WINDOWPLACEMENT struct for why the original
			// RECT isn't the same as a .NET Rectangle and why we have to de-skew it.
			Rectangle skewed = placement.rcNormalPosition;
			Rectangle result = new Rectangle(
				skewed.Left,
				skewed.Top,
				skewed.Width - skewed.Left,
				skewed.Height - skewed.Top);

			return result;
		}

		public static int GetScrollPos(IWin32Window control, bool bHorz)
		{
			const int SB_HORZ = 0;
			const int SB_VERT = 1;
			const int SIF_POS = 0x0004;

			int result = 0;
			ScrollInfo info = ScrollInfo.Create(SIF_POS);
			if (GetScrollInfo(control.Handle, bHorz ? SB_HORZ : SB_VERT, ref info) != 0)
			{
				result = info.pos;
			}

			return result;
		}

		public static int SendMessage(IWin32Window control, int iMsg, int wParam)
		{
			int result = SendMessage(control, iMsg, wParam, 0);
			return result;
		}

		public static int SendMessage(IWin32Window control, int iMsg, int wParam, int lParam)
		{
			int result = (int)SendMessage(control.Handle, iMsg, (IntPtr)wParam, (IntPtr)lParam);
			return result;
		}

		public static int SendMessage(IWin32Window control, int iMsg, int wParam, ref int lParam)
		{
			IntPtr lParamPtr = (IntPtr)lParam;
			IntPtr result = SendMessage(control.Handle, iMsg, (IntPtr)wParam, ref lParamPtr);
			lParam = (int)lParamPtr;
			return (int)result;
		}

		public static int SendMessage(IWin32Window control, int iMsg, ref int wParam, ref int lParam)
		{
			IntPtr wParamPtr = (IntPtr)wParam;
			IntPtr lParamPtr = (IntPtr)lParam;
			IntPtr result = SendMessage(control.Handle, iMsg, ref wParamPtr, ref lParamPtr);
			wParam = (int)wParamPtr;
			lParam = (int)lParamPtr;
			return (int)result;
		}

		public static int SendMessage(IWin32Window control, int iMsg, int wParam, ref IntPtr lParam)
		{
			IntPtr result = SendMessage(control.Handle, iMsg, (IntPtr)wParam, ref lParam);
			return (int)result;
		}

		public static void SetBorderStyle(Control control, BorderStyle borderStyle)
		{
			IntPtr handle = control.Handle;

			// http://www.codeproject.com/Articles/8489/Getting-a-quot-Handle-quot-on-the-MDI-Client
			const int GWL_STYLE = -16;
			const int GWL_EXSTYLE = -20;
			const int WS_BORDER = 0x00800000;
			const int WS_EX_CLIENTEDGE = 0x00000200;

			// Get styles using Win32 calls
			int style = GetWindowLong(handle, GWL_STYLE);
			int exStyle = GetWindowLong(handle, GWL_EXSTYLE);

			// Add or remove style flags as necessary.
			switch (borderStyle)
			{
				case BorderStyle.Fixed3D:
					exStyle |= WS_EX_CLIENTEDGE;
					style &= ~WS_BORDER;
					break;

				case BorderStyle.FixedSingle:
					exStyle &= ~WS_EX_CLIENTEDGE;
					style |= WS_BORDER;
					break;

				case BorderStyle.None:
					style &= ~WS_BORDER;
					exStyle &= ~WS_EX_CLIENTEDGE;
					break;
			}

			// Set the styles using Win32 calls
			if (SetWindowLong(handle, GWL_STYLE, style) == 0)
			{
				throw CreateExceptionForLastError();
			}

			if (SetWindowLong(handle, GWL_EXSTYLE, exStyle) == 0)
			{
				throw CreateExceptionForLastError();
			}

			// Force the control to repaint.
			control.Invalidate();
		}

		#endregion

		#region Public Dll Imports

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int GetDialogBaseUnits();

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

		#endregion

		#region Private Dll Imports

		[DllImport("user32.dll")]
		private static extern int GetScrollInfo(IntPtr hWnd, int nBar, ref ScrollInfo Info);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetWindowPlacement(HandleRef hWnd, ref WINDOWPLACEMENT lpwndpl);

		// We have to import this function multiple ways because sometimes the parameters
		// are pointers (i.e. references), and sometimes they're not.
		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, ref IntPtr wParam, ref IntPtr lParam);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref IntPtr lParam);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsIconic(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		[SuppressMessage("", "CC0072", Justification = "The Async suffix comes from the Win32 API.")]
		private static extern bool ShowWindowAsync(IntPtr hWnd, int commandShow);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int GetWindowLong(IntPtr hWnd, int Index);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int SetWindowLong(IntPtr hWnd, int Index, int Value);

		#endregion

		#region Private Methods

		private static Win32Exception CreateExceptionForLastError()
		{
			Win32Exception result = Exceptions.Log(new Win32Exception(Marshal.GetLastWin32Error()));
			return result;
		}

		#endregion

		#region Private Structures

		[StructLayout(LayoutKind.Sequential)]
		private struct ScrollInfo
		{
#pragma warning disable CC0074 // Make field readonly
			public uint size;
			public uint mask;
			public int min;
			public int max;
			public uint page;
			public int pos;
			public int trackPos;
#pragma warning restore CC0074 // Make field readonly

			public static ScrollInfo Create(uint mask)
			{
				ScrollInfo info = default(ScrollInfo);
				info.size = (uint)Marshal.SizeOf(typeof(ScrollInfo));
				info.mask = mask;
				return info;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct WINDOWPLACEMENT
		{
#pragma warning disable CC0074 // Make field readonly
			public int length;
			public int flags;
			public int showCmd;
			public Point ptMinPosition;
			public Point ptMaxPosition;

			// Note: This is actually a Win32 RECT, which is NOT a .NET Rectangle.
			// RECT stores {Left, Top, Right, Bottom}, which will get marshaled back
			// into Rectangle's {Left, Top, Width, Height} fields.  So the Rectangle's
			// size will be skewed by an offset of {Left, Top}.
			public Rectangle rcNormalPosition;
#pragma warning restore CC0074 // Make field readonly
		}

		#endregion
	}
}
