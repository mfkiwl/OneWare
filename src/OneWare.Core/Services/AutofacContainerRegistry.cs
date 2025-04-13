using Autofac;
using System;
using System.Collections.Generic;

namespace OneWare.Core.Services;

/// <summary>
/// Autofac implementation of IContainerRegistry
/// </summary>
public class AutofacContainerRegistry : IContainerRegistry
{
    private readonly ContainerBuilder _containerBuilder;

    public AutofacContainerRegistry(ContainerBuilder containerBuilder)
    {
        _containerBuilder = containerBuilder ?? throw new ArgumentNullException(nameof(containerBuilder));
    }

    /// <inheritdoc />
    public IContainerRegistry RegisterInstance<TInterface>(TInterface instance) where TInterface : class
    {
        _containerBuilder.RegisterInstance(instance).As<TInterface>().SingleInstance();
        return this;
    }

    /// <inheritdoc />
    public IContainerRegistry RegisterSingleton<TInterface, TImplementation>() where TInterface : class where TImplementation : class, TInterface
    {
        _containerBuilder.RegisterType<TImplementation>().As<TInterface>().SingleInstance();
        return this;
    }

    /// <inheritdoc />
    public IContainerRegistry RegisterSingleton<T>() where T : class
    {
        _containerBuilder.RegisterType<T>().SingleInstance();
        return this;
    }

    /// <inheritdoc />
    public IContainerRegistry Register<TInterface, TImplementation>() where TInterface : class where TImplementation : class, TInterface
    {
        _containerBuilder.RegisterType<TImplementation>().As<TInterface>().InstancePerDependency();
        return this;
    }

    /// <inheritdoc />
    public IContainerRegistry Register<T>() where T : class
    {
        _containerBuilder.RegisterType<T>().InstancePerDependency();
        return this;
    }

    /// <summary>
    /// Register many services for the same implementation
    /// </summary>
    /// <param name="implementationType">The implementing type</param>
    /// <param name="serviceTypes">The service types to register</param>
    /// <returns>The container registry</returns>
    public IContainerRegistry RegisterMany(Type implementationType, params Type[] serviceTypes)
    {
        var registration = _containerBuilder.RegisterType(implementationType);

        foreach (var serviceType in serviceTypes)
        {
            registration.As(serviceType);
        }

        registration.InstancePerDependency();
        return this;
    }

    /// <summary>
    /// Register many singleton services for the same implementation
    /// </summary>
    /// <param name="implementationType">The implementing type</param>
    /// <param name="serviceTypes">The service types to register</param>
    /// <returns>The container registry</returns>
    public IContainerRegistry RegisterManySingleton(Type implementationType, params Type[] serviceTypes)
    {
        var registration = _containerBuilder.RegisterType(implementationType);

        foreach (var serviceType in serviceTypes)
        {
            registration.As(serviceType);
        }

        registration.SingleInstance();
        return this;
    }
}