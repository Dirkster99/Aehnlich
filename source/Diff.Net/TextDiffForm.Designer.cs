using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Menees;
using Menees.Diffs;
using Menees.Diffs.Controls;
using Menees.Windows.Forms;
namespace Diff.Net
{
	partial class TextDiffForm
	{
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnSwap;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel pnlText;
		private System.Windows.Forms.Splitter Split;
		private CompareOptionsControl optionsControl;
		private Menees.Windows.Forms.ExtendedRichTextBox txtA;
		private Menees.Windows.Forms.ExtendedRichTextBox txtB;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextDiffForm));
			this.btnSwap = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.pnlText = new System.Windows.Forms.Panel();
			this.txtB = new Menees.Windows.Forms.ExtendedRichTextBox();
			this.Split = new System.Windows.Forms.Splitter();
			this.txtA = new Menees.Windows.Forms.ExtendedRichTextBox();
			this.optionsControl = new Diff.Net.CompareOptionsControl();
			this.pnlText.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnSwap
			// 
			this.btnSwap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSwap.Location = new System.Drawing.Point(523, 323);
			this.btnSwap.Name = "btnSwap";
			this.btnSwap.Size = new System.Drawing.Size(90, 29);
			this.btnSwap.TabIndex = 2;
			this.btnSwap.Text = "&Swap";
			this.btnSwap.Click += new System.EventHandler(this.Swap_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(523, 403);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(90, 28);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.Cancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(523, 363);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(90, 28);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.OK_Click);
			// 
			// pnlText
			// 
			this.pnlText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pnlText.Controls.Add(this.txtB);
			this.pnlText.Controls.Add(this.Split);
			this.pnlText.Controls.Add(this.txtA);
			this.pnlText.Location = new System.Drawing.Point(0, 0);
			this.pnlText.Name = "pnlText";
			this.pnlText.Size = new System.Drawing.Size(624, 311);
			this.pnlText.TabIndex = 0;
			this.pnlText.Resize += new System.EventHandler(this.TextPanel_Resize);
			// 
			// txtB
			// 
			this.txtB.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtB.Location = new System.Drawing.Point(373, 0);
			this.txtB.Name = "txtB";
			this.txtB.RichTextShortcutsEnabled = false;
			this.txtB.Size = new System.Drawing.Size(251, 311);
			this.txtB.TabIndex = 2;
			this.txtB.TextChanged += new System.EventHandler(this.TextBox_Changed);
			// 
			// Split
			// 
			this.Split.Location = new System.Drawing.Point(370, 0);
			this.Split.Name = "Split";
			this.Split.Size = new System.Drawing.Size(3, 311);
			this.Split.TabIndex = 1;
			this.Split.TabStop = false;
			// 
			// txtA
			// 
			this.txtA.Dock = System.Windows.Forms.DockStyle.Left;
			this.txtA.Location = new System.Drawing.Point(0, 0);
			this.txtA.Name = "txtA";
			this.txtA.RichTextShortcutsEnabled = false;
			this.txtA.Size = new System.Drawing.Size(370, 311);
			this.txtA.TabIndex = 0;
			this.txtA.TextChanged += new System.EventHandler(this.TextBox_Changed);
			// 
			// optionsControl
			// 
			this.optionsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.optionsControl.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.optionsControl.Location = new System.Drawing.Point(67, 315);
			this.optionsControl.Name = "optionsControl";
			this.optionsControl.ShowBinaryOptions = false;
			this.optionsControl.Size = new System.Drawing.Size(448, 119);
			this.optionsControl.TabIndex = 1;
			// 
			// TextDiffForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(624, 441);
			this.Controls.Add(this.optionsControl);
			this.Controls.Add(this.pnlText);
			this.Controls.Add(this.btnSwap);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(528, 369);
			this.Name = "TextDiffForm";
			this.Text = "Compare Text";
			this.Closed += new System.EventHandler(this.TextDiffForm_Closed);
			this.Load += new System.EventHandler(this.TextDiffDlg_Load);
			this.pnlText.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
	}
}

