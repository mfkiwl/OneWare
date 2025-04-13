using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using OneWare.Essentials.Services;

namespace OneWare.Core.Services;

/// <summary>
/// Provides a static access point to the Autofac container instance
/// This class replaces Prism's ContainerLocator
/// </summary>
public static class AutofacContainerProvider
{
    private static IContainer? _container;

    /// <summary>
    /// Gets the current container instance.
    /// </summary>
    public static IContainer Container
    {
        get
        {
            if (_container == null)
            {
                throw new InvalidOperationException("The container has not been initialized.");
            }

            return _container;
        }
    }

    /// <summary>
    /// Initializes the container with the provided instance.
    /// </summary>
    /// <param name="container">The container instance.</param>
    public static void Initialize(IContainer container)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
    }

    /// <summary>
    /// Resolves an instance of the requested service from the container.
    /// </summary>
    /// <typeparam name="T">The type of the service to resolve.</typeparam>
    /// <returns>The resolved service instance.</returns>
    public static T Resolve<T>() where T : class
    {
        return Container.Resolve<T>();
    }

    /// <summary>
    /// Resolves an instance of the requested service from the container.
    /// </summary>
    /// <param name="type">The type of the service to resolve.</param>
    /// <returns>The resolved service instance.</returns>
    public static object Resolve(Type type)
    {
        return Container.Resolve(type);
    }

    /// <summary>
    /// Determines whether the specified service type is registered.
    /// </summary>
    /// <typeparam name="T">The type to check registration for.</typeparam>
    /// <returns><c>true</c> if the type is registered; otherwise <c>false</c>.</returns>
    public static bool IsRegistered<T>() where T : class
    {
        return Container.IsRegistered<T>();
    }

    /// <summary>
    /// Determines whether the specified service type is registered.
    /// </summary>
    /// <param name="type">The type to check registration for.</param>
    /// <returns><c>true</c> if the type is registered; otherwise <c>false</c>.</returns>
    public static bool IsRegistered(Type type)
    {
        return Container.IsRegistered(type);
    }
}

/// <summary>
/// Autofac implementation of IContainerProvider
/// </summary>
public class AutofacContainerWrapper : IContainerProvider
{
    private readonly IContainer _container;

    public AutofacContainerWrapper(IContainer container)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
    }

    /// <inheritdoc />
    public T Resolve<T>() where T : class
    {
        return _container.Resolve<T>();
    }

    /// <inheritdoc />
    public object Resolve(Type type)
    {
        return _container.Resolve(type);
    }

    /// <inheritdoc />
    public T Resolve<T>(params (Type Type, object Instance)[] parameters) where T : class
    {
        if (parameters == null || parameters.Length == 0)
        {
            return _container.Resolve<T>();
        }

        var typedParameters = parameters.Select(p => new TypedParameter(p.Type, p.Instance)).ToArray();
        return _container.Resolve<T>(typedParameters);
    }

    /// <summary>
    /// Resolves all registered instances of the specified type
    /// </summary>
    /// <typeparam name="T">The type to resolve</typeparam>
    /// <returns>All registered instances of the specified type</returns>
    public IEnumerable<T> ResolveAll<T>() where T : class
    {
        return _container.Resolve<IEnumerable<T>>();
    }

    /// <inheritdoc />
    public bool IsRegistered(Type type)
    {
        return _container.IsRegistered(type);
    }

    /// <inheritdoc />
    public bool IsRegistered<T>() where T : class
    {
        return _container.IsRegistered<T>();
    }
}