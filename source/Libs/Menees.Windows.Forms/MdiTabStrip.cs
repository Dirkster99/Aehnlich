namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	#endregion

	/// <summary>
	/// Displays an <see cref="MdiTab"/> for each MDI child form.
	/// </summary>
	public sealed partial class MdiTabStrip : ExtendedToolStrip
	{
		#region Private Data Members

		private static readonly IList<Form> NoForms = new Form[0];

		private int updateLevel;
		private MdiTab dragTab;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public MdiTabStrip()
		{
			this.InitializeComponent();

			this.AllowDrop = true;
			this.GripStyle = ToolStripGripStyle.Hidden;
			this.UseFormIconAsNewTabImage = true;

			MdiTabStripColorTable colorTable = new MdiTabStripColorTable(this);
			ToolStripProfessionalRenderer renderer = new ToolStripProfessionalRenderer(colorTable);
			this.Renderer = renderer;

			// Call this to make it initially invisible (if it's not design time).
			this.UpdateTabStrip(null);
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets a value indicating whether drag-and-drop item reordering is allowed.
		/// </summary>
		/// <remarks>
		/// This defaults to true so tab items can be reordered by the user via drag-and-drop.
		/// It is hidden at a design-time because it shouldn't be changed for MdiTabStrips.
		/// </remarks>
		[DefaultValue(true)]
		[Browsable(false)]
		public override bool AllowDrop
		{
			get
			{
				return base.AllowDrop;
			}

			set
			{
				if (value)
				{
					this.AllowItemReorder = false;
				}

				base.AllowDrop = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether tabs can be reordered when ALT is pressed.
		/// </summary>
		/// <remarks>
		/// This defaults to false because its drag-and-drop is incompatible with our standard
		/// drag-and-drop (because AllowItemReorder will let you drag items to other ToolStrips).
		/// It is hidden at a design-time because it shouldn't be changed for MdiTabStrips.
		/// </remarks>
		[DefaultValue(false)]
		[Browsable(false)]
		public new bool AllowItemReorder
		{
			get { return base.AllowItemReorder; }
			set { base.AllowItemReorder = value; }
		}

		/// <summary>
		/// Gets or sets whether the move handle is visible or hidden.
		/// </summary>
		/// <remarks>
		/// This defaults to Hidden, which is opposite of how it defaults in the base ToolStrip.
		/// </remarks>
		[DefaultValue(ToolStripGripStyle.Hidden)]
		public new ToolStripGripStyle GripStyle
		{
			get { return base.GripStyle; }
			set { base.GripStyle = value; }
		}

		/// <summary>
		/// Gets or sets how the tab strip is rendered.
		/// </summary>
		/// <remarks>
		/// This class always uses a custom renderer, so the RenderMode property
		/// should not be changed.
		/// </remarks>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new ToolStripRenderMode RenderMode
		{
			get { return base.RenderMode; }
			set { base.RenderMode = value; }
		}

		/// <summary>
		/// Gets or sets whether the image for a new tab should be extracted from the form's icon.
		/// </summary>
		/// <remarks>
		/// This defaults to true.  Set it to false if you want to manually set each tab's image.
		/// </remarks>
		[DefaultValue(true)]
		[Description("Whether the image for a new tab should be extracted from the form's icon.")]
		public bool UseFormIconAsNewTabImage
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the current collection of tabs.
		/// </summary>
		[Browsable(false)]
		public IEnumerable<MdiTab> Tabs
		{
			get
			{
				IEnumerable<MdiTab> result = this.GetTabs();
				return result;
			}
		}

		#endregion

		#region Private Properties

		private Form ActiveMdiChild
		{
			get
			{
				Form result = null;

				Form ownerForm = this.FindForm();
				if (ownerForm != null)
				{
					result = ownerForm.ActiveMdiChild;
				}

				return result;
			}
		}

		private IList<Form> MdiChildren
		{
			get
			{
				IList<Form> result;

				Form ownerForm = this.FindForm();
				if (ownerForm != null)
				{
					result = ownerForm.MdiChildren;
				}
				else
				{
					result = NoForms;
				}

				return result;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Prevents the tab strip from updating until <see cref="EndUpdate"/> is called.
		/// </summary>
		/// <remarks>
		/// This is useful when adding or removing many forms at once.  You should use
		/// a try..finally block to make sure EndUpdate gets called.
		/// </remarks>
		public void BeginUpdate()
		{
			this.updateLevel++;
		}

		/// <summary>
		/// Resumes updating the tab strip after being suspended by the <see cref="BeginUpdate"/> method.
		/// </summary>
		public void EndUpdate()
		{
			this.updateLevel--;
			if (this.updateLevel == 0)
			{
				// Pass false for "senderActivating", so UpdateTabStrip
				// will determine the correct new active tab.
				this.UpdateTabStrip(null, false);
			}
		}

		/// <summary>
		/// Closes the tab (and form) for the active MDI child form.
		/// </summary>
		public void CloseActiveTab()
		{
			Form child = this.ActiveMdiChild;
			if (child != null)
			{
				child.Close();
			}
		}

		/// <summary>
		/// Closes every tab (and form) except the one for the active MDI child form.
		/// </summary>
		public void CloseAllButActiveTab()
		{
			Form child = this.ActiveMdiChild;
			if (child != null)
			{
				this.BeginUpdate();
				try
				{
					foreach (MdiTab tab in this.GetTabs())
					{
						Form form = tab.AssociatedForm;
						if (form != null && form != child)
						{
							form.Close();
						}
					}
				}
				finally
				{
					this.EndUpdate();
				}
			}
		}

		/// <summary>
		/// Finds the tab for the active MDI child form.
		/// </summary>
		/// <returns>The active tab, or null if there is no active child form.</returns>
		public MdiTab FindActiveTab()
		{
			Form child = this.ActiveMdiChild;
			MdiTab result = this.FindTab(child);
			return result;
		}

		/// <summary>
		/// Finds the tab for the specified MDI child form.
		/// </summary>
		/// <param name="form">The form to get the tab for.</param>
		/// <returns>The tab for the specified form, or null if no tab exists for the form
		/// (e.g., if the form is closed).</returns>
		public MdiTab FindTab(Form form)
		{
			MdiTab result = this.FindTab(form, null);
			return result;
		}

		/// <summary>
		/// Gets the tab at the specified screen point (e.g., <see cref="Cursor.Position"/>).
		/// </summary>
		/// <param name="point">The screen point to look under.</param>
		/// <returns>The tab under the specified point or null.</returns>
		/// <remarks>
		/// This method is useful for preventing the opening of a context menu strip
		/// unless the mouse cursor is currently over a tab.
		/// </remarks>
		public MdiTab FindTabAtScreenPoint(Point point)
		{
			Point clientPoint = this.PointToClient(point);
			MdiTab result = this.FindDisplayedTabAtClientPoint(clientPoint, true);
			return result;
		}

		#endregion

		#region Internal Methods

		internal void BeginDragTimer(MdiTab tab)
		{
			if (!this.dragTimer.Enabled)
			{
				this.dragTab = tab;
				this.Capture = true;

				// This is slow enough that most normal, quick left clicks won't begin a drag
				// but fast enough that pressing and holding to begin a drag doesn't make
				// you wait an awkward amount of time.
				const int BeginDragMilliseconds = 200;

				// The drag timer ensures that normal, quick left clicks won't begin a drag,
				// which would change the cursor to a Move cursor.  The default seems like a
				// good response time to me, but if the mouse hover time is quicker, then we'll
				// use that.  Setting this on each timer usage allows us to react to system
				// setting changes.  It also prevents us from getting a CA1601 code analysis
				// warning about mobility power management concerns with fast timers:
				// http://msdn.microsoft.com/en-us/library/ms182230.aspx
				this.dragTimer.Interval = Math.Min(BeginDragMilliseconds, SystemInformation.MouseHoverTime);
				this.dragTimer.Enabled = true;
			}
		}

		internal void EndDragTimer(bool allowDrag)
		{
			this.dragTimer.Enabled = false;

			bool hadCapture = this.Capture;
			this.Capture = false;

			if (allowDrag)
			{
				// Make sure the click is still held on the drag tab and there is more than one tab visible.
				if (hadCapture && Control.MouseButtons == MouseButtons.Left && this.dragTab != null &&
					this.dragTab == this.FindDisplayedTabAtClientPoint(this.PointToClient(Control.MousePosition), true) &&
					this.GetDisplayedTabs().Count > 1)
				{
					this.dragTab.DoDragDrop(this.dragTab, DragDropEffects.Move);
				}
			}

			this.dragTab = null;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Overrides <see cref="Control.OnParentChanged"/>.
		/// </summary>
		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);

			Form form = this.FindForm();
			if (form != null)
			{
				form.MdiChildActivate += this.Form_MdiChildActivate;
			}
		}

		/// <summary>
		/// Overrides <see cref="Control.OnEnabledChanged"/>.
		/// </summary>
		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);

			// Use an update batch, so the control will fully refresh when re-enabled.
			this.BeginUpdate();
			this.EndUpdate();
		}

		#endregion

		#region Private Helper Methods

		private static bool IsAvailable(Form form)
		{
			bool result = form != null && !form.Disposing && !form.IsDisposed && form.Visible && form.IsMdiChild;
			return result;
		}

		private MdiTab FindTab(Form form, IList<MdiTab> tabs)
		{
			MdiTab result = null;

			if (form != null)
			{
				if (tabs == null)
				{
					tabs = this.GetTabs();
				}

				result = tabs.FirstOrDefault(tab => tab.AssociatedForm == form);
			}

			return result;
		}

		private IList<MdiTab> GetTabs()
		{
			// Call ToList() to create a snapshot because callers may need to modify the this.Items collection.
			// Restrict to MdiTabs in case a caller has added other controls to the tool strip (e.g., a right-aligned,
			// never overflowing Close button).
			var result = this.Items.OfType<MdiTab>().ToList();
			return result;
		}

		private IList<MdiTab> GetDisplayedTabs()
		{
			// See comments in GetTabs about why ToList is used.
			var result = this.DisplayedItems.OfType<MdiTab>().ToList();
			return result;
		}

		private void UpdateTabStrip(MdiTab sender, bool senderActivating = true)
		{
			// Don't do anything if we're in the middle of a Begin/EndUpdate batch.
			if (this.updateLevel == 0 && this.Enabled)
			{
				// Remove tabs for closed forms.
				HashSet<Form> skipForms = new HashSet<Form>();
				IList<MdiTab> tabs = this.RemoveClosedTabs(sender, senderActivating, skipForms);

				// Add tabs for new forms.
				tabs = this.AddNewTabs(tabs, skipForms);

				// If the old sender was deactivating, then we need to activate the tab for the new active window.
				// Also, if a user right-clicks directly on a tab's close button, it will close the tab's form and then
				// we'll get the tab click event.  So we need to detect when a sender thinks it's activating even
				// though its form has already closed.  (Update: Now I've made right-click not fire Close though.)
				MdiTab activeTab = sender;
				if (!senderActivating || activeTab == null || activeTab.AssociatedForm == null)
				{
					activeTab = this.FindTab(this.ActiveMdiChild, tabs);
				}

				// Update the active tab.
				foreach (MdiTab tab in tabs)
				{
					Form form = tab.AssociatedForm;
					if (tab == activeTab)
					{
						tab.Checked = true;
						if (IsAvailable(form))
						{
							form.Select();
						}
					}
					else
					{
						tab.Checked = false;
					}

					tab.Text = form.Text;
				}

				// Make sure the active tab is visible.
				if (activeTab != null && activeTab.IsOnOverflow)
				{
					this.Items.Insert(0, activeTab);
				}

				// Show or hide the tab strip as necessary.
				if (!this.DesignMode)
				{
					this.Visible = tabs.Count > 0;
				}
			}
		}

		private IList<MdiTab> RemoveClosedTabs(MdiTab sender, bool senderActivating, HashSet<Form> skipForms)
		{
			IList<MdiTab> result = this.GetTabs();

			bool removedTabs = false;
			foreach (MdiTab tab in result)
			{
				Form form = tab.AssociatedForm;
				if ((!senderActivating && sender == tab) || !IsAvailable(form))
				{
					if (form != null)
					{
						skipForms.Add(form);
						form.FormClosed -= this.ChildForm_FormClosed;
						form.TextChanged -= this.ChildForm_TextChanged;
					}

					this.Items.Remove(tab);
					tab.AssociatedForm = null;
					removedTabs = true;

					// This case shouldn't ever occur, but we might as well be safe.
					if (this.dragTab == tab)
					{
						this.EndDragTimer(false);
					}
				}
			}

			// Update the tab list if any were removed.
			if (removedTabs)
			{
				result = this.GetTabs();
			}

			return result;
		}

		private IList<MdiTab> AddNewTabs(IList<MdiTab> tabs, HashSet<Form> skipForms)
		{
			// RemoveClosedTabs should have gotten rid of any tabs with null AssociatedForms,
			// but filtering them out again just to be sure is safe and quick.
			Dictionary<Form, MdiTab> formToTabMap = tabs.Where(t => t.AssociatedForm != null).ToDictionary(tab => tab.AssociatedForm);

			bool addedTabs = false;
			foreach (Form form in this.MdiChildren)
			{
				MdiTab tab;
				if (IsAvailable(form) && !skipForms.Contains(form) && !formToTabMap.TryGetValue(form, out tab))
				{
					tab = new MdiTab(form);
					tab.Click += this.Tab_Click;
					tab.CloseClicked += this.Tab_CloseClicked;

					if (this.UseFormIconAsNewTabImage && form.Icon != null)
					{
						tab.Image = WindowsUtility.GetSmallIconImage(form.Icon);

						// Note: I originally set tab.ImageTransparentColor = Color.Transparent here,
						// but that made some white areas in the icons appear transparent even though
						// they shouldn't have been.  That seems like a .NET bug since icons already
						// have explicitly transparent areas.
					}

					form.FormClosed += this.ChildForm_FormClosed;
					form.TextChanged += this.ChildForm_TextChanged;

					// Add it to the beginning like VS does.  This keeps new tabs from immediately going
					// into the overflow area when lots of forms are open.  This is also consistent with
					// how activating a tab from the overflow area moves it to the beginning of the tab list.
					this.Items.Insert(0, tab);
					addedTabs = true;
				}
			}

			// Update the tab list if any were added.
			IList<MdiTab> result = addedTabs ? this.GetTabs() : tabs;
			return result;
		}

		private void HandleDrag(DragEventArgs e, bool finished)
		{
			MdiTab tab = e.Data.GetData(typeof(ToolStripItem)) as MdiTab;
			if (tab != null)
			{
				var displayedTabs = this.GetDisplayedTabs();
				Point clientPoint = this.PointToClient(new Point(e.X, e.Y));

				MdiTab targetTab = this.FindDisplayedTabAtClientPoint(clientPoint, false);
				if (targetTab != null)
				{
					int targetIndex = displayedTabs.IndexOf(targetTab);
					if (targetIndex >= 0)
					{
						// Determine the target index based on if we're on the left or right side of the button.
						Rectangle targetBounds = targetTab.Bounds;
						if (clientPoint.X > (targetBounds.Left + (targetBounds.Width / 2)))
						{
							targetIndex++;
						}

						int originalIndex = displayedTabs.IndexOf(tab);
						if (originalIndex != targetIndex)
						{
							// Decrement by 1 if new index is > current index because the Insert
							// is going to remove the current tab before re-inserting it.
							if (targetIndex > originalIndex)
							{
								targetIndex--;
							}

							this.Items.Insert(targetIndex, tab);
						}
					}
				}

				// Make sure the dragged tab is selected/highlighted
				// to make the drag object clear to the user.
				tab.Select();

				// If we're done dragging, make sure the tab is checked/activated correctly.
				if (finished)
				{
					this.UpdateTabStrip(tab, true);
				}
			}
		}

		private MdiTab FindDisplayedTabAtClientPoint(Point clientPoint, bool exact)
		{
			MdiTab result = null;

			foreach (MdiTab tab in this.GetDisplayedTabs())
			{
				Rectangle bounds = tab.Bounds;

				// If the point is before the first button, then pretend it's in the first button.
				if (bounds.Contains(clientPoint) || (!exact && clientPoint.X < bounds.X && this.Bounds.Contains(clientPoint)))
				{
					result = tab;
					break;
				}
			}

			return result;
		}

		#endregion

		#region Private Event Handlers

		private void Tab_Click(object sender, EventArgs e)
		{
			this.UpdateTabStrip(sender as MdiTab);
		}

		private void Tab_CloseClicked(object sender, EventArgs e)
		{
			MdiTab tab = sender as MdiTab;
			if (tab != null)
			{
				Form form = tab.AssociatedForm;
				if (form != null)
				{
					form.Close();
				}
			}
		}

		private void ChildForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			MdiTab tab = this.FindTab(sender as Form);
			this.UpdateTabStrip(tab, false);
		}

		private void ChildForm_TextChanged(object sender, EventArgs e)
		{
			Form form = sender as Form;
			if (form != null)
			{
				MdiTab tab = this.FindTab(form);
				if (tab != null)
				{
					tab.Text = form.Text;

					// Note: There's no need to call UpdateUI because changing
					// the form's text only changes the tab caption.  It doesn't
					// show, hide, activate, or do anything else to it.
				}
			}
		}

		private void Form_MdiChildActivate(object sender, EventArgs e)
		{
			// This method is needed to detect newly added child forms.
			// When a new MDI child is first shown, this event fires, and
			// our UpdateTabStrip call will add a tab for it.
			MdiTab tab = this.FindTab(this.ActiveMdiChild);
			this.UpdateTabStrip(tab);
		}

		private void MdiTabStrip_DragEnter(object sender, DragEventArgs e)
		{
			// Don't allow ToolStripItems from other ToolStrips to be dragged into this ToolStrip.
			if (e.Data.GetDataPresent(typeof(ToolStripItem)) && e.Data.GetData(typeof(ToolStripItem)) is MdiTab)
			{
				e.Effect = DragDropEffects.Move;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void MdiTabStrip_DragOver(object sender, DragEventArgs e)
		{
			this.HandleDrag(e, false);
		}

		private void MdiTabStrip_DragDrop(object sender, DragEventArgs e)
		{
			this.HandleDrag(e, true);
		}

		private void DragTimer_Tick(object sender, EventArgs e)
		{
			this.EndDragTimer(true);
		}

		#endregion
	}
}
