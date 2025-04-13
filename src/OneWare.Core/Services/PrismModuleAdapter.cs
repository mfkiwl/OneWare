using Prism.Ioc;
using Prism.Modularity;

namespace OneWare.Core.Services;

/// <summary>
/// Adapter that converts Prism modules to our custom module interface
/// This allows existing Prism modules to work with our new DI system without changes
/// </summary>
public class PrismModuleAdapter<T> : IModule where T : Prism.Modularity.IModule, new()
{
    private readonly T _prismModule;

    public PrismModuleAdapter()
    {
        _prismModule = new T();
    }
    
    // Implementation for Prism's IModule interface
    void Prism.Modularity.IModule.OnInitialized(Prism.Ioc.IContainerProvider containerProvider)
    {
        _prismModule.OnInitialized(containerProvider);
    }

    // Implementation for Prism's IRegisterTypes interface if supported
    void IRegisterTypes.RegisterTypes(Prism.Ioc.IContainerRegistry containerRegistry)
    {
        if (_prismModule is IRegisterTypes registerTypes)
        {
            registerTypes.RegisterTypes(containerRegistry);
        }
    }

    // Implementation for our custom IModule interface
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // We will register types in OnInitialized because the Prism module might register them there
    }

    // Implementation for our custom IModule interface
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