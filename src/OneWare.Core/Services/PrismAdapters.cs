using Prism.Ioc;
using IContainerProvider = OneWare.Core.Services.IContainerProvider;
using IContainerRegistry = OneWare.Core.Services.IContainerRegistry;

namespace OneWare.Core.Services;

/// <summary>
/// Adapter that converts our container provider to Prism's container provider
/// </summary>
public class PrismContainerProviderAdapter : Prism.Ioc.IContainerProvider
{
    private readonly IContainerProvider _containerProvider;

    public PrismContainerProviderAdapter(IContainerProvider containerProvider)
    {
        _containerProvider = containerProvider;
    }

    public object Resolve(Type type)
    {
        return _containerProvider.Resolve(type);
    }

    public object Resolve(Type type, string name)
    {
        // Named resolution is not supported directly in our simplified container
        // Fallback to type resolution
        return _containerProvider.Resolve(type);
    }

    public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
    {
        if (type == typeof(Prism.Ioc.IContainerProvider))
        {
            return this;
        }
        
        return _containerProvider.Resolve(type);
    }
}

/// <summary>
/// Adapter that converts our container registry to Prism's container registry
/// </summary>
public class PrismContainerRegistryAdapter : Prism.Ioc.IContainerRegistry
{
    private readonly IContainerProvider _containerProvider;

    public PrismContainerRegistryAdapter(IContainerProvider containerProvider)
    {
        _containerProvider = containerProvider;
    }

    public Prism.Ioc.IContainerRegistry RegisterInstance(Type type, object instance)
    {
        // This is a no-op in our adapter as we can't modify a built container
        return this;
    }

    public Prism.Ioc.IContainerRegistry RegisterInstance(Type type, object instance, string name)
    {
        // This is a no-op in our adapter as we can't modify a built container
        return this;
    }

    public Prism.Ioc.IContainerRegistry RegisterSingleton(Type from, Type to)
    {
        // This is a no-op in our adapter as we can't modify a built container
        return this;
    }

    public Prism.Ioc.IContainerRegistry RegisterSingleton(Type from, Type to, string name)
    {
        // This is a no-op in our adapter as we can't modify a built container
        return this;
    }

    public Prism.Ioc.IContainerRegistry Register(Type from, Type to)
    {
        // This is a no-op in our adapter as we can't modify a built container
        return this;
    }

    public Prism.Ioc.IContainerRegistry Register(Type from, Type to, string name)
    {
        // This is a no-op in our adapter as we can't modify a built container
        return this;
    }

    public Prism.Ioc.IContainerRegistry RegisterMany(Type implementingType, params Type[] serviceTypes)
    {
        // This is a no-op in our adapter as we can't modify a built container
        return this;
    }

    public Prism.Ioc.IContainerRegistry RegisterManySingleton(Type implementingType, params Type[] serviceTypes)
    {
        // This is a no-op in our adapter as we can't modify a built container
        return this;
    }

    public Prism.Ioc.IContainerRegistry RegisterDelegate(Type serviceType, Func<object> factoryMethod)
    {
        // This is a no-op in our adapter as we can't modify a built container
        return this;
    }

    public Prism.Ioc.IContainerRegistry RegisterDelegate(Type serviceType, Func<Prism.Ioc.IContainerProvider, object> factoryMethod)
    {
        // This is a no-op in our adapter as we can't modify a built container
        return this;
    }

    public Prism.Ioc.IContainerRegistry RegisterDelegate(Type serviceType, Func<object> factoryMethod, string name)
    {
        // This is a no-op in our adapter as we can't modify a built container
        return this;
    }

    public Prism.Ioc.IContainerRegistry RegisterDelegate(Type serviceType, Func<Prism.Ioc.IContainerProvider, object> factoryMethod, string name)
    {
        // This is a no-op in our adapter as we can't modify a built container
        return this;
    }

    public bool IsRegistered(Type type)
    {
        return _containerProvider.IsRegistered(type);
    }

    public bool IsRegistered(Type type, string name)
    {
        // Named registration is not directly supported
        return _containerProvider.IsRegistered(type);
    }
}