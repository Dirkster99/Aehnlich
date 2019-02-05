namespace Menees.Diffs.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
    using DiffLib.Dir;
    using Menees.Windows.Forms;

	public sealed partial class DirectoryDiffControl : ExtendedUserControl
	{
		#region Private Data Members

		private DirectoryDiffTreeView activeTree;
		private bool showColorLegend = true;
		private bool showToolBar = true;

		#endregion

		#region Constructors

		public DirectoryDiffControl()
		{
			this.InitializeComponent();

			this.activeTree = this.TreeA;

			this.TreeA.ImageList = this.Images;
			this.TreeB.ImageList = this.Images;

			this.UpdateButtons();
			this.UpdateColors();

			DiffOptions.OptionsChanged += this.DiffOptionsChanged;
		}

		#endregion

		#region Public Events

		public event EventHandler RecompareNeeded;

		public event EventHandler<DifferenceEventArgs> ShowFileDifferences;

		#endregion

		#region Public Properties

		public bool CanRecompare => this.RecompareNeeded != null;

		[Browsable(false)]
		public bool CanShowDifferences
		{
			get
			{
				DirectoryDiffEntry entry = this.SelectedEntry;
				bool result = entry != null && entry.IsFile && entry.InA && entry.InB && this.ShowFileDifferences != null;
				return result;
			}
		}

		[Browsable(false)]
		public bool CanView => this.activeTree.CanView;

		[DefaultValue(true)]
		public bool FullRowSelect
		{
			get
			{
				return !this.TreeA.ShowLines;
			}

			set
			{
				// The TreeViews will always have FullRowSelect set to
				// true, but it doesn't affect anything unless ShowLines
				// is false.  So we'll just toggle the ShowLines property.
				if (this.FullRowSelect != value)
				{
					this.TreeA.ShowLines = !value;
					this.TreeB.ShowLines = !value;
				}
			}
		}

		[DefaultValue(true)]
		public bool ShowColorLegend
		{
			get
			{
				return this.showColorLegend;
			}

			set
			{
				if (this.showColorLegend != value)
				{
					this.showColorLegend = value;
					this.lblDelete.Visible = value;
					this.lblChange.Visible = value;
					this.lblInsert.Visible = value;
					this.tsSep2.Visible = value;
				}
			}
		}

		[DefaultValue(true)]
		public bool ShowToolBar
		{
			get
			{
				return this.showToolBar;
			}

			set
			{
				if (this.showToolBar != value)
				{
					// Note: We have to store the state ourselves because
					// Visible may return false even after we set it to true
					// if any of its parents are visible.
					this.showToolBar = value;
					this.ToolBar.Visible = value;
				}
			}
		}

		#endregion

		#region Private Properties

		private DirectoryDiffEntry SelectedEntry
		{
			get
			{
				if (this.TreeB.Focused)
				{
					return DirectoryDiffTreeView.GetEntryForNode(this.TreeB.SelectedNode);
				}
				else
				{
					return DirectoryDiffTreeView.GetEntryForNode(this.TreeA.SelectedNode);
				}
			}
		}

		#endregion

		#region Public Methods

		public bool Recompare()
		{
			bool result = false;

			if (this.CanRecompare)
			{
				this.RecompareNeeded(this, EventArgs.Empty);
				result = true;
			}

			return result;
		}

		public void SetData(DirectoryDiffResults results)
		{
			this.edtA.Text = results.DirectoryA.FullName;
			this.edtB.Text = results.DirectoryB.FullName;

			this.TreeA.SetData(results, true);
			this.TreeB.SetData(results, false);

			// Set a filter description
			if (results.Filter == null)
			{
				this.lblFilter.Text = "All Files";
			}
			else
			{
				DirectoryDiffFileFilter filter = results.Filter;
				this.lblFilter.Text = string.Format("{0}: {1}", filter.Include ? "Includes" : "Excludes", filter.FilterString);
			}

			this.UpdateButtons();

			if (this.TreeA.Nodes.Count > 0)
			{
				this.TreeA.SelectedNode = this.TreeA.Nodes[0];
			}
		}

		public void ShowDifferences()
		{
			if (this.CanShowDifferences)
			{
				DirectoryDiffEntry entry = this.SelectedEntry;

				TreeNode nodeA, nodeB;
				GetNodes(entry, out nodeA, out nodeB);
				string fileA = this.TreeA.GetFullNameForNode(nodeA);
				string fileB = this.TreeB.GetFullNameForNode(nodeB);

				this.ShowFileDifferences(this, new DifferenceEventArgs(fileA, fileB));
			}
		}

		public void View()
		{
			if (this.CanView)
			{
				this.activeTree.View();
			}
		}

		#endregion

		#region Private Methods

		private static void GetNodes(DirectoryDiffEntry entry, out TreeNode nodeA, out TreeNode nodeB)
		{
			nodeA = (TreeNode)entry.TagA;
			nodeB = (TreeNode)entry.TagB;
		}

		private static bool IsScrollingKey(KeyEventArgs e)
		{
			if (e.Modifiers == Keys.Control)
			{
				switch (e.KeyCode)
				{
					case Keys.Home:
					case Keys.End:
					case Keys.PageUp:
					case Keys.PageDown:
					case Keys.Up:
					case Keys.Down:
						return true;
				}
			}

			return false;
		}

		private void Recompare_Click(object sender, EventArgs e)
		{
			this.Recompare();
		}

		private void ShowDifferences_Click(object sender, EventArgs e)
		{
			this.ShowDifferences();
		}

		private void View_Click(object sender, EventArgs e)
		{
			this.View();
		}

		private void ColorLegend_Paint(object sender, PaintEventArgs e)
		{
			DiffControl.PaintColorLegendItem(sender as ToolStripItem, e);
		}

		private void DiffOptionsChanged(object sender, EventArgs e)
		{
			this.UpdateColors();
		}

		private void DirDiffControl_SizeChanged(object sender, EventArgs e)
		{
			this.pnlLeft.Width = (this.Width - this.Splitter.Width) / 2;
		}

		private void SyncTreeViewScrollPositions(DirectoryDiffTreeView source)
		{
			DirectoryDiffEntry entry = DirectoryDiffTreeView.GetEntryForNode(source.TopNode);
			if (entry != null)
			{
				TreeNode nodeA, nodeB;
				GetNodes(entry, out nodeA, out nodeB);
				if (nodeA != null && nodeB != null)
				{
					this.TreeA.TopNode = nodeA;
					this.TreeB.TopNode = nodeB;
				}
			}

			// An alternate but VERY flickery way to do this is:
			//  int sourcePos = Windows.GetScrollPos(source, false);
			//  target.BeginUpdate();
			//  Windows.SetScrollPos(target, false, sourcePos);
			//  target.EndUpdate();
		}

		private void TreeA_KeyDown(object sender, KeyEventArgs e)
		{
			// We only need to send scrolling key strokes (e.g. Ctrl+Home)
			// to the other tree.  If we send all keystrokes then weird
			// things happen.  For example, we already have the AfterSelect
			// events tied together, so when a user changes the selection
			// with a keystroke, we need AfterSelect to update the other
			// tree.  If we sent all keystrokes we'd end up with a double
			// move of the selection.
			if (IsScrollingKey(e))
			{
				this.SyncTreeViewScrollPositions(this.TreeA);
			}
		}

		private void TreeA_MouseWheelMsg(object sender, EventArgs e)
		{
			this.SyncTreeViewScrollPositions(this.TreeA);
		}

		private void TreeA_VScroll(object sender, EventArgs e)
		{
			this.SyncTreeViewScrollPositions(this.TreeA);
		}

		private void TreeB_KeyDown(object sender, KeyEventArgs e)
		{
			// See note in TreeA_KeyDown.
			if (IsScrollingKey(e))
			{
				this.SyncTreeViewScrollPositions(this.TreeB);
			}
		}

		private void TreeB_MouseWheelMsg(object sender, EventArgs e)
		{
			this.SyncTreeViewScrollPositions(this.TreeB);
		}

		private void TreeB_VScroll(object sender, EventArgs e)
		{
			this.SyncTreeViewScrollPositions(this.TreeB);
		}

		private void TreeNode_SelectChanged(object sender, TreeViewEventArgs e)
		{
			DirectoryDiffEntry entry = DirectoryDiffTreeView.GetEntryForNode(e.Node);
			if (entry != null)
			{
				TreeNode nodeA, nodeB;
				GetNodes(entry, out nodeA, out nodeB);
				if (nodeA != null && nodeB != null)
				{
					this.TreeA.SelectedNode = nodeA;
					this.TreeB.SelectedNode = nodeB;
				}
			}

			this.UpdateButtons();
		}

		private void TreeNode_StateChange(object sender, TreeViewEventArgs e)
		{
			DirectoryDiffEntry entry = DirectoryDiffTreeView.GetEntryForNode(e.Node);
			if (entry != null)
			{
				TreeNode nodeA, nodeB;
				GetNodes(entry, out nodeA, out nodeB);
				if (nodeA != null && nodeB != null)
				{
					if (e.Action == TreeViewAction.Collapse)
					{
						// Although the Docs say that Collapse() isn't recursive,
						// it actually is.  Since clicking the +/- isn't recursive
						// we have to simulate that by calling Collapse even on the
						// node that fired the event.
						if (nodeA.IsExpanded || e.Node == nodeA)
						{
							nodeA.Collapse();
						}

						if (nodeB.IsExpanded || e.Node == nodeB)
						{
							nodeB.Collapse();
						}
					}
					else if (e.Action == TreeViewAction.Expand)
					{
						if (!nodeA.IsExpanded || e.Node == nodeA)
						{
							nodeA.Expand();
						}

						if (!nodeB.IsExpanded || e.Node == nodeB)
						{
							nodeB.Expand();
						}
					}
				}
			}
		}

		private void TreeView_DoubleClick(object sender, EventArgs e)
		{
			this.ShowDifferences();
		}

		private void TreeView_Enter(object sender, EventArgs e)
		{
			this.activeTree = (DirectoryDiffTreeView)sender;
			this.UpdateButtons();
		}

		private void UpdateButtons()
		{
			this.btnView.Enabled = this.CanView;
			this.mnuView.Enabled = this.btnView.Enabled;
			this.btnShowDifferences.Enabled = this.CanShowDifferences;
			this.mnuShowDifferences.Enabled = this.btnShowDifferences.Enabled;
			this.btnRecompare.Enabled = this.CanRecompare;
		}

		private void UpdateColors()
		{
			this.lblDelete.BackColor = DiffOptions.DeletedColor;
			this.lblChange.BackColor = DiffOptions.ChangedColor;
			this.lblInsert.BackColor = DiffOptions.InsertedColor;
		}

		#endregion
	}
}
