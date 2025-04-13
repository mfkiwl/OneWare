namespace OneWare.Core.Services;

/// <summary>
/// Interface for modules in the application.
/// This extends Prism's IModule to provide better compatibility during the transition.
/// </summary>
public interface IModule : Prism.Modularity.IModule
{
    /// <summary>
    /// Used to register types with the container that will be used by the application.
    /// </summary>
    /// <param name="containerRegistry">The container registry to register types with.</param>
    new void RegisterTypes(IContainerRegistry containerRegistry);

    /// <summary>
    /// Called after the module has been initialized.
    /// </summary>
    /// <param name="containerProvider">The container provider to resolve dependencies from.</param>
    new void OnInitialized(IContainerProvider containerProvider);
}