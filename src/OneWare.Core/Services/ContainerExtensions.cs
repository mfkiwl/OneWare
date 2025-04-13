using System.Collections.Generic;

namespace OneWare.Core.Services;

/// <summary>
/// Extension methods for container operations
/// </summary>
public static class ContainerExtensions
{
    /// <summary>
    /// Resolves all registered instances of type T from the container
    /// </summary>
    /// <typeparam name="T">The service type to resolve</typeparam>
    /// <param name="container">The container provider</param>
    /// <returns>All resolved instances</returns>
    public static IEnumerable<T> ResolveAll<T>(this IContainerProvider container) where T : class
    {
        if (container is AutofacContainerWrapper autofacContainer)
        {
            // Use Autofac's built-in capability to resolve all registrations
            return autofacContainer.ResolveAll<T>();
        }
        
        // Fallback for other container providers (just return the single instance)
        if (container.IsRegistered<T>())
        {
            yield return container.Resolve<T>();
        }
    }
    
    /// <summary>
    /// Tries to resolve a service from the container
    /// </summary>
    /// <typeparam name="T">The service type to resolve</typeparam>
    /// <param name="container">The container provider</param>
    /// <returns>The resolved service or null if not found</returns>
    public static T TryResolve<T>(this IContainerProvider container) where T : class
    {
        return container.IsRegistered<T>() ? container.Resolve<T>() : null;
    }
}