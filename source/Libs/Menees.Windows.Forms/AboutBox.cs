namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Windows.Forms;
	using Menees.Shell;

	#endregion

	internal partial class AboutBox : ExtendedForm
	{
		#region Constructors

		public AboutBox(Assembly callingAssembly)
		{
			this.InitializeComponent();

			string applicationName = ApplicationInfo.ApplicationName;
			this.Text = "About " + applicationName;
			this.productName.Text = applicationName;
			this.version.Text = ShellUtility.GetVersionInfo(callingAssembly);
			this.copyright.Text = ShellUtility.GetCopyrightInfo(callingAssembly);
		}

		#endregion

		#region Public Methods

		public void Execute(IWin32Window owner)
		{
			bool useDesktopAsOwner = owner == null;

			Control control = owner as Control;
			if (control != null)
			{
				Form form = control.FindForm();
				if (form != null)
				{
					this.icon.Image = form.Icon.ToBitmap();

					// This is important for tray icon apps where the main form may be hidden.
					useDesktopAsOwner = !form.Visible;
				}
				else
				{
					useDesktopAsOwner = true;
				}
			}
			else if (owner != null)
			{
				// We should only get here for a WPF app or an unmanaged app.
				// From WinUser.h
				const int WM_GETICON = 0x007F;
				IntPtr iconBig = (IntPtr)1;
				IntPtr iconHandle = NativeMethods.SendMessage(new HandleRef(null, owner.Handle), WM_GETICON, iconBig, IntPtr.Zero);
				if (iconHandle != IntPtr.Zero)
				{
					using (Icon ownerIcon = Icon.FromHandle(iconHandle))
					{
						if (ownerIcon != null)
						{
							this.icon.Image = ownerIcon.ToBitmap();
						}
					}
				}
			}

			if (useDesktopAsOwner)
			{
				// If there's no owner window, then this dialog should be centered on the screen.
				this.StartPosition = FormStartPosition.CenterScreen;
				this.ShowInTaskbar = true;
			}

			if (this.icon.Image == null)
			{
				using (Icon appIcon = Icon.ExtractAssociatedIcon(ApplicationInfo.ExecutableFile))
				{
					this.icon.Image = appIcon.ToBitmap();
				}
			}

			this.ShowDialog(owner);
		}

		#endregion

		#region Private Methods

		private void WebLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (WindowsUtility.ShellExecute(this, this.webLink.Text))
			{
				this.webLink.Links[0].Visited = true;
			}
		}

		private void EmailLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			// UriBuilder always adds a '/' after the email address, which shows up in the opened
			// mail message's To field.  So we'll manually build a mailto-compatible Uri.
			string link = string.Format("mailto:{0}?subject={1} {2}", this.emailLink.Text, this.productName.Text, this.version.Text);
			if (WindowsUtility.ShellExecute(this, link))
			{
				this.emailLink.Links[0].Visited = true;
			}
		}

		#endregion
	}
}
