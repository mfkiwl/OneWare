namespace Prism.Ioc;

using OneWare.Core.Services;

/// <summary>
/// Compatibility class to replace Prism's ContainerLocator
/// This forwards all calls to our new AutofacContainerProvider
/// </summary>
public static class ContainerLocator
{
    /// <summary>
    /// Gets the container instance.
    /// </summary>
    public static IContainerProvider Container => new AutofacContainerWrapper(AutofacContainerProvider.Container);

    /// <summary>
    /// Resolves an instance of the requested service from the container.
    /// </summary>
    /// <typeparam name="T">The type of the service to resolve.</typeparam>
    /// <returns>The resolved service instance.</returns>
    public static T Resolve<T>() where T : class
    {
        return AutofacContainerProvider.Resolve<T>();
    }

    /// <summary>
    /// Resolves an instance of the requested service from the container.
    /// </summary>
    /// <param name="type">The type of the service to resolve.</param>
    /// <returns>The resolved service instance.</returns>
    public static object Resolve(Type type)
    {
        return AutofacContainerProvider.Resolve(type);
    }
}