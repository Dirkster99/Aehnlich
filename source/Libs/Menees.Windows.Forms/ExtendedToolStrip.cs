namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	#endregion

	/// <summary>
	/// Exposes additional functionality for a ToolStrip.
	/// </summary>
	public class ExtendedToolStrip : ToolStrip
	{
		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ExtendedToolStrip()
		{
			this.MouseActivateCausesClick = true;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets whether a mouse activation message will also send
		/// the message on to child windows.
		/// </summary>
		/// <remarks>
		/// This defaults to true, which allows a single click to activate the current window
		/// and still be processed as a click by a child window.  Set this to false to get the
		/// standard ToolStrip behavior, which won't pass a mouse click on to a child window
		/// if the ToolStrip's parent window is initially inactive.
		/// </remarks>
		[DefaultValue(true)]
		[Description("Whether a mouse activation message will also send the message on to child windows.")]
		public bool MouseActivateCausesClick
		{
			get;
			set;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Processes window messages.
		/// </summary>
		protected override void WndProc(ref Message m)
		{
			const int WM_MOUSEACTIVATE = 0x0021;
			const int MA_ACTIVATE = 1;

			if (m.Msg == WM_MOUSEACTIVATE && this.MouseActivateCausesClick)
			{
				// The base ToolStrip returns MA_ACTIVATEANDEAT, which means the
				// mouse click will get thrown away before clicking a button.  Instead,
				// we'll return MA_ACTIVATE, so a button will get the click even when
				// the form wasn't initially active.
				//
				// http://stackoverflow.com/questions/7121624/tool-strip-container-tools-strip-lost-focus-and-double-click
				// http://plainwonders.blogspot.com/2007/10/why-toolstrip-responses-to.html
				// http://msdn.microsoft.com/en-us/library/windows/desktop/ms645612.aspx
				m.Result = new IntPtr(MA_ACTIVATE);
			}
			else
			{
				base.WndProc(ref m);
			}
		}

		#endregion
	}
}
