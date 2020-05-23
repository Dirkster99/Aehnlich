namespace AehnlichDirViewModelLib.Interfaces
{
	/// <summary>
	/// Defines an item in a list for usage/binding to an ItemsControl's ItemsSource
	/// (eg. ListBox, ComboBox ...)
	/// </summary>
	public interface IListItemViewModel
	{
		/// <summary>
		/// Gets the name of the item in the list.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a unique integer key of the item in the list.
		/// </summary>
		int Key { get; }
	}
}
