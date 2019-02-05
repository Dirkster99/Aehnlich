namespace Menees.Windows.Forms
{
	internal partial class InputDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.TextBox value;
		private System.Windows.Forms.Label prompt;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okayButton;
		private System.Windows.Forms.ErrorProvider errorProvider;

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
			this.components = new System.ComponentModel.Container();
			this.value = new System.Windows.Forms.TextBox();
			this.prompt = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okayButton = new System.Windows.Forms.Button();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// value
			// 
			this.value.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.value.Location = new System.Drawing.Point(16, 85);
			this.value.Name = "value";
			this.value.Size = new System.Drawing.Size(424, 23);
			this.value.TabIndex = 1;
			// 
			// prompt
			// 
			this.prompt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.prompt.Location = new System.Drawing.Point(14, 14);
			this.prompt.Name = "prompt";
			this.prompt.Size = new System.Drawing.Size(428, 61);
			this.prompt.TabIndex = 0;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(353, 121);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(87, 27);
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "&Cancel";
			// 
			// okayButton
			// 
			this.okayButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okayButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okayButton.Location = new System.Drawing.Point(255, 121);
			this.okayButton.Name = "okayButton";
			this.okayButton.Size = new System.Drawing.Size(87, 27);
			this.okayButton.TabIndex = 2;
			this.okayButton.Text = "&OK";
			this.okayButton.Click += new System.EventHandler(this.OkayButton_Click);
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// InputDlg
			// 
			this.AcceptButton = this.okayButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(458, 159);
			this.Controls.Add(this.value);
			this.Controls.Add(this.prompt);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okayButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InputDlg";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
	}
}