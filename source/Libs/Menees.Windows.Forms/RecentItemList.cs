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
	/// Used to manage a list of recent items (e.g., files, projects, directories).
	/// </summary>
	[DefaultEvent("ItemClick")]
	[ToolboxBitmap(typeof(RecentItemList), "Images.RecentItemList.bmp")]
	public partial class RecentItemList : Component
	{
		#region Private Data Members

		private const string DefaultSettingsNodeName = "Recent Items";

		private int maxItems = 10;
		private FormSaver formSaver;
		private List<string> items = new List<string>();
		private Dictionary<string, IEnumerable<string>> itemToValuesMap = new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase);
		private string settingsNodeName = DefaultSettingsNodeName;
		private ToolStripMenuItem menuItem;
		private ContextMenuStrip contextMenu;
		private EventHandler<SettingsEventArgs> loadHandler;
		private EventHandler<SettingsEventArgs> saveHandler;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public RecentItemList()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Creates a new instance for the specified container.
		/// </summary>
		public RecentItemList(IContainer container)
		{
			container.Add(this);
			this.InitializeComponent();
		}

		#endregion

		#region Public Events

		/// <summary>
		/// Called when a "recent item" menu item is clicked.
		/// </summary>
		[Browsable(true)]
		[Category("Action")]
		[Description("Called when a \"recent item\" menu item is clicked.")]
		public event EventHandler<RecentItemClickEventArgs> ItemClick;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the maximum number of recent items to manage.
		/// </summary>
		[Browsable(true)]
		[DefaultValue(10)]
		[Category("Behavior")]
		[Description("The maximum number of recent items to manage.")]
		public int MaxItemCount
		{
			get
			{
				return this.maxItems;
			}

			set
			{
				this.maxItems = Math.Max(0, value);
				this.CropItemsAndUpdate(false);
			}
		}

		/// <summary>
		/// Gets or sets the node name where recent item settings should be saved.  This must be non-empty.
		/// </summary>
		[Browsable(true)]
		[DefaultValue(DefaultSettingsNodeName)]
		[Category("Behavior")]
		[Description("The node name where recent item settings should be saved.  This must be non-empty.")]
		public string SettingsNodeName
		{
			get
			{
				return this.settingsNodeName;
			}

			set
			{
				// The section name must be non-empty because Save needs to delete the
				// recent items SettingsNode, and we don't want it to delete the base key instead.
				Conditions.RequireString(value, "value");
				this.settingsNodeName = value.Trim();
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="FormSaver"/> object used to save and load settings.
		/// </summary>
		[Browsable(true)]
		[DefaultValue(null)]
		[Category("Helper Objects")]
		[Description("The FormSaver object used to save and load settings.")]
		public FormSaver FormSaver
		{
			get
			{
				return this.formSaver;
			}

			set
			{
				if (this.formSaver != value)
				{
					// Detach from old events
					if (this.formSaver != null)
					{
						this.formSaver.InternalLoadSettings -= this.loadHandler;
						this.formSaver.InternalSaveSettings -= this.saveHandler;
					}

					this.formSaver = value;

					// Attach to new events
					if (this.formSaver != null)
					{
						// Create event handlers
						if (this.loadHandler == null)
						{
							this.loadHandler = new EventHandler<SettingsEventArgs>(this.OnLoadSettings);
						}

						if (this.saveHandler == null)
						{
							this.saveHandler = new EventHandler<SettingsEventArgs>(this.OnSaveSettings);
						}

						// Attach to the internal events so we're assured of getting
						// called before the public events.  This ensures that normal
						// MainForm.FormSaver_LoadSettings event handlers fire after
						// the recent items have been loaded, which forms sometimes
						// need to check if they want to reload the last item used.
						this.formSaver.InternalLoadSettings += this.loadHandler;
						this.formSaver.InternalSaveSettings += this.saveHandler;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the menu item that should contain the recent items.
		/// </summary>
		[Browsable(true)]
		[DefaultValue(null)]
		[Category("Helper Objects")]
		[Description("The menu item that should contain the recent items.")]
		public ToolStripMenuItem MenuItem
		{
			get
			{
				return this.menuItem;
			}

			set
			{
				if (this.menuItem != value)
				{
					// Delete any menu items on the old menu
					this.DeleteMenuItems();

					this.menuItem = value;

					// Build the new menu
					this.UpdateMenu();
				}
			}
		}

		/// <summary>
		/// Gets or sets the context menu that should contain the recent items.
		/// </summary>
		[Browsable(true)]
		[DefaultValue(null)]
		[Category("Helper Objects")]
		[Description("The context menu that should contain the recent items.")]
		public ContextMenuStrip ContextMenuStrip
		{
			get
			{
				return this.contextMenu;
			}

			set
			{
				if (this.contextMenu != value)
				{
					this.DeleteMenuItems();

					this.contextMenu = value;

					this.UpdateMenu();
				}
			}
		}

		/// <summary>
		/// Gets the current number of items.
		/// </summary>
		[Browsable(false)]
		[Description("The current number of items.")]
		public int Count => this.items.Count;

		/// <summary>
		/// Gets or sets the list of items as a collection of strings.
		/// </summary>
		[Browsable(false)]
		[Description("Gets or sets the list of items as a collection of strings.")]
		public IEnumerable<string> Items
		{
			get
			{
				// Return a copy, so the caller can't muck with our internal list.
				string[] result = this.items.ToArray();
				return result;
			}

			set
			{
				// Get rid of the old menu items and item data.
				this.DeleteMenuItems();
				this.items.Clear();
				this.itemToValuesMap.Clear();

				// Add the new items.
				if (value != null)
				{
					this.items.AddRange(value.Where(s => !string.IsNullOrWhiteSpace(s)));
				}

				// Rebuild the menu
				this.CropItemsAndUpdate(true);
			}
		}

		/// <summary>
		/// Gets the item for the specified index.
		/// </summary>
		/// <param name="index">The 0-based index of an item.</param>
		/// <returns>The requested item.</returns>
		[Browsable(false)]
		[Description("Gets the item for the specified index.")]
		public string this[int index] => this.items[index];

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets whether the specified menu item is a dummy "&lt;None&gt;" menu item.
		/// </summary>
		/// <remarks>
		/// This is useful in advanced scenarios where a parent form needs to disable
		/// a menu recursively (e.g., if it's switching from one "editor" to another)
		/// and then re-enable it later.  The "None" menu items should never be enabled.
		/// </remarks>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsNoneMenuItem(ToolStripMenuItem menuItem)
		{
			Conditions.RequireReference(menuItem, "menuItem");
			bool result = menuItem is NoneMenuItem;
			return result;
		}

		/// <summary>
		/// Adds an item to the list.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(string item)
		{
			this.Add(item, null);
		}

		/// <summary>
		/// Adds an item to the list along with associated values.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <param name="values">An optional array of values to associate with the item.</param>
		public void Add(string item, IEnumerable<string> values)
		{
			Conditions.RequireString(item, "item");

			item = item.Trim();
			if (item.Length > 0)
			{
				this.Remove(item);
				this.items.Insert(0, item);
				this.itemToValuesMap[item] = values;
				this.CropItemsAndUpdate(true);
			}
		}

		/// <summary>
		/// Removes an item from the list.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		/// <returns>True if the item was found and removed.</returns>
		public bool Remove(string item)
		{
			Conditions.RequireString(item, "item");

			bool result = false;
			item = item.Trim();
			int index = this.IndexOf(item);
			if (index >= 0)
			{
				this.items.RemoveAt(index);
				this.itemToValuesMap.Remove(item);
				result = true;
				this.UpdateMenu();
			}

			return result;
		}

		/// <summary>
		/// Gets the 0-based index of an item.
		/// </summary>
		/// <param name="item">An item to lookup.</param>
		/// <returns>The 0-based index of an item, or -1 if the item isn't found.</returns>
		public int IndexOf(string item)
		{
			int result = -1;

			// Note: We can't use this.items.IndexOf() because it is case-sensitive.
			int count = this.items.Count;
			for (int i = 0; i < count; i++)
			{
				if (string.Compare(item, this.items[i], StringComparison.OrdinalIgnoreCase) == 0)
				{
					result = i;
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// Removes all items from the list.
		/// </summary>
		public void Clear()
		{
			this.items.Clear();
			this.itemToValuesMap.Clear();
			this.UpdateMenu();
		}

		/// <summary>
		/// Loads the recent item settings.
		/// </summary>
		/// <param name="baseNode">The base settings node to look for <see cref="SettingsNodeName"/> under.</param>
		public void Load(ISettingsNode baseNode)
		{
			Conditions.RequireReference(baseNode, "baseNode");

			ISettingsNode settingsNode = baseNode.GetSubNode(this.SettingsNodeName, false);
			if (settingsNode != null)
			{
				int itemIndex = 0;
				while (true)
				{
					string item = settingsNode.GetValue(itemIndex.ToString(), string.Empty);
					if (item.Length == 0)
					{
						break;
					}

					this.items.Add(item);

					// Load any custom values if they exist.
					ISettingsNode valuesNode = settingsNode.GetSubNode(itemIndex + "_Values", false);
					if (valuesNode != null)
					{
						int count = valuesNode.GetValue("Count", 0);
						string[] values = new string[count];
						for (int stringIndex = 0; stringIndex < count; stringIndex++)
						{
							values[stringIndex] = valuesNode.GetValue(stringIndex.ToString(), string.Empty);
						}

						this.itemToValuesMap[item] = values;
					}

					itemIndex++;
				}
			}

			this.UpdateMenu();
		}

		/// <summary>
		/// Saves the recent item settings.
		/// </summary>
		/// <param name="baseNode">The base settings node to look for <see cref="SettingsNodeName"/> under.</param>
		public void Save(ISettingsNode baseNode)
		{
			Conditions.RequireReference(baseNode, "baseNode");

			// Clear out any old settings.
			ISettingsNode settingsNode = baseNode.GetSubNode(this.SettingsNodeName, false);
			if (settingsNode != null)
			{
				baseNode.DeleteSubNode(this.SettingsNodeName);
			}

			// (Re)Create the section key.
			settingsNode = baseNode.GetSubNode(this.SettingsNodeName, true);
			if (settingsNode != null)
			{
				int numItems = this.items.Count;
				for (int itemIndex = 0; itemIndex < numItems; itemIndex++)
				{
					string item = this.items[itemIndex];
					settingsNode.SetValue(itemIndex.ToString(), item);

					// If they attached custom values to the item, then save those too.
					IEnumerable<string> values;
					this.itemToValuesMap.TryGetValue(item, out values);
					if (values != null)
					{
						ISettingsNode valuesNode = settingsNode.GetSubNode(itemIndex + "_Values", true);
						int count = values.Count();
						valuesNode.SetValue("Count", count);
						int stringIndex = 0;
						foreach (string value in values)
						{
							valuesNode.SetValue(stringIndex.ToString(), value);
							stringIndex++;
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the values associated with an item.
		/// </summary>
		/// <param name="item">An item to lookup.</param>
		/// <returns>The values associated with the item.  This can be null.</returns>
		public IEnumerable<string> GetItemValues(string item)
		{
			IEnumerable<string> result;
			this.itemToValuesMap.TryGetValue(item, out result);
			return result;
		}

		#endregion

		#region Private Methods

		private void CropItemsAndUpdate(bool update)
		{
			while (this.items.Count > this.maxItems && this.items.Count > 0)
			{
				update = true;
				string item = this.items[this.items.Count - 1];
				this.items.RemoveAt(this.items.Count - 1);
				this.itemToValuesMap.Remove(item);
			}

			if (update)
			{
				this.UpdateMenu();
			}
		}

		private void AddMenuItems()
		{
			if (this.menuItem != null)
			{
				this.AddMenuItems(this.menuItem.DropDownItems);
			}

			if (this.contextMenu != null)
			{
				this.AddMenuItems(this.contextMenu.Items);
			}
		}

		private void AddMenuItems(ToolStripItemCollection menuItems)
		{
			if (!this.DesignMode && menuItems != null)
			{
				int numItems = this.items.Count;
				if (numItems > 0)
				{
					for (int i = 0; i < numItems; i++)
					{
						string item = this.items[i];
						const int MaxSingleDigitNumber = 9;
						string menuText = string.Format("{0}{1}:  {2}", (i < MaxSingleDigitNumber) ? "&" : string.Empty, i + 1, item);
						IEnumerable<string> values;
						this.itemToValuesMap.TryGetValue(item, out values);
						ToolStripMenuItem menuItem = new RecentItemMenuItem(menuText, new EventHandler(this.OnMenuItemClick), item, values);
						menuItems.Add(menuItem);
					}
				}
				else
				{
					// Add a disabled dummy item with no Click handler.
					menuItems.Add(new NoneMenuItem());
				}
			}
		}

		private void DeleteMenuItems()
		{
			if (this.menuItem != null)
			{
				this.DeleteMenuItems(this.menuItem.DropDownItems);
			}

			if (this.contextMenu != null)
			{
				this.DeleteMenuItems(this.contextMenu.Items);
			}
		}

		private void DeleteMenuItems(ToolStripItemCollection menuItems)
		{
			if (!this.DesignMode && menuItems != null)
			{
				menuItems.Clear();
			}
		}

		private void UpdateMenu()
		{
			this.DeleteMenuItems();
			this.AddMenuItems();
		}

		private void OnMenuItemClick(object sender, EventArgs e)
		{
			RecentItemMenuItem menuItem = sender as RecentItemMenuItem;
			if (this.ItemClick != null && menuItem != null)
			{
				this.ItemClick(this, new RecentItemClickEventArgs(menuItem.Item, menuItem.Values));
			}
		}

		private void OnLoadSettings(object sender, SettingsEventArgs e)
		{
			if (!this.DesignMode)
			{
				this.Load(e.SettingsNode);
			}
		}

		private void OnSaveSettings(object sender, SettingsEventArgs e)
		{
			if (!this.DesignMode)
			{
				this.Save(e.SettingsNode);
			}
		}

		#endregion

		#region Private Types

		private sealed class RecentItemMenuItem : ToolStripMenuItem
		{
			#region Constructors

			public RecentItemMenuItem(string text, EventHandler eh, string item, IEnumerable<string> values)
				: base(text, null, eh)
			{
				this.Item = item;
				this.Values = values;
			}

			#endregion

			#region Public Properties

			public string Item { get; }

			public IEnumerable<string> Values { get; }

			#endregion
		}

		private sealed class NoneMenuItem : ToolStripMenuItem
		{
			#region Constructors

			public NoneMenuItem()
			{
				this.Text = "<None>";
				this.Enabled = false;
			}

			#endregion
		}

		#endregion
	}
}
