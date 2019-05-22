namespace AehnlichDirViewModelLib.ViewModels
{
    using AehnlichDirViewModelLib.Interfaces;
    using AehnlichLib.Dir;

    /// <summary>
    /// Implements a factory that constructs, initiates and returns internal objects.
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Gets an initialized application viewmodel.
        /// </summary>
        /// <param name="args">Options to be used in this directory diff.</param>
        /// <returns></returns>
        public static IAppViewModel ConstructAppViewModel(DirDiffArgs args)
        {
            return new AppViewModel(args, new FsDataLib.Dir.DirDataSourceFactory());
        }

        /// <summary>
        /// Gets a viewmodel that supports a simple list based selection from a list of modes.
        /// </summary>
        /// <returns></returns>
        public static IFileDiffModeViewModel ConstructFileDiffModes()
        {
            return new FileDiffModeViewModel();
        }
    }
}
