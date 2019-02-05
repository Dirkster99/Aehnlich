namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	#endregion

	/// <summary>
	/// A panel container that can paint a themed border.
	/// </summary>
	/// <remarks>
	/// This is useful for containing a RichTextBox that has its BorderStyle set to None.
	/// RichTextBox doesn't support a themed border in .NET, but if you set the RichTextBox's
	/// border to None and embed it in an ExtendedPanel with UseVisualStyleBorder
	/// set to true then it at least appears to use a themed border.
	/// </remarks>
	public sealed class ExtendedPanel : Panel
	{
		#region Private Data Members

		private bool useVisualStyleBorder;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ExtendedPanel()
		{
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

			this.UseVisualStyleBorder = true;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets whether the panel should use a themed border if the BorderStyle is None.
		/// </summary>
		[DefaultValue(true)]
		[Description("Whether the panel should use a themed border if the BorderStyle is None.")]
		public bool UseVisualStyleBorder
		{
			get
			{
				return this.useVisualStyleBorder && this.BorderStyle == BorderStyle.None;
			}

			set
			{
				if (this.useVisualStyleBorder != value)
				{
					this.useVisualStyleBorder = value;
					this.Invalidate();
				}

				this.UpdateVisualStyleProperties();
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Handles the custom painting for the control.
		/// </summary>
		/// <param name="e">The paint event arguments.</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (this.UseVisualStyleBorder)
			{
				Rectangle rectClient = this.ClientRectangle;
				Rectangle rectBorder = new Rectangle(rectClient.X, rectClient.Y, rectClient.Width - 1, rectClient.Height - 1);
				if (Application.RenderWithVisualStyles)
				{
					ControlPaint.DrawVisualStyleBorder(e.Graphics, rectBorder);
				}
				else
				{
					ControlPaint.DrawBorder3D(e.Graphics, rectBorder, Border3DStyle.Sunken);
				}
			}
		}

		/// <summary>
		/// Called when the system colors change.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		protected override void OnSystemColorsChanged(EventArgs e)
		{
			base.OnSystemColorsChanged(e);
			this.UpdateVisualStyleProperties();
			this.Invalidate();
		}

		#endregion

		#region Private Methods

		private void UpdateVisualStyleProperties()
		{
			if (this.useVisualStyleBorder)
			{
				if (Application.RenderWithVisualStyles)
				{
					this.Padding = new Padding(SystemInformation.BorderSize.Width);
				}
				else
				{
					Size sz = SystemInformation.Border3DSize;
					this.Padding = new Padding(sz.Width, sz.Height, sz.Width + 1, sz.Height + 1);
				}

				this.BorderStyle = BorderStyle.None;
			}
			else
			{
				this.Padding = Padding.Empty;
			}
		}

		#endregion
	}
}
