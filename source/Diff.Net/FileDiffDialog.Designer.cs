using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Menees;
using Menees.Diffs;
using Menees.Windows.Forms;
namespace Diff.Net
{
	partial class FileDiffDialog
	{
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnLeft;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnRight;
		private System.Windows.Forms.Button btnSwap;
		private System.Windows.Forms.CheckBox chkOnlyIfShift;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox edtLeft;
		private System.Windows.Forms.TextBox edtRight;
		private System.Windows.Forms.Label lblLeft;
		private System.Windows.Forms.Label lblRight;
		private System.Windows.Forms.OpenFileDialog OpenDlg;
		private CompareOptionsControl optionsControl;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileDiffDialog));
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.edtLeft = new System.Windows.Forms.TextBox();
			this.edtRight = new System.Windows.Forms.TextBox();
			this.btnRight = new System.Windows.Forms.Button();
			this.btnLeft = new System.Windows.Forms.Button();
			this.OpenDlg = new System.Windows.Forms.OpenFileDialog();
			this.lblLeft = new System.Windows.Forms.Label();
			this.lblRight = new System.Windows.Forms.Label();
			this.btnSwap = new System.Windows.Forms.Button();
			this.chkOnlyIfShift = new System.Windows.Forms.CheckBox();
			this.optionsControl = new Diff.Net.CompareOptionsControl();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(374, 275);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(90, 29);
			this.btnCancel.TabIndex = 10;
			this.btnCancel.Text = "Cancel";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(273, 275);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(90, 29);
			this.btnOK.TabIndex = 9;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.OK_Click);
			// 
			// edtLeft
			// 
			this.edtLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.edtLeft.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.edtLeft.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
			this.edtLeft.Location = new System.Drawing.Point(58, 15);
			this.edtLeft.Name = "edtLeft";
			this.edtLeft.Size = new System.Drawing.Size(369, 23);
			this.edtLeft.TabIndex = 1;
			this.edtLeft.TextChanged += new System.EventHandler(this.Edit_TextChanged);
			// 
			// edtRight
			// 
			this.edtRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.edtRight.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.edtRight.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
			this.edtRight.Location = new System.Drawing.Point(58, 49);
			this.edtRight.Name = "edtRight";
			this.edtRight.Size = new System.Drawing.Size(369, 23);
			this.edtRight.TabIndex = 4;
			this.edtRight.TextChanged += new System.EventHandler(this.Edit_TextChanged);
			// 
			// btnRight
			// 
			this.btnRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRight.Location = new System.Drawing.Point(436, 49);
			this.btnRight.Name = "btnRight";
			this.btnRight.Size = new System.Drawing.Size(29, 25);
			this.btnRight.TabIndex = 5;
			this.btnRight.Text = "...";
			this.btnRight.Click += new System.EventHandler(this.Right_Click);
			// 
			// btnLeft
			// 
			this.btnLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnLeft.Location = new System.Drawing.Point(436, 15);
			this.btnLeft.Name = "btnLeft";
			this.btnLeft.Size = new System.Drawing.Size(29, 24);
			this.btnLeft.TabIndex = 2;
			this.btnLeft.Text = "...";
			this.btnLeft.Click += new System.EventHandler(this.Left_Click);
			// 
			// OpenDlg
			// 
			this.OpenDlg.AddExtension = false;
			this.OpenDlg.Filter = resources.GetString("OpenDlg.Filter");
			// 
			// lblLeft
			// 
			this.lblLeft.AutoSize = true;
			this.lblLeft.Location = new System.Drawing.Point(10, 17);
			this.lblLeft.Name = "lblLeft";
			this.lblLeft.Size = new System.Drawing.Size(30, 15);
			this.lblLeft.TabIndex = 0;
			this.lblLeft.Text = "Left:";
			// 
			// lblRight
			// 
			this.lblRight.AutoSize = true;
			this.lblRight.Location = new System.Drawing.Point(10, 52);
			this.lblRight.Name = "lblRight";
			this.lblRight.Size = new System.Drawing.Size(38, 15);
			this.lblRight.TabIndex = 3;
			this.lblRight.Text = "Right:";
			// 
			// btnSwap
			// 
			this.btnSwap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSwap.Location = new System.Drawing.Point(172, 275);
			this.btnSwap.Name = "btnSwap";
			this.btnSwap.Size = new System.Drawing.Size(90, 29);
			this.btnSwap.TabIndex = 8;
			this.btnSwap.Text = "&Swap";
			this.btnSwap.Click += new System.EventHandler(this.Swap_Click);
			// 
			// chkOnlyIfShift
			// 
			this.chkOnlyIfShift.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkOnlyIfShift.AutoSize = true;
			this.chkOnlyIfShift.Location = new System.Drawing.Point(10, 243);
			this.chkOnlyIfShift.Name = "chkOnlyIfShift";
			this.chkOnlyIfShift.Size = new System.Drawing.Size(198, 19);
			this.chkOnlyIfShift.TabIndex = 7;
			this.chkOnlyIfShift.Text = "Only Show When Shift Is Pressed";
			// 
			// optionsControl
			// 
			this.optionsControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.optionsControl.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.optionsControl.Location = new System.Drawing.Point(5, 84);
			this.optionsControl.Name = "optionsControl";
			this.optionsControl.Size = new System.Drawing.Size(466, 152);
			this.optionsControl.TabIndex = 6;
			// 
			// FileDiffDlg
			// 
			this.AcceptButton = this.btnOK;
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(477, 316);
			this.Controls.Add(this.optionsControl);
			this.Controls.Add(this.chkOnlyIfShift);
			this.Controls.Add(this.btnSwap);
			this.Controls.Add(this.lblRight);
			this.Controls.Add(this.lblLeft);
			this.Controls.Add(this.edtLeft);
			this.Controls.Add(this.edtRight);
			this.Controls.Add(this.btnRight);
			this.Controls.Add(this.btnLeft);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FileDiffDlg";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Compare Files";
			this.Load += new System.EventHandler(this.FileDiffDlg_Load);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FileDiffDlg_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FileDiffDlg_DragEnter);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
	}
}

