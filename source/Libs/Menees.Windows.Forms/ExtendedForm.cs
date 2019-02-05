namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows.Forms;

	#endregion

	/// <summary>
	/// Extends the base Form class to use the default system font at design-time and run-time.
	/// </summary>
	public partial class ExtendedForm : Form
	{
		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ExtendedForm()
		{
			// The idea for using a base class that sets the font came from: http://stackoverflow.com/a/4076183/1882616
			this.Font = SystemFonts.MessageBoxFont;
			this.InitializeComponent();
		}

		#endregion
	}
}
