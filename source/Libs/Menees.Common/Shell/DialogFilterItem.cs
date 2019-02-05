namespace Menees.Shell
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;

	#endregion

	/// <summary>
	/// Represents a single item in a shell Open or Save file dialog filter.
	/// </summary>
	public sealed class DialogFilterItem
	{
		#region Private Data Members

		private static readonly DialogFilterItem AllFilesValue = new DialogFilterItem("All Files", "*");

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance for the specified extension using the shell's pluralized file type as the item name.
		/// </summary>
		/// <param name="extension">The extension to use.  A "*." or "." prefix is optional.</param>
		public DialogFilterItem(string extension)
			: this(extension, true)
		{
		}

		/// <summary>
		/// Creates a new instance for the specified extension using the shell's file type as the item name.
		/// </summary>
		/// <param name="extension">The extension to use.  A "*." or "." prefix is optional.</param>
		/// <param name="pluralize">Whether to pluralize the shell's file type.  This defaults to true.</param>
		public DialogFilterItem(string extension, bool pluralize)
		{
			string mask = GetMask(extension);
			this.Masks = new[] { mask };

			this.ItemName = (ShellUtility.GetFileTypeInfo(mask, false, IconOptions.None, null) ?? string.Empty).Trim();
			if (string.IsNullOrEmpty(this.ItemName))
			{
				throw Exceptions.NewArgumentExceptionFormat("Unable to get the file type for extension {0}.", extension);
			}

			if (pluralize)
			{
				int lastSpaceIndex = this.ItemName.LastIndexOf(' ');

				// Ignore if the last space is at the beginning or end.  Trim() above should have prevented that.
				if (lastSpaceIndex > 0 && (lastSpaceIndex + 1) < this.ItemName.Length)
				{
					string prefix = this.ItemName.Substring(0, lastSpaceIndex);
					string lastWord = this.ItemName.Substring(lastSpaceIndex + 1);

					this.ItemName = prefix + ' ' + TextUtility.MakePlural(lastWord);
				}
			}
		}

		/// <summary>
		/// Creates a new instance using the specified item name and one or more extensions.
		/// </summary>
		/// <param name="itemName">The item name to use.</param>
		/// <param name="firstExtension">The first extension to use.  A "*." or "." prefix is optional.</param>
		/// <param name="otherExtensions">Zero or more other extensions.    A "*." or "." prefix is optional on each.</param>
		public DialogFilterItem(string itemName, string firstExtension, params string[] otherExtensions)
		{
			Conditions.RequireString(itemName, () => itemName);

			this.ItemName = itemName;

			List<string> masks = new List<string>(1 + (otherExtensions != null ? otherExtensions.Length : 0));
			masks.Add(GetMask(firstExtension));
			if (otherExtensions != null)
			{
				foreach (string extension in otherExtensions)
				{
					masks.Add(GetMask(extension));
				}
			}

			this.Masks = masks.AsReadOnly();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets an item that "filters" for all files.
		/// </summary>
		public static DialogFilterItem AllFiles => AllFilesValue;

		/// <summary>
		/// Gets the user-friendly description of the type of files being filtered to.
		/// </summary>
		public string ItemName { get; }

		/// <summary>
		/// Gets the masks being used by this filter.
		/// </summary>
		public ICollection<string> Masks { get; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Joins zero or more filter items together.
		/// </summary>
		/// <param name="items">A collection of filter items to join together.</param>
		/// <returns>A single filter string compatible with the shell's Open and Save dialog filters.</returns>
		public static string Join(params DialogFilterItem[] items)
		{
			string result = string.Join("|", (object[])items);
			return result;
		}

		/// <summary>
		/// Gets a string representation of the filter.
		/// </summary>
		/// <returns>A string like "ItemName (Masks)|Masks".</returns>
		public override string ToString()
		{
			string result = string.Format("{0} ({1})|{1}", this.ItemName, string.Join(";", this.Masks));
			return result;
		}

		#endregion

		#region Private Methods

		private static string GetMask(string extension)
		{
			Conditions.RequireString(extension, () => extension);

			int dotIndex = extension.LastIndexOf('.');

			string result;
			switch (dotIndex)
			{
				case -1:
					result = "*." + extension;
					break;

				case 0:
					result = "*" + extension;
					break;

				default:
					result = extension;
					break;
			}

			return result;
		}

		#endregion
	}
}
