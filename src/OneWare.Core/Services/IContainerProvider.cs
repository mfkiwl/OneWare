namespace OneWare.Core.Services;

/// <summary>
/// Interface for resolving services from the container
/// Mimics Prism's IContainerProvider
/// </summary>
public interface IContainerProvider
{
    /// <summary>
    /// Resolves a service from the container by the specified <typeparamref name="T" /> type.
    /// </summary>
    /// <typeparam name="T">The type to resolve.</typeparam>
    /// <returns>The resolved service.</returns>
    T Resolve<T>() where T : class;

    /// <summary>
    /// Resolves a service from the container by the specified type.
    /// </summary>
    /// <param name="type">The type to resolve.</param>
    /// <returns>The resolved service.</returns>
    object Resolve(Type type);

    /// <summary>
    /// Resolves a service from the container, with optional parameters.
    /// </summary>
    /// <typeparam name="T">The type to resolve.</typeparam>
    /// <param name="parameters">The parameters to use when resolving the service.</param>
    /// <returns>The resolved service.</returns>
    T Resolve<T>(params (Type Type, object Instance)[] parameters) where T : class;

    /// <summary>
    /// Determines if a given service is registered with the container.
    /// </summary>
    /// <param name="type">The type to check registration for.</param>
    /// <returns><c>true</c> if the type is registered; otherwise, <c>false</c>.</returns>
    bool IsRegistered(Type type);
    
    /// <summary>
    /// Determines if a given service is registered with the container.
    /// </summary>
    /// <typeparam name="T">The type to check registration for.</typeparam>
    /// <returns><c>true</c> if the type is registered; otherwise, <c>false</c>.</returns>
    bool IsRegistered<T>() where T : class;
}