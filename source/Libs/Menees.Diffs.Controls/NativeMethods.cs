namespace Menees.Diffs.Controls
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Drawing;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Windows.Forms;

	#endregion

	internal static class NativeMethods
	{
		#region Public Constants

		// ScrollBars for Get/SetScrollInfo
		public const int SB_HORZ = 0;
		public const int SB_VERT = 1;

		// Scroll Bar Commands
		public const int SB_LINEUP = 0; // SB_LINELEFT
		public const int SB_LINEDOWN = 1; // SB_LINERIGHT
		public const int SB_PAGEUP = 2; // SB_PAGELEFT
		public const int SB_PAGEDOWN = 3; // SB_PAGERIGHT
		public const int SB_THUMBTRACK = 5;
		public const int SB_TOP = 6; // SB_LEFT
		public const int SB_BOTTOM = 7; // SB_RIGHT

		public const int DLGC_WANTARROWS = 0x0001; /* Control wants arrow keys */
		public const int WM_HSCROLL = 0x0114;
		public const int WM_VSCROLL = 0x0115;
		public const int WM_MOUSEWHEEL = 0x020A;

		public const int WS_EX_CLIENTEDGE = 0x00000200;
		public const int WS_VSCROLL = 0x00200000;
		public const int WS_HSCROLL = 0x00100000;
		public const int WS_BORDER = 0x00800000;

		#endregion

		#region Private Data Members

		private const int SIF_RANGE = 0x0001;
		private const int SIF_PAGE = 0x0002;
		private const int SIF_POS = 0x0004;
		private const int SIF_DISABLENOSCROLL = 0x0008;
		private const int SIF_TRACKPOS = 0x0010;
		private const int SIF_ALL = SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS;

		#endregion

		#region Public Caret Methods

		public static bool CreateCaret(IWin32Window Ctrl, int nWidth, int nHeight) => CreateCaret(Ctrl.Handle, IntPtr.Zero, nWidth, nHeight);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DestroyCaret();

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetCaretPos(ref Point lpPoint);

		public static bool HideCaret(IWin32Window Ctrl) => HideCaret(Ctrl.Handle);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetCaretPos(int X, int Y);

		public static bool ShowCaret(IWin32Window Ctrl) => ShowCaret(Ctrl.Handle);

		#endregion

		#region Public Scrolling Methods

		public static ScrollInfo GetScrollInfo(IWin32Window Ctrl, bool bHorz)
		{
			ScrollInfo Info = ScrollInfo.Create(SIF_ALL);
			GetScrollInfo(Ctrl.Handle, bHorz ? SB_HORZ : SB_VERT, ref Info);
			return Info;
		}

		public static int GetScrollPage(IWin32Window Ctrl, bool bHorz)
		{
			ScrollInfo Info = ScrollInfo.Create(SIF_PAGE);
			if (GetScrollInfo(Ctrl.Handle, bHorz ? SB_HORZ : SB_VERT, ref Info))
			{
				return (int)Info.nPage;
			}
			else
			{
				return 0;
			}
		}

		public static int GetScrollPos(IWin32Window Ctrl, bool bHorz)
		{
			ScrollInfo Info = ScrollInfo.Create(SIF_POS);
			if (GetScrollInfo(Ctrl.Handle, bHorz ? SB_HORZ : SB_VERT, ref Info))
			{
				return Info.nPos;
			}
			else
			{
				return 0;
			}
		}

		public static int ScrollWindow(IWin32Window Ctrl, int dx, int dy, ref Rectangle rcScroll, ref Rectangle rcClip)
			=> ScrollWindow(Ctrl.Handle, dx, dy, ref rcScroll, ref rcClip);

		public static int ScrollWindow(IWin32Window Ctrl, int dx, int dy) => ScrollWindow(Ctrl.Handle, dx, dy, (IntPtr)0, (IntPtr)0);

		public static void SetScrollPageAndRange(IWin32Window Ctrl, bool bHorz, int iMin, int iMax, int iPage)
		{
			ScrollInfo Info = ScrollInfo.Create(SIF_RANGE | SIF_PAGE);
			Info.nMin = iMin;
			Info.nMax = iMax;
			Info.nPage = (uint)iPage;
			SetScrollInfo(Ctrl.Handle, bHorz ? SB_HORZ : SB_VERT, ref Info, 1);
		}

		public static void SetScrollPos(IWin32Window Ctrl, bool bHorz, int iPos)
		{
			ScrollInfo Info = ScrollInfo.Create(SIF_POS);
			Info.nPos = iPos;
			SetScrollInfo(Ctrl.Handle, bHorz ? SB_HORZ : SB_VERT, ref Info, 1);
		}

		#endregion

		#region Public Miscellaneous Methods

		public static void SetBorderStyle(CreateParams P, BorderStyle Style)
		{
			switch (Style)
			{
				case BorderStyle.Fixed3D:
					P.Style = P.Style & ~WS_BORDER;
					P.ExStyle = P.ExStyle | WS_EX_CLIENTEDGE;
					break;

				case BorderStyle.FixedSingle:
					P.Style = P.Style | WS_BORDER;
					P.ExStyle = P.ExStyle & ~WS_EX_CLIENTEDGE;
					break;

				case BorderStyle.None:
					P.Style = P.Style & ~WS_BORDER;
					P.ExStyle = P.ExStyle & ~WS_EX_CLIENTEDGE;
					break;
			}
		}

		#endregion

		#region Private Caret Methods

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool HideCaret(IntPtr hWnd);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool ShowCaret(IntPtr hWnd);

		#endregion

		#region Private Scrolling Methods

		[DllImport("User32.Dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetScrollInfo(IntPtr hWnd, int nBar, ref ScrollInfo Info);

		[DllImport("User32.Dll")]
		private static extern int ScrollWindow(IntPtr hWnd, int dx, int dy, ref Rectangle rcScroll, ref Rectangle rcClip);

		[DllImport("User32.Dll")]
		private static extern int ScrollWindow(IntPtr hWnd, int dx, int dy, IntPtr prcScroll, IntPtr prcClip);

		[DllImport("User32.Dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetScrollInfo(IntPtr hWnd, int nBar, ref ScrollInfo Info, int bRedraw);

		#endregion

		#region WinUser.h Types

		[StructLayout(LayoutKind.Sequential)]
		public struct ScrollInfo
		{
			public uint cbSize;
			public uint fMask;
			public int nMin;
			public int nMax;
			public uint nPage;
			public int nPos;
			public int nTrackPos;

			public static ScrollInfo Create(uint uiMask)
			{
				ScrollInfo Info = new ScrollInfo();
				Info.cbSize = (uint)Marshal.SizeOf(typeof(ScrollInfo));
				Info.fMask = uiMask;
				return Info;
			}
		}

		#endregion
	}
}
