namespace Diff.Net
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;
	using System.Xml;
	using Menees.Windows.Forms;

	#endregion

	internal sealed partial class CompareOptionsControl : ExtendedUserControl
	{
		#region Constructors

		public CompareOptionsControl()
		{
			this.InitializeComponent();
		}

		#endregion

		#region Public Properties

		[DefaultValue(true)]
		public bool ShowBinaryOptions
		{
			get
			{
				return this.compareBinary.Enabled;
			}

			set
			{
				if (this.ShowBinaryOptions != value)
				{
					this.compareBinary.Enabled = value;
					this.compareBinary.Visible = value;
					this.binaryFootprintLength.Enabled = value;
					this.binaryFootprintLength.Visible = value;
					this.binaryLabel.Enabled = value;
					this.binaryLabel.Visible = value;
				}
			}
		}

		#endregion

		#region Public Methods

		public void SaveOptions()
		{
			if (this.compareAuto.Checked)
			{
				Options.CompareType = CompareType.Auto;
			}
			else if (this.compareText.Checked)
			{
				Options.CompareType = CompareType.Text;
			}
			else if (this.compareXml.Checked)
			{
				Options.CompareType = CompareType.Xml;
			}

			if (this.ShowBinaryOptions)
			{
				if (this.compareBinary.Checked)
				{
					Options.CompareType = CompareType.Binary;
				}

				Options.BinaryFootprintLength = (int)this.binaryFootprintLength.Value;
			}

			Options.IgnoreXmlWhitespace = this.ignoreXmlWhitespace.Checked;
			Options.IgnoreCase = this.ignoreCase.Checked;
			Options.IgnoreTextWhitespace = this.ignoreTextWhitespace.Checked;
		}

		#endregion

		#region Private Event Handlers

		private void CompareOptionsControl_Load(object sender, EventArgs e)
		{
			// Use Auto by default.  If the last compare type used was Binary,
			// and they load a TextDiffForm with the binary options hidden,
			// then we need this to make sure the compare type gets set
			// back to one of the visible types.
			this.compareAuto.Checked = true;
			this.compareText.Checked = Options.CompareType == CompareType.Text;
			this.compareXml.Checked = Options.CompareType == CompareType.Xml;
			if (this.ShowBinaryOptions)
			{
				this.compareBinary.Checked = Options.CompareType == CompareType.Binary;
			}

			this.ignoreXmlWhitespace.Checked = Options.IgnoreXmlWhitespace;
			this.ignoreCase.Checked = Options.IgnoreCase;
			this.ignoreTextWhitespace.Checked = Options.IgnoreTextWhitespace;
			this.binaryFootprintLength.Value = Options.BinaryFootprintLength;
		}

		#endregion
	}
}
