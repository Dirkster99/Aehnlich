namespace AehnlichDirViewModelLib.ViewModels
{
    using AehnlichDirViewModelLib.Interfaces;
    using AehnlichDirViewModelLib.Models;

    /// <summary>
    /// Implements a factory that constructs, initiates and returns internal objects.
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Gets an initialized application viewmodel.
        /// </summary>
        /// <returns></returns>
        public static IAppViewModel ConstructAppViewModel()
        {
            return new AppViewModel();
        }

        /// <summary>
        /// Gets an initialized application viewmodel.
        /// </summary>
        /// <param name="args">Options to be used in this directory diff.</param>
        /// <returns></returns>
        public static IAppViewModel ConstructAppViewModel(ShowDirDiffArgs args)
        {
            return new AppViewModel(args);
        }
    }
}
