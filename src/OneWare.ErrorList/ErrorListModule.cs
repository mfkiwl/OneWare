using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using OneWare.Core.Services;
using OneWare.ErrorList.ViewModels;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;

// Keep the Prism namespace for backward compatibility
using Prism.Modularity;

namespace OneWare.ErrorList;

// Implement both our new IModule and Prism's IModule for compatibility
public class ErrorListModule : IModule, Prism.Modularity.IModule
{
    public const string KeyErrorListFilterMode = "ErrorList_FilterMode";
    public const string KeyErrorListShowExternalErrors = "ErrorList_ShowExternalErrors";
    public const string KeyErrorListVisibleSource = "ErrorList_VisibleSource";

    // For Autofac constructor injection
    public ErrorListModule()
    {
    }
    
    // For Prism constructor injection (kept for backward compatibility)
    public ErrorListModule(ISettingsService settingsService, IWindowService windowService, IDockService dockService)
    {
        _settingsService = settingsService;
        _windowService = windowService;
        _dockService = dockService;
    }
    
    private ISettingsService _settingsService;
    private IWindowService _windowService;
    private IDockService _dockService;

    // Method for our new Autofac module system
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<ErrorListViewModel>();
        containerRegistry.RegisterSingleton<IErrorService, ErrorListViewModel>();
    }
    
    // Method for our new Autofac module system
    public void OnInitialized(IContainerProvider containerProvider)
    {
        _settingsService = containerProvider.Resolve<ISettingsService>();
        _windowService = containerProvider.Resolve<IWindowService>();
        _dockService = containerProvider.Resolve<IDockService>();
        
        Initialize(containerProvider);
    }
    
    // Method for Prism's module system (kept for backward compatibility)
    public void RegisterTypes(Prism.Ioc.IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterManySingleton<ErrorListViewModel>(typeof(IErrorService),
            typeof(ErrorListViewModel));
    }
    
    // Method for Prism's module system (kept for backward compatibility)
    public void OnInitialized(Prism.Ioc.IContainerProvider containerProvider)
    {
        Initialize(containerProvider);
    }
    
    // Common initialization method for both systems
    private void Initialize(object containerProvider)
    {
        _dockService.RegisterLayoutExtension<IErrorService>(DockShowLocation.Bottom);
        
        _settingsService.Register(KeyErrorListFilterMode, 0);
        _settingsService.RegisterTitled("Experimental", "Errors", KeyErrorListShowExternalErrors,
            "Show external errors", "Sets if errors from files outside of your project should be visible", false);
        _settingsService.Register(KeyErrorListVisibleSource, 0);
        
        var provider = containerProvider as Prism.Ioc.IContainerProvider ?? 
                      new PrismContainerProviderAdapter(containerProvider as IContainerProvider);
        
        _windowService.RegisterMenuItem("MainWindow_MainMenu/View/Tool Windows", new MenuItemViewModel("Problems")
        {
            Header = "Problems",
            Command = new RelayCommand(() => _dockService.Show(provider.Resolve<IErrorService>())),
            IconObservable = Application.Current!.GetResourceObservable(ErrorListViewModel.IconKey)
        });
    }
}