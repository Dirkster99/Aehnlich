namespace AehnlichDirViewModelLib.ViewModels
{
    using AehnlichDirViewModelLib.Interfaces;

    /// <summary>
    /// Models an item in a list for usage/binding to an ItemsControl's ItemsSource
    /// (eg. ListBox, ComboBox ...)
    /// </summary>
    internal class ListItemViewModel : IListItemViewModel
    {
        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public ListItemViewModel(string name, int key)
            : this()
        {
            this.Name = name;
            this.Key = key;
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected ListItemViewModel()
        {
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets the name of the item in the list.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a unique integer key of the item in the list.
        /// </summary>
        public int Key { get; }
        #endregion properties
    }
}