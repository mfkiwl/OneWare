using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using OneWare.Debugger.ViewModels;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using Autofac;
using OneWare.Core.ModuleLogic;


namespace OneWare.Debugger;

public class DebuggerModule : IAutofacModule
{
    public void RegisterTypes(ContainerBuilder builder)
    {
    }

    public void OnInitialized(IComponentContext context)
    {
        var dockService = context.Resolve<IDockService>();
        //dockService.RegisterLayoutExtension<DebuggerViewModel>(DockShowLocation.Bottom);

        context.Resolve<IWindowService>().RegisterMenuItem("MainWindow_MainMenu/View/Tool Windows",
            new MenuItemViewModel("Debugger")
            {
                Header = "Debugger",
                Command = new RelayCommand(() =>
                    dockService.Show(context.Resolve<DebuggerViewModel>(), DockShowLocation.Bottom)),
                IconObservable = Application.Current!.GetResourceObservable(DebuggerViewModel.IconKey)
            });
    }
}