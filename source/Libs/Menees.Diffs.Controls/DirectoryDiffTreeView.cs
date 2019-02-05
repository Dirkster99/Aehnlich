namespace Menees.Diffs.Controls
{
	using System;
	using System.Windows.Forms;
    using DiffLib.Dir;
	using Menees.Windows.Forms;

	internal sealed class DirectoryDiffTreeView : TreeView
	{
		#region Private Data Members

		private const int FileIndex = 3;
		private const int FileErrorIndex = 5;
		private const int FileMissingIndex = 4;
		private const int FolderClosedIndex = 0;
		private const int FolderMissingIndex = 2;
		private const int FolderOpenIndex = 1;

		private bool useA;
		private DirectoryDiffResults results;

		#endregion

		#region Constructors

		public DirectoryDiffTreeView()
		{
			this.HideSelection = false;
			this.ShowLines = false;
			this.FullRowSelect = true;

			DiffOptions.OptionsChanged += this.DiffOptionsChanged;
		}

		#endregion

		#region Internal Events

		internal event EventHandler MouseWheelMsg;

		internal event EventHandler VScroll;

		#endregion

		#region Public Properties

		public bool CanView
		{
			get
			{
				bool result = false;

				TreeNode node = this.SelectedNode;
				if (node != null)
				{
					DirectoryDiffEntry entry = GetEntryForNode(node);
					if (entry != null)
					{
						result = (this.useA && entry.InA) || (!this.useA && entry.InB);
					}
				}

				return result;
			}
		}

		#endregion

		#region Public Methods

		public static DirectoryDiffEntry GetEntryForNode(TreeNode node)
		{
			if (node != null)
			{
				return node.Tag as DirectoryDiffEntry;
			}
			else
			{
				return null;
			}
		}

		public string GetFullNameForNode(TreeNode node)
		{
			string nodePath = node.FullPath;
			string basePath = this.useA ? this.results.DirectoryA.FullName : this.results.DirectoryB.FullName;
			if (!basePath.EndsWith("\\"))
			{
				basePath += "\\";
			}

			return basePath + nodePath;
		}

		public void SetData(DirectoryDiffResults results, bool useA)
		{
			this.results = results;
			this.useA = useA;
			this.PopulateTree();
		}

		public void View()
		{
			if (this.CanView)
			{
				TreeNode node = this.SelectedNode;
				string fullName = this.GetFullNameForNode(node);
				WindowsUtility.ShellExecute(this, fullName);
			}
		}

		#endregion

		#region Protected Methods

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				DiffOptions.OptionsChanged -= this.DiffOptionsChanged;
			}

			base.Dispose(disposing);
		}

		protected override void OnAfterCollapse(TreeViewEventArgs e)
		{
			this.SetNodeImage(e.Node, GetEntryForNode(e.Node));
			base.OnAfterCollapse(e);
		}

		protected override void OnAfterExpand(TreeViewEventArgs e)
		{
			this.SetNodeImage(e.Node, GetEntryForNode(e.Node));
			base.OnAfterExpand(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			TreeNode node = this.GetNodeAt(e.X, e.Y);
			if (node != null)
			{
				this.SelectedNode = node;
			}
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if (m.Msg == NativeMethods.WM_VSCROLL && this.VScroll != null)
			{
				this.VScroll(this, EventArgs.Empty);
			}
			else if (m.Msg == NativeMethods.WM_MOUSEWHEEL && this.MouseWheelMsg != null)
			{
				this.MouseWheelMsg(this, EventArgs.Empty);
			}
		}

		#endregion

		#region Private Methods

		private void AddEntry(DirectoryDiffEntry entry, TreeNode parentNode)
		{
			TreeNode node = new TreeNode();
			node.Tag = entry;

			if (this.useA)
			{
				entry.TagA = node;
			}
			else
			{
				entry.TagB = node;
			}

			node.Expand();

			this.SetNodeText(node, entry);
			this.SetNodeImage(node, entry);
			this.SetNodeColor(node, entry);

			if (parentNode != null)
			{
				parentNode.Nodes.Add(node);
			}
			else
			{
				this.Nodes.Add(node);
			}

			if (!entry.IsFile)
			{
				foreach (DirectoryDiffEntry subEntry in entry.Subentries)
				{
					this.AddEntry(subEntry, node);
				}
			}
		}

		private void DiffOptionsChanged(object sender, EventArgs e)
		{
			this.SetNodesColors(this.Nodes);
		}

		private void PopulateTree()
		{
			this.BeginUpdate();
			try
			{
				this.Nodes.Clear();

				foreach (DirectoryDiffEntry entry in this.results.Entries)
				{
					this.AddEntry(entry, null);
				}

				this.ExpandAll();
			}
			finally
			{
				this.EndUpdate();
			}
		}

		private void SetNodeColor(TreeNode node, DirectoryDiffEntry entry)
		{
			if (entry.InA && entry.InB)
			{
				if (entry.Different)
				{
					node.BackColor = DiffOptions.ChangedColor;
				}
				else
				{
					node.BackColor = this.BackColor;
				}
			}
			else
			{
				if (entry.InA)
				{
					node.BackColor = DiffOptions.DeletedColor;
				}
				else
				{
					node.BackColor = DiffOptions.InsertedColor;
				}
			}
		}

		private void SetNodeImage(TreeNode node, DirectoryDiffEntry entry)
		{
			bool present = (this.useA && entry.InA) || (!this.useA && entry.InB);
			int index = -1;

			if (entry.Error != null)
			{
				index = FileErrorIndex;
			}
			else if (entry.IsFile)
			{
				if (present)
				{
					index = FileIndex;
				}
				else
				{
					index = FileMissingIndex;
				}
			}
			else if (present)
			{
				// If a folder is only present on one side, then
				// we should always show it closed since we haven't
				// actually recursed into it.
				//
				// Also, we should only show a folder open if we're
				// showing recursive differences.
				if (this.results.Recursive && node.IsExpanded && entry.InA && entry.InB)
				{
					index = FolderOpenIndex;
				}
				else
				{
					index = FolderClosedIndex;
				}
			}
			else
			{
				index = FolderMissingIndex;
			}

			node.ImageIndex = index;
			node.SelectedImageIndex = index;
		}

		private void SetNodesColors(TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{
				this.SetNodeColor(node, GetEntryForNode(node));
				this.SetNodesColors(node.Nodes);
			}
		}

		private void SetNodeText(TreeNode node, DirectoryDiffEntry entry)
		{
			if ((entry.InA && entry.InB) || (this.useA && entry.InA) || (!this.useA && entry.InB))
			{
				if (entry.Error == null)
				{
					node.Text = entry.Name;
				}
				else
				{
					node.Text = string.Format("{0}: {1}", entry.Name, entry.Error);
				}
			}
		}

		#endregion
	}
}
