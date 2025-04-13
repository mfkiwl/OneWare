namespace OneWare.Core.Services;

/// <summary>
/// Interface for container registration that mimics Prism's IContainerRegistry
/// </summary>
public interface IContainerRegistry
{
    /// <summary>
    /// Registers an instance of a given <typeparamref name="TInterface"/> with the container.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to register.</typeparam>
    /// <param name="instance">The instance of the interface to register.</param>
    /// <returns>The current <see cref="IContainerRegistry"/> instance.</returns>
    IContainerRegistry RegisterInstance<TInterface>(TInterface instance) where TInterface : class;

    /// <summary>
    /// Registers a singleton type with the container.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to register for.</typeparam>
    /// <typeparam name="TImplementation">The implementing type.</typeparam>
    /// <returns>The current <see cref="IContainerRegistry"/> instance.</returns>
    IContainerRegistry RegisterSingleton<TInterface, TImplementation>() where TInterface : class where TImplementation : class, TInterface;

    /// <summary>
    /// Registers a singleton type with the container.
    /// </summary>
    /// <typeparam name="T">The type to register.</typeparam>
    /// <returns>The current <see cref="IContainerRegistry"/> instance.</returns>
    IContainerRegistry RegisterSingleton<T>() where T : class;

    /// <summary>
    /// Registers a type with the container.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to register for.</typeparam>
    /// <typeparam name="TImplementation">The implementing type.</typeparam>
    /// <returns>The current <see cref="IContainerRegistry"/> instance.</returns>
    IContainerRegistry Register<TInterface, TImplementation>() where TInterface : class where TImplementation : class, TInterface;

    /// <summary>
    /// Registers a type with the container.
    /// </summary>
    /// <typeparam name="T">The type to register.</typeparam>
    /// <returns>The current <see cref="IContainerRegistry"/> instance.</returns>
    IContainerRegistry Register<T>() where T : class;
}