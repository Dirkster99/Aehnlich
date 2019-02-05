namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Drawing;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Security.Permissions;
	using System.Text;
	using System.Windows.Forms;

	#endregion

	/// <summary>
	/// A custom list view type that exposes methods for auto-sizing and sorting its columns.
	/// </summary>
	[ToolboxBitmap(typeof(ListView))]
	public class ExtendedListView : ListView
	{
		#region Public Constants

		/// <summary>
		/// Set ColumnHeader.Width to this to auto-size the column based on the text in the items.
		/// </summary>
		public const int AutoSizeByData = -1;

		/// <summary>
		/// Set ColumnHeader.Width to this to auto-size the column based on its header text.
		/// </summary>
		public const int AutoSizeByHeader = -2;

		#endregion

		#region Private Data Members

		private const int LvmFirst = 0x1000;

		private Sorter sorter;
		private IComparer previousSorter;
		private int sorterUpdateLevel;
		private int capacity;
		private Dictionary<int, HeaderData> columnNumberToHeaderDataMap = new Dictionary<int, HeaderData>();
		private Dictionary<int, ListViewColumnType> columnNumberToTypeMap = new Dictionary<int, ListViewColumnType>();

		private bool inDoubleClick;
		private bool doubleClickEventFired;
		private bool mouseDown;
		private bool allowItemCheck = true;
		private Point mouseDownPoint;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ExtendedListView()
		{
			base.FullRowSelect = true;
			base.MultiSelect = false;
			base.View = View.Details;
			base.HideSelection = false;

			this.sorter = new Sorter(this);
			this.ListViewItemSorter = this.sorter;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Changes the inherited <see cref="ListView.HideSelection"/> property to default to false.
		/// </summary>
		[DefaultValue(false)]
		public new bool HideSelection
		{
			get
			{
				return base.HideSelection;
			}

			set
			{
				base.HideSelection = value;
			}
		}

		/// <summary>
		/// Changes the inherited <see cref="ListView.FullRowSelect"/> property to default to true.
		/// </summary>
		[DefaultValue(true)]
		public new bool FullRowSelect
		{
			get
			{
				return base.FullRowSelect;
			}

			set
			{
				base.FullRowSelect = value;
			}
		}

		/// <summary>
		/// Changes the inherited <see cref="ListView.MultiSelect"/> property to default to false.
		/// </summary>
		[DefaultValue(false)]
		[SuppressMessage(
			"Microsoft.Naming",
			"CA1704:IdentifiersShouldBeSpelledCorrectly",
			MessageId = "Multi",
			Justification = "This property name is dictated by the base class.")]
		public new bool MultiSelect
		{
			get
			{
				return base.MultiSelect;
			}

			set
			{
				base.MultiSelect = value;
			}
		}

		/// <summary>
		/// Changes the inherited <see cref="ListView.View"/> property to default to "Details".
		/// </summary>
		[DefaultValue(View.Details)]
		public new View View
		{
			get
			{
				return base.View;
			}

			set
			{
				base.View = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of items that space is reserved for.
		/// </summary>
		/// <remarks>
		/// This setting doesn't affect Items.Count.  It just tells the
		/// Win32 ListView common control to pre-allocate buffers large
		/// enough to allow a certain number of items to be added without
		/// having to reallocate any internal lists.
		/// </remarks>
		[DefaultValue(0)]
		[Browsable(false)]
		public int Capacity
		{
			get
			{
				return this.capacity;
			}

			set
			{
				if (this.capacity != value)
				{
					this.capacity = value;
					this.SetCapacity();
				}
			}
		}

		/// <summary>
		/// Returns the number of items that can be fully visible in the control.
		/// </summary>
		[Browsable(false)]
		public int VisibleItemCount
		{
			get
			{
				const int LVM_GETCOUNTPERPAGE = LvmFirst + 40;
				int result = NativeMethods.SendMessage(this, LVM_GETCOUNTPERPAGE, 0, 0);
				return result;
			}
		}

		/// <summary>
		/// Gets whether a double-click event handler is currently being executed.
		/// </summary>
		[Browsable(false)]
		public bool InDoubleClick => this.inDoubleClick;

		/// <summary>
		/// Gets or sets whether the user can change item checkboxes.
		/// </summary>
		[DefaultValue(true)]
		public bool AllowItemCheck
		{
			get
			{
				return this.allowItemCheck;
			}

			set
			{
				this.allowItemCheck = value;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// See <see cref="ListView.BeginUpdate"/>.
		/// </summary>
		public new void BeginUpdate()
		{
			base.BeginUpdate();
			this.BeginSorterUpdate();
		}

		/// <summary>
		/// See <see cref="ListView.EndUpdate"/>.
		/// </summary>
		public new void EndUpdate()
		{
			this.EndSorterUpdate();
			base.EndUpdate();
		}

		/// <summary>
		/// Sorts the specified column in ascending order.
		/// </summary>
		/// <param name="column">The index of the column to sort.</param>
		public void SortColumn(int column)
		{
			this.sorter.Column = column;
			this.sorter.Ascending = true;
			this.Sort();
		}

		/// <summary>
		/// Sorts the specified column in ascending or descending order.
		/// </summary>
		/// <param name="column">The index of the column to sort.</param>
		/// <param name="ascending">True to sort ascending.  False to sort descending.</param>
		public void SortColumn(int column, bool ascending)
		{
			this.sorter.Column = column;
			this.sorter.Ascending = ascending;
			this.Sort();
		}

		/// <summary>
		/// Sorts using the current ListViewItemSorter.
		/// </summary>
		/// <remarks>
		/// This method adds support for sorting a virtual list view by calling the
		/// <see cref="OnVirtualSortColumn"/> method.
		/// </remarks>
		public new void Sort()
		{
			if (this.VirtualMode)
			{
				if (this.VirtualListSize > 0)
				{
					int column = this.sorter.Column;
					bool ascending = this.sorter.Ascending;

					// Save off the selected and focused items.
					ListViewItem focusedItem = null;
					int numSelectedItems = this.SelectedIndices.Count;
					ListViewItem[] selectedItems = new ListViewItem[numSelectedItems];
					for (int i = 0; i < numSelectedItems; i++)
					{
						ListViewItem item = this.Items[this.SelectedIndices[i]];
						selectedItems[i] = item;
						if (item.Focused)
						{
							focusedItem = item;
						}
					}

					this.BeginUpdate();
					try
					{
						this.SelectedIndices.Clear();

						this.OnVirtualSortColumn(column, ascending, this.GetColumnType(column));

						// If there's only one selected item, then make sure it is visible.
						bool ensureVisible = selectedItems.Length == 1;

						// Now that the list is sorted, restore the selected items.
						foreach (ListViewItem item in selectedItems)
						{
							// Unfortunately, in a virtual list view, we have no way to
							// know the current index for a ListViewItem now that the
							// sort of the backing items has occurred.  Calling ListViewItem's
							// Index property will just return the previous index.  So
							// we have to call a virtual method that a derived class can
							// overload to return the index of where the backing item
							// really is in the list now.
							//
							// A derived list can quickly do an IndexOf in its own collection
							// of objects.  If we tried to do that on the Items collection,
							// it would have to do a RetrieveVirtualItem request for every
							// ListViewItem, which would defeat the purpose of using a
							// a virtual listview.
							int newIndex = this.IndexOf(item);
							ListViewItem newItem = this.Items[newIndex];
							newItem.Selected = true;
							if (item == focusedItem)
							{
								newItem.Focused = true;
							}

							if (ensureVisible)
							{
								newItem.EnsureVisible();
							}
						}
					}
					finally
					{
						this.EndUpdate();
					}
				}
			}
			else
			{
				base.Sort();
			}
		}

		/// <summary>
		/// This returns the column width even inside of a BeginUpdate/EndUpdate pair.
		/// </summary>
		public int GetActualColumnWidth(ColumnHeader header)
		{
			Conditions.RequireReference(header, "header");
			const int LVM_GETCOLUMNWIDTH = LvmFirst + 29;
			int result = NativeMethods.SendMessage(this, LVM_GETCOLUMNWIDTH, header.Index, 0);
			return result;
		}

		/// <summary>
		/// Auto-sizes the column to fit both the header and data text.  This will grow or shrink the column as necessary.
		/// </summary>
		/// <param name="column">The column to auto-size.</param>
		public void AutoSizeColumn(ColumnHeader column)
		{
			this.AutoSizeColumn(column, true);
		}

		/// <summary>
		/// Auto-sizes the column to fit both the header and data text.
		/// </summary>
		/// <param name="column">The column to auto-size.</param>
		/// <param name="allowShrinking">Whether the column should be allowed to get smaller.</param>
		public void AutoSizeColumn(ColumnHeader column, bool allowShrinking)
		{
			Conditions.RequireReference(column, "column");

			this.BeginUpdate();
			try
			{
				int originalWidth = column.Width;

				// Only auto-size the header if it is showing.
				int headerWidth = 0;
				if (this.HeaderStyle != ColumnHeaderStyle.None)
				{
					bool autoSizeHeader = false;

					// Only do the Header auto-size the first time and when the column name or position changes.
					HeaderData data;
					if (!this.columnNumberToHeaderDataMap.TryGetValue(column.Index, out data))
					{
						data = new HeaderData(column.Text);
						this.columnNumberToHeaderDataMap.Add(column.Index, data);
						autoSizeHeader = true;
					}
					else if (column.Index == this.Columns.Count - 1)
					{
						// The AutoSizeByHeader logic for the last column
						// will size the header to fit the remaining width,
						// so we always want to use it because the control
						// or one of the other columns may have changed size
						// (possibly by a means we can't detect, like someone
						// set Column.Width directly).
						autoSizeHeader = true;
						data.IsLastColumn = true;
					}
					else if (column.Index != this.Columns.Count - 1 && data.IsLastColumn)
					{
						// The column used to be the last column, but now it isn't.
						// So we need to resize it because AutoSizeByHeader
						// probably made it take up all the remaining width before,
						// and now it shouldn't do that.
						autoSizeHeader = true;
						data.IsLastColumn = false;
					}
					else if (data.Name != column.Text)
					{
						autoSizeHeader = true;
						data.Name = column.Text;
					}

					if (autoSizeHeader)
					{
						column.Width = AutoSizeByHeader;
						data.AutoHeaderWidth = this.GetActualColumnWidth(column);
					}

					headerWidth = data.AutoHeaderWidth;
				}

				// Since the data width is usually larger than the header text width,
				// we'll do it last since we'll probably end up keeping the data width.
				int contentWidth = 0;
				if (this.Items.Count > 0)
				{
					column.Width = AutoSizeByData;
					contentWidth = this.GetActualColumnWidth(column);

					// This is a bug workaround for XP's "themed" ListView.  It sizes the
					// first column slightly too small when no images are used with the
					// control.  It doesn't do that when "themes" are turned off.
					if (column.Index == 0 && WindowsUtility.AreVisualStylesEnabled && this.SmallImageList == null)
					{
						contentWidth += SystemInformation.SmallIconSize.Width / 2;
					}
				}

				int newWidth = Math.Max(headerWidth, contentWidth);

				if (!allowShrinking && newWidth < originalWidth)
				{
					// Set the column width back to what it was.
					column.Width = originalWidth;
				}
				else if (newWidth != column.Width)
				{
					column.Width = newWidth;
				}
			}
			finally
			{
				this.EndUpdate();
			}
		}

		/// <summary>
		/// Auto-sizes all columns.
		/// </summary>
		public void AutoSizeColumns()
		{
			this.BeginUpdate();
			try
			{
				foreach (ColumnHeader column in this.Columns)
				{
					this.AutoSizeColumn(column);
				}
			}
			finally
			{
				this.EndUpdate();
			}
		}

		/// <summary>
		/// Inserts a range of items at the specified index.
		/// </summary>
		/// <param name="index">The index to start the insertion at.</param>
		/// <param name="items">The array of items to insert.</param>
		public void InsertRange(int index, ListViewItem[] items)
		{
			this.InsertRange(index, items, false);
		}

		/// <summary>
		/// Inserts a range of items at the specified index optionally
		/// in reverse order.
		/// </summary>
		/// <param name="index">The index to start the insertion at.</param>
		/// <param name="items">The array of items to insert.</param>
		/// <param name="reverseOrder">Whether the items should be inserted in reverse order.</param>
		public void InsertRange(int index, ListViewItem[] items, bool reverseOrder)
		{
			this.BeginSorterUpdate();
			try
			{
				int numItems = items.Length;
				this.Capacity = this.Items.Count + numItems;

				if (reverseOrder)
				{
					for (int i = 0; i < numItems; i++)
					{
						this.Items.Insert(index + i, items[numItems - 1 - i]);
					}
				}
				else
				{
					for (int i = 0; i < numItems; i++)
					{
						this.Items.Insert(index + i, items[i]);
					}
				}
			}
			finally
			{
				this.EndSorterUpdate();
			}
		}

		/// <summary>
		/// Saves the list contents to a tab-separated text file.
		/// </summary>
		/// <param name="fileName">The name of the file to save to.</param>
		public void SaveAsText(string fileName)
		{
			this.SaveAsText(fileName, "\t");
		}

		/// <summary>
		/// Saves the list contents to a token-separated text file.
		/// </summary>
		/// <param name="fileName">The name of the file to save to.</param>
		/// <param name="separator">The separator string to put between each value in a row.</param>
		public void SaveAsText(string fileName, string separator)
		{
			using (StreamWriter writer = File.CreateText(fileName))
			{
				this.GetAsText(separator, true, this.Items, writer);
			}
		}

		/// <summary>
		/// Gets the list view data for all items as text.
		/// </summary>
		/// <param name="separator">The separator string to put between each value in a row.</param>
		/// <param name="includeHeaders">Whether the column headers should be included.</param>
		/// <returns>The text representation of the list view.</returns>
		public string GetAsText(string separator, bool includeHeaders)
		{
			using (StringWriter writer = new StringWriter())
			{
				this.GetAsText(separator, includeHeaders, this.Items, writer);
				string result = writer.ToString();
				return result;
			}
		}

		/// <summary>
		/// Gets the list view data for the selected items as text.
		/// </summary>
		/// <param name="separator">The separator string to put between each value in a row.</param>
		/// <param name="includeHeaders">Whether the column headers should be included.</param>
		/// <param name="items">The collection of ListViewItems to write.</param>
		/// <param name="writer">The text writer to append to.</param>
		public void GetAsText(string separator, bool includeHeaders, IEnumerable<ListViewItem> items, TextWriter writer)
		{
			Conditions.RequireReference(items, "items");
			Conditions.RequireReference(writer, "writer");
			this.GetAsText(separator, includeHeaders, (IEnumerable)items, writer);
		}

		/// <summary>
		/// Informs the ListView that you've initially populated it with a column in pre-sorted order.
		/// </summary>
		/// <remarks>
		/// If you load a list in order, then this method allows you to tell the ListView control about
		/// the pre-existing sort order without having to incur the overhead of an explicit <see cref="Sort"/>
		/// operation.  Then when a user clicks on the header for the sorted column it can swap the
		/// sort order instead of just re-sorting into the same order.
		/// </remarks>
		/// <param name="column">The column that the list entries are already sorted by.</param>
		/// <param name="ascending">True if the column is sorted in ascending order.  False if it is in descending order.</param>
		public void SetPresortedOrder(int column, bool ascending)
		{
			this.sorter.Column = column;
			this.sorter.Ascending = ascending;
		}

		/// <summary>
		/// Sets the column type for the specified column.  This is used for sorting.
		/// </summary>
		/// <param name="column">The column index.</param>
		/// <param name="columnType">The column type.</param>
		public void SetColumnType(int column, ListViewColumnType columnType)
		{
			this.columnNumberToTypeMap[column] = columnType;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Determines the given column's type.
		/// </summary>
		/// <param name="column">The index of the column.</param>
		/// <returns>The column's type.</returns>
		/// <remarks>
		/// By default, this assumes it is numeric if it is right-aligned,
		/// which works for most cases.  Derived classes can change
		/// this behavior if necessary by overriding this function.
		/// </remarks>
		protected internal virtual ListViewColumnType GetColumnType(int column)
		{
			ListViewColumnType result;
			if (!this.columnNumberToTypeMap.TryGetValue(column, out result))
			{
				result = this.Columns[column].TextAlign == HorizontalAlignment.Right ? ListViewColumnType.Number : ListViewColumnType.String;
			}

			return result;
		}

		/// <summary>
		/// Compares two <see cref="ListViewItem"/> instances during a sort.
		/// </summary>
		/// <param name="itemX">The left-hand item to compare.</param>
		/// <param name="itemY">The right-hand item to compare.</param>
		/// <param name="column">The column to compare for.</param>
		/// <param name="ascending">Whether an ascending or decending sort is being done.</param>
		/// <param name="columnType">The column's type based on the result of <see cref="GetColumnType"/>.</param>
		/// <returns>-1 if ItemX is less than ItemY.  0 if they're equal.  1 if ItemX is greater than ItemY.</returns>
		protected internal virtual int CompareItems(ListViewItem itemX, ListViewItem itemY, int column, bool ascending, ListViewColumnType columnType)
		{
			// For strings this does a case-insensitive comparison.
			// For numeric columns this does a double comparison.
			// For dates this does a date comparision.
			// Derived classes can change this behavior if necessary
			// by overriding this function.
			// Note: Sometimes, not all of the SubItems are populated.
			string textX;
			if (column < itemX.SubItems.Count)
			{
				textX = itemX.SubItems[column].Text;
			}
			else
			{
				textX = string.Empty;
				columnType = ListViewColumnType.String;
			}

			string textY;
			if (column < itemY.SubItems.Count)
			{
				textY = itemY.SubItems[column].Text;
			}
			else
			{
				textY = string.Empty;
				columnType = ListViewColumnType.String;
			}

			int result = 0;

			switch (columnType)
			{
				case ListViewColumnType.Number:
					double doubleX, doubleY;
					if (double.TryParse(textX, out doubleX) && double.TryParse(textY, out doubleY))
					{
						result = Math.Sign(doubleX - doubleY);
						break;
					}
					else
					{
						goto default;
					}

				case ListViewColumnType.DateTime:
					DateTime dateTimeX, dateTimeY;
					if (DateTime.TryParse(textX, out dateTimeX) && DateTime.TryParse(textY, out dateTimeY))
					{
						result = dateTimeX.CompareTo(dateTimeY);
						break;
					}
					else
					{
						goto default;
					}

				default:
					result = string.Compare(textX, textY, StringComparison.OrdinalIgnoreCase);
					break;
			}

			if (!ascending)
			{
				result = -result;
			}

			return result;
		}

		/// <summary>
		/// Called from a virtual listview (i.e., <see cref="ListView.VirtualMode"/> is true) when a column header is clicked
		/// or when <see cref="SortColumn(int, bool)"/> is called.
		/// </summary>
		/// <remarks>
		/// You should not call this method from an override in a derived class.  All it does is throw an exception
		/// saying that the derived class needs to provide the implementation for it.
		/// </remarks>
		/// <param name="column">The column to sort.</param>
		/// <param name="ascending">Whether an ascending or decending sort should be done.</param>
		/// <param name="columnType">The column's type based on the result of <see cref="GetColumnType"/>.</param>
		protected virtual void OnVirtualSortColumn(int column, bool ascending, ListViewColumnType columnType)
		{
			throw Exceptions.NewInvalidOperationException(
				"A sort was requested on a virtual ListView, but the list didn't override the OnVirtualSortColumn method.");
		}

		/// <summary>
		/// Overriden to sort the column in a toggling manner like Explorer does.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnColumnClick(ColumnClickEventArgs e)
		{
			bool ascending = true;

			// Toggle the sort direction if they clicked the currently sorted column.
			if (this.sorter.Column != -1 && e.Column == this.sorter.Column)
			{
				ascending = !this.sorter.Ascending;
			}

			this.SortColumn(e.Column, ascending);
			base.OnColumnClick(e);
		}

		/// <summary>
		/// Overriden to set the capacity if necessary.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			this.SetCapacity();
		}

		/// <summary>
		/// Processes Windows messages.
		/// </summary>
		/// <param name="m">The Windows message to process.</param>
		protected override void WndProc(ref Message m)
		{
			// We need to change three things about the default ListView's double-click behavior:
			// 1.  We don't want double-clicks to cause items to be checked/unchecked.
			// 2.  We want the event to fire even if they click in the whitespace (i.e. not on an item).
			// 3.  We don't want the event to fire again after a double-click launches a modal, that
			//     gets closed, and then someone tries to check or uncheck the item.  Somehow in
			//     .NET 1.0 that sends a WM_LBUTTONUP message and fires the OnDoubleClick override!
			const int WM_LBUTTONDBLCLK = 0x0203;
			if (m.Msg == WM_LBUTTONDBLCLK)
			{
				this.doubleClickEventFired = false;
				this.inDoubleClick = true;
				try
				{
					base.WndProc(ref m);
				}
				finally
				{
					this.inDoubleClick = false;
				}

				if (!this.doubleClickEventFired)
				{
					this.doubleClickEventFired = true;
					base.OnDoubleClick(EventArgs.Empty);
				}

				this.doubleClickEventFired = false;
			}
			else
			{
				base.WndProc(ref m);
			}
		}

		/// <summary>
		/// Called when a double-click event occurs.
		/// </summary>
		/// <param name="e">The event args.</param>
		protected override void OnDoubleClick(EventArgs e)
		{
			if (this.inDoubleClick)
			{
				this.doubleClickEventFired = true;
				base.OnDoubleClick(e);
			}
		}

		/// <summary>
		/// Called when an item is checked.
		/// </summary>
		/// <param name="ice">The event args.</param>
		protected override void OnItemCheck(ItemCheckEventArgs ice)
		{
			bool callBaseImplementation = true;

			// Don't let double click change the check state.
			// Also, don't let mouse multi-selection change the
			// check states.
			if (!this.allowItemCheck || this.inDoubleClick)
			{
				ice.NewValue = ice.CurrentValue;
				callBaseImplementation = false;
			}
			else if (this.mouseDown && this.SelectedIndices.Count > 1)
			{
				// Only allow a mouse click to change multiple checks
				// if they clicked on one item.  If they changed items,
				// this works around a ListView bug.
				ListViewItem mouseDownItem = this.GetItemAt(this.mouseDownPoint.X, this.mouseDownPoint.Y);
				Point currentPoint = this.PointToClient(Control.MousePosition);
				ListViewItem currentMouseItem = this.GetItemAt(currentPoint.X, currentPoint.Y);

				// The list view also has a bug where it will do item checks
				// during multi-selection with Ctrl+Click.
				if (mouseDownItem != currentMouseItem || !this.IsPointInCheck(currentPoint, ice.Index))
				{
					ice.NewValue = ice.CurrentValue;
					callBaseImplementation = false;
				}
			}

			if (callBaseImplementation)
			{
				base.OnItemCheck(ice);
			}
		}

		/// <summary>
		/// Called when a mouse button is pressed.
		/// </summary>
		/// <param name="e">The event args.</param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.Capture = true;
			this.mouseDown = true;
			this.mouseDownPoint = new Point(e.X, e.Y);
		}

		/// <summary>
		/// Called when a mouse button is released.
		/// </summary>
		/// <param name="e">The event args.</param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			this.Capture = false;
			this.mouseDown = false;
		}

		/// <summary>
		/// Used to find the new index for a ListViewItem after a sort
		/// or any other operation that changes the virtual collection
		/// of items.
		/// </summary>
		/// <remarks>
		/// A derived class should override this to do an IndexOf on its
		/// own collection of objects associated with the listview items.
		/// <p/>
		/// The Index property of the <paramref name="item"/> parameter
		/// will still report the last index the item had before the collection
		/// was changed.
		/// </remarks>
		/// <param name="item">
		/// The listview item to lookup.  If you have
		/// associated an object with the Tag property it will be available
		/// so you can look it up in your own collection.
		/// </param>
		/// <returns>The new index where this listview item is located.</returns>
		protected virtual int IndexOf(ListViewItem item)
		{
			int result = item.Index;
			return result;
		}

		#endregion

		#region Private Methods

		private void SetCapacity()
		{
			if (this.capacity > 0 && this.IsHandleCreated)
			{
				const int LVM_SETITEMCOUNT = LvmFirst + 47;
				const int LVSICF_NOINVALIDATEALL = 1;
				NativeMethods.SendMessage(this, LVM_SETITEMCOUNT, this.capacity, LVSICF_NOINVALIDATEALL);
			}
		}

		private bool IsPointInCheck(Point point, int index)
		{
			bool result = false;

			if (this.Columns.Count > 0)
			{
				ListViewItem item = this.Items[index];
				int scrollPos = NativeMethods.GetScrollPos(this, true);
				int posX = point.X + scrollPos;

				const int DefaultStateImageWidth = 16;
				int imageWidth = this.StateImageList != null ? this.StateImageList.ImageSize.Width : DefaultStateImageWidth;

				int checkStart = imageWidth * item.IndentCount;
				int checkStop = checkStart + imageWidth;

				result = posX >= checkStart && posX <= checkStop;
			}

			return result;
		}

		private void BeginSorterUpdate()
		{
			if (this.sorterUpdateLevel == 0)
			{
				this.previousSorter = this.ListViewItemSorter;
				this.ListViewItemSorter = null;
			}

			this.sorterUpdateLevel++;
		}

		private void EndSorterUpdate()
		{
			this.sorterUpdateLevel--;
			if (this.sorterUpdateLevel == 0)
			{
				this.ListViewItemSorter = this.previousSorter;
				this.previousSorter = null;
			}
		}

		private void GetAsText(string separator, bool includeHeaders, IEnumerable items, TextWriter writer)
		{
			// Write out the column headers first.
			if (includeHeaders)
			{
				int numColumns = this.Columns.Count;
				for (int i = 0; i < numColumns; i++)
				{
					if (i != 0)
					{
						writer.Write(separator);
					}

					writer.Write(this.Columns[i].Text);
				}

				writer.WriteLine();
			}

			// Write out the column data.
			foreach (ListViewItem item in items)
			{
				int numSubItems = item.SubItems.Count;
				for (int i = 0; i < numSubItems; i++)
				{
					if (i != 0)
					{
						writer.Write(separator);
					}

					writer.Write(item.SubItems[i].Text);
				}

				writer.WriteLine();
			}
		}

		#endregion

		#region Private Types

		private sealed class HeaderData
		{
			#region Constructors

			public HeaderData(string name)
			{
				this.Name = name;
			}

			#endregion

			#region Public Properties

			public string Name { get; set; }

			public int AutoHeaderWidth { get; set; }

			public bool IsLastColumn { get; set; }

			#endregion
		}

		private sealed class Sorter : IComparer
		{
			#region Private Data Members

			private ExtendedListView listView;
			private int column = -1;
			private bool ascending = true;
			private ListViewColumnType columnType;

			#endregion

			#region Constructors

			public Sorter(ExtendedListView lstView)
			{
				this.listView = lstView;
			}

			#endregion

			#region Public Properties

			public int Column
			{
				get
				{
					return this.column;
				}

				set
				{
					if (this.column != value)
					{
						this.column = value;

						// Since the column changed, we always want to reset to an ascending sort.
						this.ascending = true;
						this.CheckData();
					}
				}
			}

			public bool Ascending
			{
				get
				{
					return this.ascending;
				}

				set
				{
					this.ascending = value;
				}
			}

			#endregion

			#region Public Methods

			public int Compare(object x, object y)
			{
				int result = 0;

				if (this.column >= 0)
				{
					ListViewItem itemX = (ListViewItem)x;
					ListViewItem itemY = (ListViewItem)y;

					result = this.listView.CompareItems(itemX, itemY, this.column, this.ascending, this.columnType);
				}

				return result;
			}

			#endregion

			#region Private Methods

			private void CheckData()
			{
				if (this.listView != null && this.column >= 0 && this.column < this.listView.Columns.Count)
				{
					this.columnType = this.listView.GetColumnType(this.column);
				}
				else
				{
					this.column = -1;
					this.columnType = ListViewColumnType.String;
				}
			}

			#endregion
		}

		#endregion
	}
}
