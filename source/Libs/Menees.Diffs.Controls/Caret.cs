namespace Menees.Diffs.Controls
{
	#region Using Directives

	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Windows.Forms;

	#endregion

	/// <summary>
	/// Encapsulates the Win32 API for Carets.
	/// </summary>
	public sealed class Caret : IDisposable
	{
		#region Private Data Members

		private bool createdCaret;
		private bool visible;
		private Control control;
		private Point position;
		private Size size;

		#endregion

		#region Constructors and Destructors

		public Caret(Control control, int height)
			: this(control, 2, height)
		{
		}

		public Caret(Control control, int width, int height)
		{
			Conditions.RequireReference(control, "control");

			this.control = control;
			this.size = new Size(width, height);
			this.position = Point.Empty;

			this.control.GotFocus += this.ControlGotFocus;
			this.control.LostFocus += this.ControlLostFocus;

			// If the control already has focus, then create the caret.
			if (control.Focused)
			{
				this.ControlGotFocus(control, EventArgs.Empty);
			}
		}

		~Caret()
		{
			this.Dispose(false);
		}

		#endregion

		#region Public Properties

		public Control Control => this.control;

		public Point Position
		{
			get
			{
				if (this.createdCaret)
				{
					NativeMethods.GetCaretPos(ref this.position);
				}

				return this.position;
			}

			set
			{
				if (this.createdCaret)
				{
					NativeMethods.SetCaretPos(value.X, value.Y);
				}
				else
				{
					this.position = value;
				}
			}
		}

		public Size Size => this.size;

		public bool Visible
		{
			get
			{
				return this.visible;
			}

			set
			{
				if (this.visible != value)
				{
					this.visible = value;

					if (this.createdCaret)
					{
						if (value)
						{
							NativeMethods.ShowCaret(this.control);
						}
						else
						{
							NativeMethods.HideCaret(this.control);
						}
					}
				}
			}
		}

		#endregion

		#region Public Methods

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region Private Methods

		private void ControlGotFocus(object sender, EventArgs e)
		{
			// Sometimes in the debugger, we'll get focus without ever having been sent
			// the LostFocus event.  So I'll fire it now just to make things balance out.
			// I'd rather make sure we clean up the old caret and then recreate a new one.
			// That seems safer than trying to reuse an existing one if it's in a state we
			// don't think should occur normally.
			this.ControlLostFocus(sender, e);

			this.createdCaret = NativeMethods.CreateCaret(this.control, this.size.Width, this.size.Height);
			if (this.createdCaret)
			{
				NativeMethods.SetCaretPos(this.position.X, this.position.Y);
				this.Visible = true;
			}
		}

		private void ControlLostFocus(object sender, EventArgs e)
		{
			if (this.createdCaret)
			{
				this.Visible = false;
				NativeMethods.DestroyCaret();
				this.createdCaret = false;
			}
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.control.Focused)
				{
					this.ControlLostFocus(this.control, EventArgs.Empty);
				}

				this.control.GotFocus -= this.ControlGotFocus;
				this.control.LostFocus -= this.ControlLostFocus;
			}
			else if (this.createdCaret)
			{
				// The GC called our Finalize method, so we only
				// need to release unmanaged resources.
				NativeMethods.DestroyCaret();
			}
		}

		#endregion
	}
}
