namespace Menees.Windows.Forms
{
	public partial class FindDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okayButton;
		private System.Windows.Forms.Label findLabel;
		private System.Windows.Forms.CheckBox searchUp;
		private System.Windows.Forms.CheckBox matchCase;
		private System.Windows.Forms.TextBox findText;

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
			this.searchUp = new System.Windows.Forms.CheckBox();
			this.matchCase = new System.Windows.Forms.CheckBox();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okayButton = new System.Windows.Forms.Button();
			this.findText = new System.Windows.Forms.TextBox();
			this.findLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// searchUp
			// 
			this.searchUp.AutoSize = true;
			this.searchUp.Location = new System.Drawing.Point(52, 80);
			this.searchUp.Name = "searchUp";
			this.searchUp.Size = new System.Drawing.Size(79, 19);
			this.searchUp.TabIndex = 3;
			this.searchUp.Text = "Search &Up";
			// 
			// matchCase
			// 
			this.matchCase.AutoSize = true;
			this.matchCase.Location = new System.Drawing.Point(52, 52);
			this.matchCase.Name = "matchCase";
			this.matchCase.Size = new System.Drawing.Size(88, 19);
			this.matchCase.TabIndex = 2;
			this.matchCase.Text = "Match &Case";
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(240, 88);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(87, 27);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "Cancel";
			// 
			// okayButton
			// 
			this.okayButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okayButton.Enabled = false;
			this.okayButton.Location = new System.Drawing.Point(240, 52);
			this.okayButton.Name = "okayButton";
			this.okayButton.Size = new System.Drawing.Size(87, 27);
			this.okayButton.TabIndex = 4;
			this.okayButton.Text = "OK";
			// 
			// findText
			// 
			this.findText.Location = new System.Drawing.Point(52, 12);
			this.findText.Name = "findText";
			this.findText.Size = new System.Drawing.Size(275, 23);
			this.findText.TabIndex = 1;
			this.findText.TextChanged += new System.EventHandler(this.FindText_TextChanged);
			// 
			// findLabel
			// 
			this.findLabel.AutoSize = true;
			this.findLabel.Location = new System.Drawing.Point(12, 16);
			this.findLabel.Name = "findLabel";
			this.findLabel.Size = new System.Drawing.Size(33, 15);
			this.findLabel.TabIndex = 0;
			this.findLabel.Text = "&Find:";
			// 
			// FindDialog
			// 
			this.AcceptButton = this.okayButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(341, 128);
			this.Controls.Add(this.searchUp);
			this.Controls.Add(this.matchCase);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okayButton);
			this.Controls.Add(this.findText);
			this.Controls.Add(this.findLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FindDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Find";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
	}
}