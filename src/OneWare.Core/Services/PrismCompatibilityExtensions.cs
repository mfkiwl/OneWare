using System;
using System.Collections.Generic;

namespace OneWare.Core.Services;

/// <summary>
/// Extension methods to provide compatibility with Prism's container registry methods
/// </summary>
public static class PrismCompatibilityExtensions
{
    /// <summary>
    /// Registers multiple interfaces for the same implementation
    /// </summary>
    /// <param name="containerRegistry">The container registry</param>
    /// <param name="implementationType">The implementing type</param>
    /// <param name="serviceTypes">The service types to register</param>
    /// <returns>The container registry</returns>
    public static IContainerRegistry RegisterMany(this IContainerRegistry containerRegistry, Type implementationType, params Type[] serviceTypes)
    {
        if (containerRegistry == null) throw new ArgumentNullException(nameof(containerRegistry));
        if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));
        
        // Register each service type 
        foreach (var serviceType in serviceTypes)
        {
            RegisterService(containerRegistry, serviceType, implementationType);
        }
        
        return containerRegistry;
    }
    
    /// <summary>
    /// Registers multiple interfaces for the same implementation as singletons
    /// </summary>
    /// <param name="containerRegistry">The container registry</param>
    /// <param name="implementationType">The implementing type</param>
    /// <param name="serviceTypes">The service types to register</param>
    /// <returns>The container registry</returns>
    public static IContainerRegistry RegisterManySingleton(this IContainerRegistry containerRegistry, Type implementationType, params Type[] serviceTypes)
    {
        if (containerRegistry == null) throw new ArgumentNullException(nameof(containerRegistry));
        if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));
        
        // Register the implementation as a singleton
        var registration = typeof(IContainerRegistry).GetMethod("RegisterSingleton", new[] { typeof(Type), typeof(Type) });
        
        // Register each service type as a singleton
        foreach (var serviceType in serviceTypes)
        {
            if (registration != null)
            {
                registration.MakeGenericMethod(serviceType, implementationType).Invoke(containerRegistry, null);
            }
        }
        
        return containerRegistry;
    }
    
    /// <summary>
    /// Registers many interfaces for the same implementation
    /// </summary>
    /// <typeparam name="T">The implementing type</typeparam>
    /// <param name="containerRegistry">The container registry</param>
    /// <param name="serviceTypes">The service types to register</param>
    /// <returns>The container registry</returns>
    public static IContainerRegistry RegisterMany<T>(this IContainerRegistry containerRegistry, params Type[] serviceTypes) where T : class
    {
        return RegisterMany(containerRegistry, typeof(T), serviceTypes);
    }
    
    /// <summary>
    /// Registers many interfaces for the same implementation as singletons
    /// </summary>
    /// <typeparam name="T">The implementing type</typeparam>
    /// <param name="containerRegistry">The container registry</param>
    /// <param name="serviceTypes">The service types to register</param>
    /// <returns>The container registry</returns>
    public static IContainerRegistry RegisterManySingleton<T>(this IContainerRegistry containerRegistry, params Type[] serviceTypes) where T : class
    {
        return RegisterManySingleton(containerRegistry, typeof(T), serviceTypes);
    }
    
    private static void RegisterService(IContainerRegistry containerRegistry, Type serviceType, Type implementationType)
    {
        // Use reflection to call the generic Register method
        var registerMethod = typeof(IContainerRegistry).GetMethod("Register", new[] { typeof(Type), typeof(Type) });
        
        if (registerMethod != null)
        {
            // Create a generic method with the specific service and implementation types
            var genericMethod = registerMethod.MakeGenericMethod(serviceType, implementationType);
            
            // Invoke the method on the containerRegistry
            genericMethod.Invoke(containerRegistry, null);
        }
    }
}