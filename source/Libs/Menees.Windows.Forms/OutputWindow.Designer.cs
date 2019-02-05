namespace Menees.Windows.Forms
{
    public partial class OutputWindow
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
		private Menees.Windows.Forms.ExtendedRichTextBox output;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.output = new Menees.Windows.Forms.ExtendedRichTextBox();
			this.SuspendLayout();
			// 
			// output
			// 
			this.output.BackColor = System.Drawing.SystemColors.Window;
			this.output.Dock = System.Windows.Forms.DockStyle.Fill;
			this.output.HideSelection = false;
			this.output.Location = new System.Drawing.Point(0, 0);
			this.output.Name = "output";
			this.output.ReadOnly = true;
			this.output.RichTextShortcutsEnabled = false;
			this.output.Size = new System.Drawing.Size(294, 179);
			this.output.TabIndex = 0;
			this.output.WordWrap = false;
			this.output.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.RichTextBox_LinkClicked);
			this.output.DoubleClick += new System.EventHandler(this.RichTextBox_DoubleClick);
			// 
			// OutputWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.output);
			this.Name = "OutputWindow";
			this.Size = new System.Drawing.Size(294, 179);
			this.ResumeLayout(false);

        }

        #endregion
    }
}
