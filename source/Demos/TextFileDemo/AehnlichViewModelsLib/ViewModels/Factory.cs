namespace AehnlichViewModelsLib.ViewModels
{
    /// <summary>
    /// Implements a factory that constructs, initiates and returns internal objects.
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Gets an initialized application viewmodel.
        /// </summary>
        /// <param name="fileA"></param>
        /// <param name="fileB"></param>
        /// <returns></returns>
        public static IAppViewModel ConstructAppViewModel(string fileA, string fileB)
        {
            return new AppViewModel(fileA, fileB);
        }
    }
}
