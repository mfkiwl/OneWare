using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using OneWare.ErrorList.ViewModels;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using Autofac;
using OneWare.Core.ModuleLogic;

namespace OneWare.ErrorList;

public class ErrorListModule : IAutofacModule
{
    public const string KeyErrorListFilterMode = "ErrorList_FilterMode";
    public const string KeyErrorListShowExternalErrors = "ErrorList_ShowExternalErrors";
    public const string KeyErrorListVisibleSource = "ErrorList_VisibleSource";

    public void RegisterTypes(ContainerBuilder builder)
    {
        builder.RegisterType<ErrorListViewModel>().As<IErrorService>().As<ErrorListViewModel>().SingleInstance();
    }

    public void OnInitialized(IComponentContext context)
    {
        var dockService = context.Resolve<IDockService>();
        var settingsService = context.Resolve<ISettingsService>();
        var windowService = context.Resolve<IWindowService>();
        
        dockService.RegisterLayoutExtension<IErrorService>(DockShowLocation.Bottom);

        settingsService.Register(KeyErrorListFilterMode, 0);
        settingsService.RegisterTitled("Experimental", "Errors", KeyErrorListShowExternalErrors,
            "Show external errors", "Sets if errors from files outside of your project should be visible", false);
        settingsService.Register(KeyErrorListVisibleSource, 0);

        windowService.RegisterMenuItem("MainWindow_MainMenu/View/Tool Windows", new MenuItemViewModel("Problems")
        {
            Header = "Problems",
            Command = new RelayCommand(() => dockService.Show(context.Resolve<IErrorService>())),
            IconObservable = Application.Current!.GetResourceObservable(ErrorListViewModel.IconKey)
        });
    }
}