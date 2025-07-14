using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using OneWare.TerminalManager.ViewModels;
using Autofac;
using OneWare.Core.ModuleLogic;


namespace OneWare.TerminalManager;

public class TerminalManagerModule : IAutofacModule
{
    public void RegisterTypes(ContainerBuilder builder)
    {
        builder.RegisterType<TerminalManagerViewModel>();
    }

    public void OnInitialized(IComponentContext context)
    {
        context.Resolve<IDockService>()
            .RegisterLayoutExtension<TerminalManagerViewModel>(DockShowLocation.Bottom);
        context.Resolve<IWindowService>().RegisterMenuItem("MainWindow_MainMenu/View/Tool Windows",
            new MenuItemViewModel("Terminal")
            {
                Header = "Terminal",
                Command = new RelayCommand(() =>
                    context.Resolve<IDockService>()
                        .Show(context.Resolve<TerminalManagerViewModel>())),
                IconObservable = Application.Current!.GetResourceObservable(TerminalManagerViewModel.IconKey)
            });
    }
}