using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Menees.Windows.Forms;
namespace Menees.Diffs.Controls
{
	partial class GoToDialog
	{
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.NumericUpDown edtLineNumber;
		private System.Windows.Forms.Label lblLineNumber;

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
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblLineNumber = new System.Windows.Forms.Label();
			this.edtLineNumber = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.edtLineNumber)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(63, 71);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(90, 28);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(164, 71);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(90, 28);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			// 
			// lblLineNumber
			// 
			this.lblLineNumber.AutoSize = true;
			this.lblLineNumber.Location = new System.Drawing.Point(10, 10);
			this.lblLineNumber.Name = "lblLineNumber";
			this.lblLineNumber.Size = new System.Drawing.Size(110, 15);
			this.lblLineNumber.TabIndex = 0;
			this.lblLineNumber.Text = "&Line Number (1-N):";
			// 
			// edtLineNumber
			// 
			this.edtLineNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.edtLineNumber.Location = new System.Drawing.Point(10, 34);
			this.edtLineNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.edtLineNumber.Name = "edtLineNumber";
			this.edtLineNumber.Size = new System.Drawing.Size(245, 23);
			this.edtLineNumber.TabIndex = 1;
			this.edtLineNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// GoToDlg
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(266, 111);
			this.Controls.Add(this.edtLineNumber);
			this.Controls.Add(this.lblLineNumber);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GoToDlg";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Go To Line";
			((System.ComponentModel.ISupportInitialize)(this.edtLineNumber)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
	}
}

