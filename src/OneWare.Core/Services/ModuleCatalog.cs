using Prism.Modularity;

namespace OneWare.Core.Services;

/// <summary>
/// Implementation of IModuleCatalog
/// </summary>
public class ModuleCatalog : IModuleCatalog
{
    private readonly List<Type> _modules = new List<Type>();
    private readonly Dictionary<Type, Type> _prismModuleAdapters = new Dictionary<Type, Type>();

    /// <inheritdoc />
    public void AddModule<T>() where T : IModule, new()
    {
        _modules.Add(typeof(T));
    }

    /// <summary>
    /// Adds a Prism module to the catalog
    /// </summary>
    /// <typeparam name="T">The type of module to add</typeparam>
    public void AddPrismModule<T>() where T : Prism.Modularity.IModule, new()
    {
        var moduleType = typeof(T);
        var adapterType = typeof(PrismModuleAdapter<>).MakeGenericType(moduleType);
        _prismModuleAdapters[moduleType] = adapterType;
        _modules.Add(adapterType);
    }

    /// <inheritdoc />
    public IEnumerable<Type> GetModules()
    {
        return _modules;
    }
}

/// <summary>
/// Generic adapter for Prism modules
/// </summary>
/// <typeparam name="T">Type of Prism module</typeparam>
public class PrismModuleAdapter<T> : IModule where T : Prism.Modularity.IModule, new()
{
    private readonly T _prismModule = new T();

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Prism modules register their types in OnInitialized or RegisterTypes
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        // Create an adapter for the Prism container provider
        var prismContainerProvider = new PrismContainerProviderAdapter(containerProvider);
        
        // Call the Prism module's OnInitialized method
        _prismModule.OnInitialized(prismContainerProvider);
        
        // If the module also implements IRegisterTypes, call RegisterTypes
        if (_prismModule is IRegisterTypes registerTypes)
        {
            registerTypes.RegisterTypes(new PrismContainerRegistryAdapter(containerProvider));
        }
    }
}