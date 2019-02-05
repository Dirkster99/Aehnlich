namespace Menees.Diffs.Controls
{
    using DiffLib.Enums;
    #region Using Directives

    using System;
	using System.ComponentModel;
	using System.Diagnostics.CodeAnalysis;
	using System.Drawing;
	using System.Windows.Forms;

	#endregion

	/// <summary>
	/// This shows a "colored line" overview of the diffs (sort of like WinDiff has on the left).
	/// Instead of blue vertical bars like WinDiff, it uses a translucent selection view window.
	/// </summary>
	internal sealed class DiffOverview : Control
	{
		#region Private Data Members

		private bool dragging;
		private BorderStyle borderStyle = BorderStyle.Fixed3D;
		private bool useTranslucentView = true;
		private Bitmap image;
		private DiffView view;
		private Rectangle viewRect;

		#endregion

		#region Constructors

		public DiffOverview()
		{
			// Set some important control styles
			this.SetStyle(ControlStyles.Opaque, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.StandardClick, true);
			this.SetStyle(ControlStyles.StandardDoubleClick, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);

			this.BackColor = SystemColors.Window;

			DiffOptions.OptionsChanged += this.DiffOptionsChanged;
		}

		#endregion

		#region Public Events

		/// <summary>
		/// Fired when the user clicks and/or drags to move the view.
		/// </summary>
		public event EventHandler<DiffLineClickEventArgs> LineClick;

		#endregion

		#region Public Properties

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
			Justification = "The get_DiffView accessor is only called by the Windows Forms designer via reflection.")]
		public DiffView DiffView
		{
			get
			{
				return this.view;
			}

			set
			{
				if (this.view != value)
				{
					// Detach from old view's events
					if (this.view != null)
					{
						this.view.SizeChanged -= this.DiffView_SizeChanged;
						this.view.VScrollPosChanged -= this.DiffView_VScrollPosChanged;
						this.view.LinesChanged -= this.DiffView_LinesChanged;
					}

					this.view = value;

					// Attach to new view's events
					if (this.view != null)
					{
						this.view.SizeChanged += this.DiffView_SizeChanged;
						this.view.VScrollPosChanged += this.DiffView_VScrollPosChanged;
						this.view.LinesChanged += this.DiffView_LinesChanged;
					}

					this.UpdateAll();
				}
			}
		}

		/// <summary>
		/// Total number of lines
		/// </summary>
		[Browsable(false)]
		public int LineCount => this.view != null ? this.view.LineCount : 0;

		[DefaultValue(true)]
		public bool UseTranslucentView
		{
			get
			{
				return this.useTranslucentView;
			}

			set
			{
				if (this.useTranslucentView != value)
				{
					this.useTranslucentView = value;
					this.InvalidateView();
				}
			}
		}

		/// <summary>
		/// Used to determine size of “view” window.
		/// </summary>
		[Browsable(false)]
		public int ViewLineCount
		{
			get
			{
				if (this.view != null)
				{
					return this.view.VisibleLineCount;
				}
				else
				{
					return 0;
				}
			}
		}

		/// <summary>
		/// Used to determine position of “view” window
		/// </summary>
		[Browsable(false)]
		public int ViewTopLine
		{
			get
			{
				if (this.view != null)
				{
					return this.view.FirstVisibleLine;
				}
				else
				{
					return 0;
				}
			}
		}

		#endregion

		#region Protected Properties

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams result = base.CreateParams;
				NativeMethods.SetBorderStyle(result, this.borderStyle);
				return result;
			}
		}

		#endregion

		#region Protected Methods

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.image != null)
				{
					this.image.Dispose();
				}

				DiffOptions.OptionsChanged -= this.DiffOptionsChanged;
			}

			base.Dispose(disposing);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (this.view != null && e.Button == MouseButtons.Left)
			{
				this.dragging = true;
				this.Capture = true;
				this.FireLineClicked(this.GetLineFromY(e.Y));
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (this.view != null && this.dragging && e.Button == MouseButtons.Left)
			{
				this.FireLineClicked(this.GetLineFromY(e.Y));
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if (this.view != null && this.dragging && e.Button == MouseButtons.Left)
			{
				this.Capture = false;
				this.dragging = false;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Graphics g = e.Graphics;

			if (this.image != null)
			{
				Rectangle r = e.ClipRectangle;
				g.DrawImage(this.image, r.X, r.Y, r, GraphicsUnit.Pixel);

				// Repaint the view window if any of it is invalid
				if (r.IntersectsWith(this.viewRect))
				{
					bool disposePen = false;
					Pen pen = SystemPens.Highlight;
					Rectangle penRect;

					if (this.UseTranslucentView)
					{
						// Set the alpha blend to 20% (51/256);
						using (SolidBrush b = new SolidBrush(Color.FromArgb(51, SystemColors.Highlight)))
						{
							r.Intersect(this.viewRect);
							g.FillRectangle(b, r);
						}

						// Draw the pen border with view rect.
						penRect = this.viewRect;
					}
					else
					{
						// Create a two pixel wide highlight pen.
						pen = new Pen(SystemColors.Highlight, 2);
						disposePen = true;

						// Because the lines will go back up a pixel
						// we have to shrink the bounds of the rect.
						penRect = new Rectangle(this.viewRect.X + 1, this.viewRect.Y + 1, this.viewRect.Width - 1, this.viewRect.Height - 1);
					}

					// Draw a Highlight Pen border.  In some cases, it will
					// draw a pixel too far (because we always round up), so
					// we'll check for that case here.  If we're scrolled to
					// the bottom, I don't want the last line cut off.
					int viewHeight = penRect.Height - 1;
					int usableHeight = this.ClientSize.Height - penRect.Y - 1;
					int height = Math.Min(viewHeight, usableHeight);
					g.DrawRectangle(pen, penRect.X, penRect.Y, penRect.Width - 1, height);

					if (disposePen)
					{
						pen.Dispose();
					}
				}
			}
			else
			{
				g.FillRectangle(SystemBrushes.Control, this.ClientRectangle);
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			this.UpdateAll();
			base.OnSizeChanged(e);
		}

		#endregion

		#region Private Methods

		private void CalculateViewRect()
		{
			if (this.ViewLineCount == 0)
			{
				this.viewRect = new Rectangle(0, 0, 0, 0);
			}
			else
			{
				int y = this.GetPixelLineHeight(this.ViewTopLine);
				this.viewRect.Location = new Point(0, y);

				int height = this.GetPixelLineHeight(this.ViewLineCount);
				this.viewRect.Size = new Size(this.ClientSize.Width, height);
			}
		}

		private void DiffOptionsChanged(object sender, EventArgs e)
		{
			// The diff colors changed, so we need to rerender the
			// image.  The current view rect should still be valid.
			this.RenderImage();
			this.Invalidate();
		}

		private void DiffView_LinesChanged(object sender, EventArgs e)
		{
			// If the Lines changed, we need to update everything.
			this.UpdateAll();
		}

		private void DiffView_SizeChanged(object sender, EventArgs e)
		{
			// If the DiffView size changed, then our view window
			// may be longer or shorter, but the rendered image is
			// still valid.  So we just need to recalc the view rect
			// and invalidate the whole window.
			this.CalculateViewRect();
			this.Invalidate();
		}

		private void DiffView_VScrollPosChanged(object sender, EventArgs e)
		{
			// The DiffView's FirstVisibleLine has changed, so we
			// just need to invalidate our view.
			this.InvalidateView();
		}

		private void FireLineClicked(int line)
		{
			if (this.LineClick != null)
			{
				// Force it in bounds
				line = DiffView.EnsureInRange(0, line, this.LineCount - 1);
				this.LineClick(this, new DiffLineClickEventArgs(line));
			}
		}

		private int GetLineFromY(int y)
		{
			double percent = ((double)y) / this.ClientSize.Height;
			int result = (int)(this.LineCount * percent);
			return result;
		}

		private int GetPixelLineHeight(int lines)
		{
			int result = (int)Math.Ceiling(this.GetPixelLineHeightF(lines));
			return result;
		}

		private float GetPixelLineHeightF(int lines)
		{
			float result = 0;

			if (this.LineCount > 0)
			{
				result = this.ClientSize.Height * (lines / (float)this.LineCount);
			}

			return result;
		}

		private void InvalidateView()
		{
			Rectangle oldViewRect = this.viewRect;
			this.CalculateViewRect();

			// If the old and new view rects are the
			// same, then there's nothing to do.
			if (oldViewRect != this.viewRect)
			{
				// If there is an old view rect, invalidate it first.
				if (!oldViewRect.IsEmpty)
				{
					this.Invalidate(oldViewRect);
				}

				// Invalidate the current view rectangle
				if (!this.viewRect.IsEmpty)
				{
					this.Invalidate(this.viewRect);
				}
			}
		}

		private void RenderImage()
		{
			if (this.image != null)
			{
				this.image.Dispose();
				this.image = null;
			}

			int width = this.ClientSize.Width;
			int height = this.ClientSize.Height;

			if (width > 0 && height > 0 && this.view != null && this.view.Lines != null)
			{
				// Draw a bitmap in memory that we can render from
				this.image = new Bitmap(width, height);
				using (Graphics g = Graphics.FromImage(this.image))
				using (SolidBrush backBrush = new SolidBrush(this.BackColor))
				{
					g.FillRectangle(backBrush, 0, 0, width, height);

					const float GutterWidth = 2.0F;

					// Make sure each line is at least 1 pixel high
					float lineHeight = (float)Math.Max(1.0, this.GetPixelLineHeightF(1));
					DiffViewLines lines = this.view.Lines;
					int numLines = lines.Count;
					for (int i = 0; i < numLines; i++)
					{
						DiffViewLine line = lines[i];
						if (line.Edited)
						{
							backBrush.Color = DiffOptions.GetColorForEditType(line.EditType);
							float y = this.GetPixelLineHeightF(i);
							float fullFillWidth = width - (2 * GutterWidth);

							switch (line.EditType)
							{
								case EditType.Change:

									// Draw all the way across
									g.FillRectangle(backBrush, GutterWidth, y, fullFillWidth, lineHeight);
									break;

								case EditType.Delete:

									// Draw delete on the left and dead space on the right.
									g.FillRectangle(backBrush, GutterWidth, y, fullFillWidth / 2, lineHeight);
									using (Brush deadBrush = DiffOptions.TryCreateDeadSpaceBrush(backBrush.Color))
									{
										g.FillRectangle(deadBrush ?? backBrush, GutterWidth + (fullFillWidth / 2), y, fullFillWidth / 2, lineHeight);
									}

									break;

								case EditType.Insert:

									// Draw dead space on the left and insert on the right.
									using (Brush deadBrush = DiffOptions.TryCreateDeadSpaceBrush(backBrush.Color))
									{
										g.FillRectangle(deadBrush ?? backBrush, GutterWidth, y, fullFillWidth / 2, lineHeight);
									}

									g.FillRectangle(backBrush, GutterWidth + (fullFillWidth / 2), y, fullFillWidth / 2, lineHeight);
									break;
							}
						}
					}
				}
			}
		}

		private void UpdateAll()
		{
			this.RenderImage();
			this.CalculateViewRect();
			this.Invalidate();
		}

		#endregion
	}
}
