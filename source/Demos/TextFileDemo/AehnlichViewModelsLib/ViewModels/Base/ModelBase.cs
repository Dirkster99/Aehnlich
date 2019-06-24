namespace AehnlichViewModelsLib.ViewModels.Base
{
    /// <summary>
    /// Models a base objects that can access a service container.
    /// </summary>
    public class ModelBase
    {
        /// <summary>
        /// Gets an instance of the service container and retrieves the requested service coponent.
        /// </summary>
        /// <typeparam name="TServiceContract"></typeparam>
        /// <returns></returns>
        public TServiceContract GetService<TServiceContract>() where TServiceContract : class
        {
            return ServiceLocator.ServiceContainer.Instance.GetService<TServiceContract>();
        }
    }
}
