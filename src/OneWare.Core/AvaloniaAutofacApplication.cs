using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using OneWare.Core.Services;
using System.Collections.Generic;

namespace OneWare.Core;

/// <summary>
/// Base class for Avalonia applications that use Autofac for dependency injection
/// This replaces Prism's PrismApplication
/// </summary>
public abstract class AvaloniaAutofacApplication : Application
{
    private readonly Dictionary<System.Type, ModuleInfo> _moduleInfos = new Dictionary<System.Type, ModuleInfo>();
    
    /// <summary>
    /// Container used by the application
    /// </summary>
    protected IContainer Container { get; private set; }
    
    /// <summary>
    /// Container provider interface for modules to use
    /// </summary>
    protected IContainerProvider ContainerProvider { get; private set; }

    /// <summary>
    /// Gets the module catalog used to configure the application's modules
    /// </summary>
    protected IModuleCatalog ModuleCatalog { get; private set; }

    /// <summary>
    /// Constructor for the AvaloniaAutofacApplication
    /// </summary>
    protected AvaloniaAutofacApplication()
    {
        ModuleCatalog = CreateModuleCatalog();
    }

    /// <summary>
    /// Initializes the application and the component container
    /// </summary>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
        var containerBuilder = new ContainerBuilder();
        
        // Create container registry for registering dependencies
        var containerRegistry = new AutofacContainerRegistry(containerBuilder);
        
        // Register module catalog
        containerRegistry.RegisterInstance(ModuleCatalog);
        
        // Register application types
        RegisterTypes(containerRegistry);
        
        // Configure module catalog
        ConfigureModuleCatalog(ModuleCatalog);
        
        // Register module types and collect their registration logic
        RegisterModules(containerBuilder);
        
        // Build container
        Container = containerBuilder.Build();
        ContainerProvider = new AutofacContainerWrapper(Container);
        
        // Initialize AutofacContainerProvider for static access
        AutofacContainerProvider.Initialize(Container);
        
        // Initialize modules
        InitializeModules();
        
        // Create and initialize the shell
        var shell = CreateShell();
        if (shell != null)
        {
            InitializeShell(shell);
        }
    }

    /// <summary>
    /// Creates the module catalog used to configure the application's modules
    /// </summary>
    /// <returns>The module catalog</returns>
    protected virtual IModuleCatalog CreateModuleCatalog()
    {
        return new ModuleCatalog();
    }

    /// <summary>
    /// Used to register types with the container that will be used by your application
    /// </summary>
    /// <param name="containerRegistry">Container registry for registering types</param>
    protected abstract void RegisterTypes(IContainerRegistry containerRegistry);

    /// <summary>
    /// Configures the <see cref="IModuleCatalog"/> used by the application
    /// </summary>
    /// <param name="moduleCatalog">The module catalog to configure</param>
    protected virtual void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
    }

    /// <summary>
    /// Creates the shell or main window of the application
    /// </summary>
    /// <returns>The shell of the application</returns>
    protected abstract AvaloniaObject CreateShell();
    
    /// <summary>
    /// Initializes the shell
    /// </summary>
    /// <param name="shell">The shell to initialize</param>
    protected virtual void InitializeShell(AvaloniaObject shell)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && 
            shell is Window window)
        {
            desktop.MainWindow = window;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime single && 
                 shell is Control control)
        {
            single.MainView = control;
        }
    }
    
    private void RegisterModules(ContainerBuilder containerBuilder)
    {
        // Register all modules
        foreach (var moduleType in ModuleCatalog.GetModules())
        {
            if (!typeof(IModule).IsAssignableFrom(moduleType))
            {
                throw new InvalidOperationException($"Type {moduleType.Name} does not implement IModule");
            }
            
            // Create module instance
            var moduleInfo = new ModuleInfo(moduleType);
            _moduleInfos[moduleType] = moduleInfo;
            
            // Register the module itself
            containerBuilder.RegisterType(moduleType).AsSelf().SingleInstance();
            
            // For Autofac modules, we need to collect their registration logic
            if (moduleType.IsClass && !moduleType.IsAbstract)
            {
                if (Activator.CreateInstance(moduleType) is IModule module)
                {
                    // Create a scope builder to collect registrations
                    var moduleBuilder = new ContainerBuilder();
                    var moduleRegistry = new AutofacContainerRegistry(moduleBuilder);
                    
                    // Let module register its types
                    module.RegisterTypes(moduleRegistry);
                    
                    // Store the module builder for later use
                    moduleInfo.ContainerBuilder = moduleBuilder;
                }
            }
        }
        
        // Apply all module registrations after all modules have been processed
        foreach (var moduleInfo in _moduleInfos.Values)
        {
            if (moduleInfo.ContainerBuilder != null)
            {
                // Copy all registrations from the module builder to the main builder
                moduleInfo.ContainerBuilder.Update(containerBuilder);
            }
        }
    }
    
    private void InitializeModules()
    {
        // Initialize all modules
        foreach (var moduleType in ModuleCatalog.GetModules())
        {
            // Resolve the module
            var module = (IModule)Container.Resolve(moduleType);
            
            // Initialize the module
            module.OnInitialized(ContainerProvider);
        }
    }
    
    /// <summary>
    /// Helper class to store information about modules during registration
    /// </summary>
    private class ModuleInfo
    {
        public ModuleInfo(Type moduleType)
        {
            ModuleType = moduleType;
        }
        
        public Type ModuleType { get; }
        
        public ContainerBuilder ContainerBuilder { get; set; }
    }
}