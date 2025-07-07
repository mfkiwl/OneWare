using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using OneWare.Output.ViewModels;
using Prism.Ioc;
using Prism.Modularity;

namespace OneWare.Output;

public class OutputModule : IModule
{
    private readonly IDockService _dockService;
    private readonly ISettingsService _settingsService;
    private readonly IWindowService _windowService;

    public OutputModule(ISettingsService settingsService, IDockService dockService, IWindowService windowService)
    {
        _settingsService = settingsService;
        _windowService = windowService;
        _dockService = dockService;
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterManySingleton<OutputViewModel>(typeof(IOutputService),
            typeof(OutputViewModel));
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        _dockService.RegisterLayoutExtension<IOutputService>(DockShowLocation.Bottom);

        _settingsService.Register("Output_Autoscroll", true);

        _windowService.RegisterMenuItem("MainWindow_MainMenu/View/Tool Windows", new MenuItemViewModel("Output")
        {
            Header = "Output",
            Command = new RelayCommand(() => _dockService.Show(containerProvider.Resolve<IOutputService>()))
         //   IconObservable = Application.Current!.GetResourceObservable(OutputViewModel.IconKey)
        });
    }
}