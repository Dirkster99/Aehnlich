namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Text;
	using System.Windows.Forms;

	#endregion

	/// <summary>
	/// Adds commonly needed functionality to the Windows Forms PropertyGrid.
	/// </summary>
	public sealed partial class ExtendedPropertyGrid : PropertyGrid
	{
		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ExtendedPropertyGrid()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Creates a new instance for the specified container.
		/// </summary>
		/// <param name="container">The container for this component.</param>
		public ExtendedPropertyGrid(IContainer container)
		{
			container.Add(this);

			this.InitializeComponent();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Selects the specified property on the currently selected object.
		/// </summary>
		/// <param name="propertyName">The name of a property to select.</param>
		public void SelectProperty(string propertyName)
		{
			// Select the specific property in the grid if propertyName is non-empty.
			if (!string.IsNullOrEmpty(propertyName))
			{
				// According to the following thread SelectedGridItem will never be null, and it
				// can be used to get GridItem.Parent.GridItems.
				// http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=542741&SiteID=1
				GridItem selectedItem = this.SelectedGridItem;
				if (selectedItem != null)
				{
					// Search up to the root item, so we can then search only top-level properties.
					while (selectedItem.GridItemType != GridItemType.Root)
					{
						selectedItem = selectedItem.Parent;
					}

					// If the name of a non-browsable property is specified, then this will
					// return null.  But we can't assign null to SelectedGridItem because
					// it will throw an ArgumentException.
					GridItem result = FindPropertyGridItem(selectedItem.GridItems, propertyName);
					if (result != null)
					{
						this.SelectedGridItem = result;
					}
				}
			}
		}

		/// <summary>
		/// Forces the grid to refresh itself in case properties were updated.
		/// </summary>
		public void RefreshProperties()
		{
			object[] selection = this.SelectedObjects;

			// Try to save the name of the currently selected property.
			string selectedPropertyName = null;
			if (selection != null && selection.Length > 0)
			{
				GridItem gridItem = this.SelectedGridItem;
				if (gridItem != null && gridItem.GridItemType == GridItemType.Property)
				{
					selectedPropertyName = gridItem.PropertyDescriptor.Name;
				}
			}

			this.SelectedObjects = null;
			this.SelectedObjects = selection;

			// Try to restore the previously selected property.
			if (!string.IsNullOrEmpty(selectedPropertyName))
			{
				this.SelectProperty(selectedPropertyName);
			}
		}

		#endregion

		#region Private Methods

		private static GridItem FindPropertyGridItem(GridItemCollection gridItems, string propertyName)
		{
			GridItem result = null;

			foreach (GridItem item in gridItems)
			{
				if (item.GridItemType == GridItemType.Category)
				{
					result = FindPropertyGridItem(item.GridItems, propertyName);
					if (result != null)
					{
						// Make sure the category is expanded to avoid an ArgumentException
						// when trying to select a child grid item.
						if (item.Expandable)
						{
							item.Expanded = true;
						}

						break;
					}
				}
				else if (item.GridItemType == GridItemType.Property && item.PropertyDescriptor.Name == propertyName)
				{
					result = item;
					break;
				}
			}

			return result;
		}

		private void Reset_Click(object sender, EventArgs e)
		{
			this.ResetSelectedProperty();
		}

		private void GridContext_Opening(object sender, CancelEventArgs e)
		{
			bool enabled = false;

			if (this.SelectedObject != null && this.SelectedGridItem != null
				&& this.SelectedGridItem.GridItemType == GridItemType.Property)
			{
				enabled = this.SelectedGridItem.PropertyDescriptor.CanResetValue(this.SelectedObject);
			}

			this.reset.Enabled = enabled;
		}

		#endregion
	}
}
