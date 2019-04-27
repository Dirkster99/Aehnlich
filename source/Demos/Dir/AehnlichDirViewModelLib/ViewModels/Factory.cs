namespace AehnlichDirViewModelLib.ViewModels
{
    using AehnlichDirViewModelLib.Interfaces;

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
    }
}
