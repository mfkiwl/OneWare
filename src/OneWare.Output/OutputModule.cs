using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using OneWare.Output.ViewModels;
using Autofac;
using OneWare.Core.ModuleLogic;


namespace OneWare.Output;

public class OutputModule : IAutofacModule
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

    public void RegisterTypes(ContainerBuilder builder)
    {
        containerRegistry.RegisterManySingleton<OutputViewModel>(typeof(IOutputService),
            typeof(OutputViewModel));
    }

    public void OnInitialized(IComponentContext context)
    {
        _dockService.RegisterLayoutExtension<IOutputService>(DockShowLocation.Bottom);

        _settingsService.Register("Output_Autoscroll", true);

        _windowService.RegisterMenuItem("MainWindow_MainMenu/View/Tool Windows", new MenuItemViewModel("Output")
        {
            Header = "Output",
            Command = new RelayCommand(() => _dockService.Show(context.Resolve<IOutputService>())),
            IconObservable = Application.Current!.GetResourceObservable(OutputViewModel.IconKey)
        });
    }
}