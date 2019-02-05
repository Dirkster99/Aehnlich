namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Windows.Forms;

	#endregion

	/// <summary>
	/// Provides a "bare-bones" wrapper for a window handle received through interop.
	/// </summary>
	internal sealed class Win32Window : IWin32Window
	{
		#region Private Data Members

		private IntPtr windowHandle;

		#endregion

		#region Constructors

		private Win32Window(IntPtr windowHandle)
		{
			this.windowHandle = windowHandle;
		}

		#endregion

		#region IWin32Window Members

		/// <summary>
		/// Gets the handle for this window.
		/// </summary>
		public IntPtr Handle
		{
			get
			{
				return this.windowHandle;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns an IWin32Window wrapper for the specified window handle.
		/// </summary>
		/// <param name="windowHandle">A window handle.</param>
		/// <returns>An IWin32Window.</returns>
		public static IWin32Window FromHandle(IntPtr windowHandle)
		{
			return new Win32Window(windowHandle);
		}

		#endregion
	}
}
