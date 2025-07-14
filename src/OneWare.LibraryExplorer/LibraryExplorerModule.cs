using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using OneWare.LibraryExplorer.ViewModels;
using OneWare.ProjectExplorer.ViewModels;
using Autofac;
using OneWare.Core.ModuleLogic;


namespace OneWare.LibraryExplorer;

public class LibraryExplorerModule : IAutofacModule
{
    public void RegisterTypes(ContainerBuilder builder)
    {
        builder.RegisterType<LibraryExplorerViewModel>();
    }

    public void OnInitialized(IComponentContext context)
    {
        var dockService = context.Resolve<IDockService>();
        var windowService = context.Resolve<IWindowService>();

        dockService.RegisterLayoutExtension<LibraryExplorerViewModel>(DockShowLocation.Left);
        
        windowService.RegisterMenuItem("MainWindow_MainMenu/View/Tool Windows",
            new MenuItemViewModel("Library Explorer")
            {
                Header = "Library Explorer",
                Command =
                    new RelayCommand(() => dockService.Show(context.Resolve<LibraryExplorerViewModel>())),
                IconObservable = Application.Current!.GetResourceObservable(LibraryExplorerViewModel.IconKey)
            });
    }
}