namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	#endregion

	/// <summary>
	/// A single tab in an <see cref="MdiTabStrip"/>.
	/// </summary>
	public sealed class MdiTab : ToolStripButton
	{
		#region Private Data Members

		private const int CloseImagePixelSpace = 10;
		private const int CloseButtonSpace = 14;

		#endregion

		#region Constructors

		internal MdiTab(Form form)
			: base(form.Text)
		{
			this.ImageScaling = ToolStripItemImageScaling.None;
			this.ImageAlign = ContentAlignment.MiddleLeft;
			this.TextAlign = ContentAlignment.MiddleLeft;
			this.CheckOnClick = true;
			this.AutoToolTip = false;
			this.AssociatedForm = form;
			this.Padding = new Padding(0, 0, CloseButtonSpace, 0);
		}

		#endregion

		#region Internal Events

		internal event EventHandler CloseClicked;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the form associated with the current tab.
		/// </summary>
		public Form AssociatedForm
		{
			get;
			internal set;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Overrides <see cref="ToolStripButton.OnPaint"/>.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			// Draw the Close "button" if necessary.
			Rectangle closeRect = this.GetCloseButtonRect();
			bool clipIncludesCloseRect = e.ClipRectangle.IntersectsWith(closeRect);
			if (clipIncludesCloseRect && (this.Selected || this.Checked))
			{
				Graphics graph = e.Graphics;

				Point mousePositionInStrip = this.Owner.PointToClient(Cursor.Position);
				Rectangle bounds = this.Bounds;
				Point mousePosition = new Point(mousePositionInStrip.X - bounds.X, mousePositionInStrip.Y - bounds.Y);

				Bitmap closeImage = Properties.Resources.CloseGray;
				bool mouseOnClose = closeRect.Contains(mousePosition);
				if (mouseOnClose)
				{
					closeImage = Properties.Resources.CloseBlack;
					Color closeBackColor, closeBorderColor;
					this.GetCloseButtonColors(out closeBackColor, out closeBorderColor);

					using (Brush backBrush = new SolidBrush(closeBackColor))
					{
						graph.FillRectangle(backBrush, closeRect);
					}

					using (Pen borderPen = new Pen(closeBorderColor))
					{
						graph.DrawRectangle(borderPen, closeRect.X, closeRect.Y, closeRect.Width - 1, closeRect.Height - 1);
					}
				}

				const int Offset = (CloseButtonSpace - CloseImagePixelSpace) / 2;
				Point pt = closeRect.Location;
				pt.Offset(Offset, Offset);
				graph.DrawImage(closeImage, pt);
			}
		}

		/// <summary>
		/// Overrides <see cref="ToolStripItem.OnMouseDown"/>.
		/// </summary>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.Button == MouseButtons.Left)
			{
				bool clickedClose = false;
				if (this.CloseClicked != null)
				{
					Rectangle closeRect = this.GetCloseButtonRect();
					if (closeRect.Contains(e.Location))
					{
						this.CloseClicked(this, EventArgs.Empty);
						clickedClose = true;
					}
				}

				if (!clickedClose)
				{
					MdiTabStrip owner = this.Owner as MdiTabStrip;
					if (owner != null && owner.AllowDrop && !this.IsOnOverflow)
					{
						// Perform the click to activate the tab.  Otherwise, the click
						// is lost when we capture the mouse to begin the drag.
						this.PerformClick();

						// This will begin the drag after a very short delay.
						owner.BeginDragTimer(this);
					}
				}
			}
			else if (e.Button.HasFlag(MouseButtons.Right) || e.Button.HasFlag(MouseButtons.Middle))
			{
				// Select the tab when the right or middle mouse buttons have been clicked on it.
				// Visual Studio makes a middle-click close the tab, but I don't like that behavior.
				this.PerformClick();
			}
		}

		/// <summary>
		/// Overrides <see cref="ToolStripItem.OnMouseUp"/>.
		/// </summary>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			MdiTabStrip owner = this.Owner as MdiTabStrip;
			if (owner != null)
			{
				owner.EndDragTimer(false);
			}
		}

		/// <summary>
		/// Overrides <see cref="ToolStripItem.OnMouseEnter"/>.
		/// </summary>
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			Rectangle closeRect = this.GetCloseButtonRect();
			this.Invalidate(closeRect);
		}

		/// <summary>
		/// Overrides <see cref="ToolStripItem.OnMouseMove"/>.
		/// </summary>
		protected override void OnMouseMove(MouseEventArgs mea)
		{
			base.OnMouseMove(mea);
			Rectangle closeRect = this.GetCloseButtonRect();
			this.Invalidate(closeRect);
		}

		/// <summary>
		/// Overrides <see cref="ToolStripItem.OnMouseLeave"/>.
		/// </summary>
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			Rectangle closeRect = this.GetCloseButtonRect();
			this.Invalidate(closeRect);
		}

		#endregion

		#region Private Methods

		private Rectangle GetCloseButtonRect()
		{
			Rectangle bounds = this.Bounds;
			int x = bounds.Width - CloseButtonSpace - 2;
			int y = (bounds.Height - CloseButtonSpace) / 2;
			Rectangle result = new Rectangle(x, y, CloseButtonSpace, CloseButtonSpace);
			return result;
		}

		private void GetCloseButtonColors(out Color closeBackColor, out Color closeBorderColor)
		{
			// Set the default colors.
			closeBackColor = MdiTabStripColorTable.SelectedTab;
			closeBorderColor = MdiTabStripColorTable.SelectedTabBorder;

			// Try to use the colors from the renderer's color table (if we can find one).
			ToolStrip tabStrip = this.Owner;
			if (tabStrip != null)
			{
				var renderer = tabStrip.Renderer as ToolStripProfessionalRenderer;
				if (renderer != null)
				{
					ProfessionalColorTable colorTable = renderer.ColorTable;
					if (colorTable != null)
					{
						closeBackColor = colorTable.ButtonCheckedGradientBegin;
						closeBorderColor = colorTable.ButtonSelectedBorder;
					}
				}
			}
		}

		#endregion
	}
}
