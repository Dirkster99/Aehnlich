namespace Aehnlich.ViewModels
{
    internal interface IAehnlichAppViewModel
    {
        /// <summary>
        /// Invoke this method before application shut down
        /// to save all relevant user settings for recovery on appliaction re-start.
        /// </summary>
        void SaveSettings();
    }
}