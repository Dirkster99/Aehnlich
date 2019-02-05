namespace Menees.Windows.Forms
{
	internal partial class AboutBox
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.LinkLabel emailLink;
		private System.Windows.Forms.GroupBox bottomSeparator;
		private System.Windows.Forms.LinkLabel webLink;
		private System.Windows.Forms.Label copyright;
		private System.Windows.Forms.Label version;
		private System.Windows.Forms.Label productName;
		private System.Windows.Forms.PictureBox logo;
		private System.Windows.Forms.PictureBox icon;
		private System.Windows.Forms.Button okayButton;
		private System.Windows.Forms.Label charityWare;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (this.components != null))
			{
				this.components.Dispose();
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
			this.emailLink = new System.Windows.Forms.LinkLabel();
			this.bottomSeparator = new System.Windows.Forms.GroupBox();
			this.webLink = new System.Windows.Forms.LinkLabel();
			this.copyright = new System.Windows.Forms.Label();
			this.version = new System.Windows.Forms.Label();
			this.productName = new System.Windows.Forms.Label();
			this.logo = new System.Windows.Forms.PictureBox();
			this.icon = new System.Windows.Forms.PictureBox();
			this.okayButton = new System.Windows.Forms.Button();
			this.charityWare = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.logo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.icon)).BeginInit();
			this.SuspendLayout();
			// 
			// emailLink
			// 
			this.emailLink.AutoSize = true;
			this.emailLink.Location = new System.Drawing.Point(232, 188);
			this.emailLink.Name = "emailLink";
			this.emailLink.Size = new System.Drawing.Size(102, 15);
			this.emailLink.TabIndex = 5;
			this.emailLink.TabStop = true;
			this.emailLink.Text = "bill@menees.com";
			this.emailLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.EmailLink_LinkClicked);
			// 
			// bottomSeparator
			// 
			this.bottomSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.bottomSeparator.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.bottomSeparator.Location = new System.Drawing.Point(-9, 262);
			this.bottomSeparator.Name = "bottomSeparator";
			this.bottomSeparator.Size = new System.Drawing.Size(485, 5);
			this.bottomSeparator.TabIndex = 7;
			this.bottomSeparator.TabStop = false;
			// 
			// webLink
			// 
			this.webLink.AutoSize = true;
			this.webLink.Location = new System.Drawing.Point(106, 188);
			this.webLink.Name = "webLink";
			this.webLink.Size = new System.Drawing.Size(105, 15);
			this.webLink.TabIndex = 4;
			this.webLink.TabStop = true;
			this.webLink.Text = "www.menees.com";
			this.webLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.WebLink_LinkClicked);
			// 
			// copyright
			// 
			this.copyright.AutoSize = true;
			this.copyright.Location = new System.Drawing.Point(106, 164);
			this.copyright.Name = "copyright";
			this.copyright.Size = new System.Drawing.Size(193, 15);
			this.copyright.TabIndex = 3;
			this.copyright.Text = "Copyright © 2002-2013 Bill Menees";
			// 
			// version
			// 
			this.version.AutoSize = true;
			this.version.Location = new System.Drawing.Point(106, 140);
			this.version.Name = "version";
			this.version.Size = new System.Drawing.Size(64, 15);
			this.version.TabIndex = 2;
			this.version.Text = "Version 1.0";
			// 
			// productName
			// 
			this.productName.AutoSize = true;
			this.productName.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.productName.Location = new System.Drawing.Point(104, 107);
			this.productName.Name = "productName";
			this.productName.Size = new System.Drawing.Size(136, 25);
			this.productName.TabIndex = 1;
			this.productName.Text = "Product Name";
			// 
			// logo
			// 
			this.logo.Dock = System.Windows.Forms.DockStyle.Top;
			this.logo.Image = global::Menees.Windows.Forms.Properties.Resources.AboutBoxBanner;
			this.logo.Location = new System.Drawing.Point(0, 0);
			this.logo.Name = "logo";
			this.logo.Size = new System.Drawing.Size(474, 92);
			this.logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.logo.TabIndex = 12;
			this.logo.TabStop = false;
			// 
			// icon
			// 
			this.icon.Location = new System.Drawing.Point(40, 112);
			this.icon.Name = "icon";
			this.icon.Size = new System.Drawing.Size(37, 37);
			this.icon.TabIndex = 11;
			this.icon.TabStop = false;
			// 
			// okayButton
			// 
			this.okayButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okayButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okayButton.Location = new System.Drawing.Point(373, 280);
			this.okayButton.Name = "okayButton";
			this.okayButton.Size = new System.Drawing.Size(87, 27);
			this.okayButton.TabIndex = 0;
			this.okayButton.Text = "OK";
			// 
			// charityWare
			// 
			this.charityWare.Location = new System.Drawing.Point(106, 212);
			this.charityWare.Name = "charityWare";
			this.charityWare.Size = new System.Drawing.Size(331, 42);
			this.charityWare.TabIndex = 6;
			this.charityWare.Text = "This software is CharityWare.  If you use it, please donate at least $5 (US) to t" +
    "he charity of your choice.";
			// 
			// AboutBox
			// 
			this.AcceptButton = this.okayButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.okayButton;
			this.ClientSize = new System.Drawing.Size(474, 321);
			this.Controls.Add(this.charityWare);
			this.Controls.Add(this.emailLink);
			this.Controls.Add(this.bottomSeparator);
			this.Controls.Add(this.webLink);
			this.Controls.Add(this.copyright);
			this.Controls.Add(this.version);
			this.Controls.Add(this.productName);
			this.Controls.Add(this.logo);
			this.Controls.Add(this.icon);
			this.Controls.Add(this.okayButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutBox";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About";
			((System.ComponentModel.ISupportInitialize)(this.logo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.icon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
	}
}