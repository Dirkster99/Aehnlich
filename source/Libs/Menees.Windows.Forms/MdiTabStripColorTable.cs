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

	internal sealed class MdiTabStripColorTable : ProfessionalColorTable
	{
		#region Private Data Members

		private MdiTabStrip tabStrip;

		#endregion

		#region Constructors

		internal MdiTabStripColorTable(MdiTabStrip tabStrip)
		{
			this.tabStrip = tabStrip;
		}

		#endregion

		#region Public Properties

		public override Color ButtonCheckedGradientBegin => SelectedTab;

		public override Color ButtonCheckedGradientEnd => this.ButtonCheckedGradientBegin;

		public override Color ButtonPressedGradientBegin => Pressed;

		public override Color ButtonPressedGradientEnd => this.ButtonPressedGradientBegin;

		public override Color ButtonSelectedBorder => SelectedTabBorder;

		public override Color ToolStripGradientBegin
		{
			get
			{
				// If the tab strip has a custom back color, then use it.
				// Otherwise, use our own default back color.
				Color result = this.tabStrip.BackColor;
				if (result == Control.DefaultBackColor)
				{
					result = BackColor;
				}

				return result;
			}
		}

		public override Color ToolStripGradientEnd => this.ToolStripGradientBegin;

		public override Color ToolStripGradientMiddle => this.ToolStripGradientBegin;

		#endregion

		#region Internal Properties

		internal static Color SelectedTab => SystemColors.ButtonHighlight;

		internal static Color SelectedTabBorder => SystemColors.Highlight;

		#endregion

		#region Private Properties

		private static Color Pressed => SelectedTab;

		private static Color BackColor => SystemColors.InactiveCaption;

		#endregion
	}
}
