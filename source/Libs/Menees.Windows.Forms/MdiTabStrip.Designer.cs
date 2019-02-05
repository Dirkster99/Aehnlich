namespace Menees.Windows.Forms
{
	#region Using Directives

	using System.ComponentModel;
	using System.Windows.Forms;

	#endregion

	public partial class MdiTabStrip
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components;
		private Timer dragTimer;

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
			this.dragTimer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// dragTimer
			// 
			this.dragTimer.Interval = 1000;
			this.dragTimer.Tick += new System.EventHandler(this.DragTimer_Tick);
			// 
			// MdiTabStrip
			// 
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MdiTabStrip_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MdiTabStrip_DragEnter);
			this.DragOver += new System.Windows.Forms.DragEventHandler(this.MdiTabStrip_DragOver);
			this.ResumeLayout(false);

		}

		#endregion
	}
}
