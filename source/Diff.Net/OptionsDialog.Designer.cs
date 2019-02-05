using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Menees;
using Menees.Diffs.Controls;
using Menees.Windows.Forms;
namespace Diff.Net
{
	partial class OptionsDialog
	{
		private System.Windows.Forms.Button btnCancel;
		private Button btnChanged;
		private Button btnDeleted;
		private Button btnFont;
		private Button btnInserted;
		private System.Windows.Forms.Button btnOK;
		private Button btnResetChanged;
		private Button btnResetDeleted;
		private Button btnResetInserted;
		private CheckBox chkGoToFirstDiff;
		private CheckBox chkShowChangeAsDeleteInsert;
		private CheckBox chkShowWSInLineDiff;
		private CheckBox chkShowWSInMainDiff;
		private System.Windows.Forms.ColorDialog ColorDlg;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private NumericUpDown edtSpacesPerTab;
		private System.Windows.Forms.FontDialog FontDlg;
		private GroupBox grpColors;
		private GroupBox grpFile;
		private Label lblChange;
		private Label lblDelete;
		private Label lblFont;
		private Label lblInsert;
		private Label lblSpacesPerTab;

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
			this.FontDlg = new System.Windows.Forms.FontDialog();
			this.ColorDlg = new System.Windows.Forms.ColorDialog();
			this.grpFile = new System.Windows.Forms.GroupBox();
			this.lblFont = new System.Windows.Forms.Label();
			this.chkShowChangeAsDeleteInsert = new System.Windows.Forms.CheckBox();
			this.chkShowWSInLineDiff = new System.Windows.Forms.CheckBox();
			this.edtSpacesPerTab = new System.Windows.Forms.NumericUpDown();
			this.btnFont = new System.Windows.Forms.Button();
			this.lblSpacesPerTab = new System.Windows.Forms.Label();
			this.chkGoToFirstDiff = new System.Windows.Forms.CheckBox();
			this.chkShowWSInMainDiff = new System.Windows.Forms.CheckBox();
			this.grpColors = new System.Windows.Forms.GroupBox();
			this.btnResetInserted = new System.Windows.Forms.Button();
			this.btnResetChanged = new System.Windows.Forms.Button();
			this.btnResetDeleted = new System.Windows.Forms.Button();
			this.btnChanged = new System.Windows.Forms.Button();
			this.btnDeleted = new System.Windows.Forms.Button();
			this.lblInsert = new System.Windows.Forms.Label();
			this.lblChange = new System.Windows.Forms.Label();
			this.lblDelete = new System.Windows.Forms.Label();
			this.btnInserted = new System.Windows.Forms.Button();
			this.showMdiTabs = new System.Windows.Forms.CheckBox();
			this.grpFile.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtSpacesPerTab)).BeginInit();
			this.grpColors.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(119, 406);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(90, 28);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.OK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(219, 406);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(90, 28);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			// 
			// FontDlg
			// 
			this.FontDlg.FontMustExist = true;
			this.FontDlg.ShowEffects = false;
			// 
			// ColorDlg
			// 
			this.ColorDlg.AnyColor = true;
			// 
			// grpFile
			// 
			this.grpFile.Controls.Add(this.showMdiTabs);
			this.grpFile.Controls.Add(this.lblFont);
			this.grpFile.Controls.Add(this.chkShowChangeAsDeleteInsert);
			this.grpFile.Controls.Add(this.chkShowWSInLineDiff);
			this.grpFile.Controls.Add(this.edtSpacesPerTab);
			this.grpFile.Controls.Add(this.btnFont);
			this.grpFile.Controls.Add(this.lblSpacesPerTab);
			this.grpFile.Controls.Add(this.chkGoToFirstDiff);
			this.grpFile.Controls.Add(this.chkShowWSInMainDiff);
			this.grpFile.Location = new System.Drawing.Point(12, 8);
			this.grpFile.Name = "grpFile";
			this.grpFile.Size = new System.Drawing.Size(297, 260);
			this.grpFile.TabIndex = 1;
			this.grpFile.TabStop = false;
			this.grpFile.Text = "Files";
			// 
			// lblFont
			// 
			this.lblFont.BackColor = System.Drawing.SystemColors.Window;
			this.lblFont.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblFont.Location = new System.Drawing.Point(20, 25);
			this.lblFont.Name = "lblFont";
			this.lblFont.Size = new System.Drawing.Size(144, 29);
			this.lblFont.TabIndex = 0;
			this.lblFont.Text = "C# Rules!";
			this.lblFont.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// chkShowChangeAsDeleteInsert
			// 
			this.chkShowChangeAsDeleteInsert.AutoSize = true;
			this.chkShowChangeAsDeleteInsert.Location = new System.Drawing.Point(20, 157);
			this.chkShowChangeAsDeleteInsert.Name = "chkShowChangeAsDeleteInsert";
			this.chkShowChangeAsDeleteInsert.Size = new System.Drawing.Size(223, 19);
			this.chkShowChangeAsDeleteInsert.TabIndex = 5;
			this.chkShowChangeAsDeleteInsert.Text = "Show Changes As Deletes And Inserts";
			// 
			// chkShowWSInLineDiff
			// 
			this.chkShowWSInLineDiff.AutoSize = true;
			this.chkShowWSInLineDiff.Location = new System.Drawing.Point(20, 98);
			this.chkShowWSInLineDiff.Name = "chkShowWSInLineDiff";
			this.chkShowWSInLineDiff.Size = new System.Drawing.Size(222, 19);
			this.chkShowWSInLineDiff.TabIndex = 3;
			this.chkShowWSInLineDiff.Text = "Show Whitespace In Current Line Diff";
			// 
			// edtSpacesPerTab
			// 
			this.edtSpacesPerTab.Location = new System.Drawing.Point(20, 220);
			this.edtSpacesPerTab.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.edtSpacesPerTab.Name = "edtSpacesPerTab";
			this.edtSpacesPerTab.Size = new System.Drawing.Size(53, 23);
			this.edtSpacesPerTab.TabIndex = 6;
			this.edtSpacesPerTab.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.edtSpacesPerTab.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
			// 
			// btnFont
			// 
			this.btnFont.Location = new System.Drawing.Point(176, 25);
			this.btnFont.Name = "btnFont";
			this.btnFont.Size = new System.Drawing.Size(76, 29);
			this.btnFont.TabIndex = 1;
			this.btnFont.Text = "Font...";
			this.btnFont.Click += new System.EventHandler(this.Font_Click);
			// 
			// lblSpacesPerTab
			// 
			this.lblSpacesPerTab.AutoSize = true;
			this.lblSpacesPerTab.Location = new System.Drawing.Point(77, 224);
			this.lblSpacesPerTab.Name = "lblSpacesPerTab";
			this.lblSpacesPerTab.Size = new System.Drawing.Size(86, 15);
			this.lblSpacesPerTab.TabIndex = 7;
			this.lblSpacesPerTab.Text = "Spaces Per Tab";
			// 
			// chkGoToFirstDiff
			// 
			this.chkGoToFirstDiff.AutoSize = true;
			this.chkGoToFirstDiff.Location = new System.Drawing.Point(20, 127);
			this.chkGoToFirstDiff.Name = "chkGoToFirstDiff";
			this.chkGoToFirstDiff.Size = new System.Drawing.Size(217, 19);
			this.chkGoToFirstDiff.TabIndex = 4;
			this.chkGoToFirstDiff.Text = "Automatically Go To First Difference";
			// 
			// chkShowWSInMainDiff
			// 
			this.chkShowWSInMainDiff.AutoSize = true;
			this.chkShowWSInMainDiff.Location = new System.Drawing.Point(20, 68);
			this.chkShowWSInMainDiff.Name = "chkShowWSInMainDiff";
			this.chkShowWSInMainDiff.Size = new System.Drawing.Size(229, 19);
			this.chkShowWSInMainDiff.TabIndex = 2;
			this.chkShowWSInMainDiff.Text = "Show Whitespace In Side-By-Side Diffs";
			// 
			// grpColors
			// 
			this.grpColors.Controls.Add(this.btnResetInserted);
			this.grpColors.Controls.Add(this.btnResetChanged);
			this.grpColors.Controls.Add(this.btnResetDeleted);
			this.grpColors.Controls.Add(this.btnChanged);
			this.grpColors.Controls.Add(this.btnDeleted);
			this.grpColors.Controls.Add(this.lblInsert);
			this.grpColors.Controls.Add(this.lblChange);
			this.grpColors.Controls.Add(this.lblDelete);
			this.grpColors.Controls.Add(this.btnInserted);
			this.grpColors.Location = new System.Drawing.Point(12, 276);
			this.grpColors.Name = "grpColors";
			this.grpColors.Size = new System.Drawing.Size(297, 118);
			this.grpColors.TabIndex = 0;
			this.grpColors.TabStop = false;
			this.grpColors.Text = "Colors";
			// 
			// btnResetInserted
			// 
			this.btnResetInserted.Location = new System.Drawing.Point(216, 84);
			this.btnResetInserted.Name = "btnResetInserted";
			this.btnResetInserted.Size = new System.Drawing.Size(62, 24);
			this.btnResetInserted.TabIndex = 8;
			this.btnResetInserted.Text = "Reset";
			this.btnResetInserted.Click += new System.EventHandler(this.ResetInserted_Click);
			// 
			// btnResetChanged
			// 
			this.btnResetChanged.Location = new System.Drawing.Point(216, 54);
			this.btnResetChanged.Name = "btnResetChanged";
			this.btnResetChanged.Size = new System.Drawing.Size(62, 25);
			this.btnResetChanged.TabIndex = 5;
			this.btnResetChanged.Text = "Reset";
			this.btnResetChanged.Click += new System.EventHandler(this.ResetChanged_Click);
			// 
			// btnResetDeleted
			// 
			this.btnResetDeleted.Location = new System.Drawing.Point(216, 25);
			this.btnResetDeleted.Name = "btnResetDeleted";
			this.btnResetDeleted.Size = new System.Drawing.Size(62, 24);
			this.btnResetDeleted.TabIndex = 2;
			this.btnResetDeleted.Text = "Reset";
			this.btnResetDeleted.Click += new System.EventHandler(this.ResetDeleted_Click);
			// 
			// btnChanged
			// 
			this.btnChanged.Location = new System.Drawing.Point(110, 54);
			this.btnChanged.Name = "btnChanged";
			this.btnChanged.Size = new System.Drawing.Size(90, 25);
			this.btnChanged.TabIndex = 4;
			this.btnChanged.Text = "Changed...";
			this.btnChanged.Click += new System.EventHandler(this.Changed_Click);
			// 
			// btnDeleted
			// 
			this.btnDeleted.Location = new System.Drawing.Point(110, 25);
			this.btnDeleted.Name = "btnDeleted";
			this.btnDeleted.Size = new System.Drawing.Size(90, 24);
			this.btnDeleted.TabIndex = 1;
			this.btnDeleted.Text = "Deleted...";
			this.btnDeleted.Click += new System.EventHandler(this.Deleted_Click);
			// 
			// lblInsert
			// 
			this.lblInsert.BackColor = System.Drawing.Color.PaleTurquoise;
			this.lblInsert.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblInsert.Location = new System.Drawing.Point(19, 84);
			this.lblInsert.Name = "lblInsert";
			this.lblInsert.Size = new System.Drawing.Size(77, 24);
			this.lblInsert.TabIndex = 6;
			this.lblInsert.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblChange
			// 
			this.lblChange.BackColor = System.Drawing.Color.PaleGreen;
			this.lblChange.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblChange.Location = new System.Drawing.Point(19, 54);
			this.lblChange.Name = "lblChange";
			this.lblChange.Size = new System.Drawing.Size(77, 25);
			this.lblChange.TabIndex = 3;
			this.lblChange.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// lblDelete
			// 
			this.lblDelete.BackColor = System.Drawing.Color.Pink;
			this.lblDelete.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblDelete.Location = new System.Drawing.Point(19, 25);
			this.lblDelete.Name = "lblDelete";
			this.lblDelete.Size = new System.Drawing.Size(77, 24);
			this.lblDelete.TabIndex = 0;
			this.lblDelete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnInserted
			// 
			this.btnInserted.Location = new System.Drawing.Point(110, 84);
			this.btnInserted.Name = "btnInserted";
			this.btnInserted.Size = new System.Drawing.Size(90, 24);
			this.btnInserted.TabIndex = 7;
			this.btnInserted.Text = "Inserted...";
			this.btnInserted.Click += new System.EventHandler(this.Inserted_Click);
			// 
			// showMdiTabs
			// 
			this.showMdiTabs.AutoSize = true;
			this.showMdiTabs.Location = new System.Drawing.Point(20, 188);
			this.showMdiTabs.Name = "showMdiTabs";
			this.showMdiTabs.Size = new System.Drawing.Size(177, 19);
			this.showMdiTabs.TabIndex = 8;
			this.showMdiTabs.Text = "Show MDI Window Tab Strip";
			// 
			// OptionsDlg
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(320, 446);
			this.Controls.Add(this.grpFile);
			this.Controls.Add(this.grpColors);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OptionsDlg";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Options";
			this.Load += new System.EventHandler(this.OptionsDlg_Load);
			this.grpFile.ResumeLayout(false);
			this.grpFile.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtSpacesPerTab)).EndInit();
			this.grpColors.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private CheckBox showMdiTabs;
	}
}

