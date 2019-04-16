namespace AehnlichDirViewModelLib.ViewModels
{
    using AehnlichLib.Enums;

    public class DiffFileModeItemViewModel
    {
        #region ctors
        public DiffFileModeItemViewModel(string name,
                                         string description,
                                         DiffDirFileMode modeKey)
        {
            this.Name = name;
            this.Description = description;
            this.ModeKey = modeKey;
        }

        protected DiffFileModeItemViewModel()
        {
        }
        #endregion ctors

        #region properties
        public string Name { get; }

        public string Description { get; }

        public DiffDirFileMode ModeKey { get; }

        public uint Key { get { return (uint)ModeKey; } }
        #endregion properties
    }
}
