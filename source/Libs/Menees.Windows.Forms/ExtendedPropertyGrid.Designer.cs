namespace Menees.Windows.Forms
{
    public partial class ExtendedPropertyGrid
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.ContextMenuStrip gridContext;
		private System.Windows.Forms.ToolStripMenuItem reset;

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
			this.components = new System.ComponentModel.Container();
			this.gridContext = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.reset = new System.Windows.Forms.ToolStripMenuItem();
			this.gridContext.SuspendLayout();
			this.SuspendLayout();
			// 
			// gridContext
			// 
			this.gridContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reset});
			this.gridContext.Name = "m_gridContext";
			this.gridContext.Size = new System.Drawing.Size(103, 26);
			this.gridContext.Opening += new System.ComponentModel.CancelEventHandler(this.GridContext_Opening);
			// 
			// reset
			// 
			this.reset.Name = "reset";
			this.reset.Size = new System.Drawing.Size(102, 22);
			this.reset.Text = "&Reset";
			this.reset.Click += new System.EventHandler(this.Reset_Click);
			// 
			// ExtendedPropertyGrid
			// 
			this.ContextMenuStrip = this.gridContext;
			this.gridContext.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion
    }
}
