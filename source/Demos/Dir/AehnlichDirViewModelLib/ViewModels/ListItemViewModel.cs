namespace AehnlichDirViewModelLib.ViewModels
{
    public class ListItemViewModel
    {
        #region ctors
        public ListItemViewModel(string name, int key)
            : this()
        {
            this.Name = name;
            this.Key = key;
        }

        protected ListItemViewModel()
        {
        }
        #endregion ctors

        #region properties
        public string Name { get; }

        public int Key { get; }
        #endregion properties
    }
}