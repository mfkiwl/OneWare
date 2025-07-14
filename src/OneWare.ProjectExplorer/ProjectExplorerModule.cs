using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using OneWare.ProjectExplorer.Services;
using OneWare.ProjectExplorer.ViewModels;
using OneWare.ProjectExplorer.Views;
using Autofac;
using OneWare.Core.ModuleLogic;


namespace OneWare.ProjectExplorer;

public class ProjectExplorerModule : IAutofacModule
{
    public void RegisterTypes(ContainerBuilder builder)
    {
        builder.RegisterType<IFileWatchService, FileWatchService>();
        containerRegistry.RegisterManySingleton<ProjectExplorerViewModel>(typeof(IProjectExplorerService),
            typeof(ProjectExplorerViewModel));
    }

    public void OnInitialized(IComponentContext context)
    {
        if (context.Resolve<IProjectExplorerService>() is not ProjectExplorerViewModel vm) return;

        var dockService = context.Resolve<IDockService>();
        var windowService = context.Resolve<IWindowService>();

        dockService.RegisterLayoutExtension<IProjectExplorerService>(DockShowLocation.Left);

        windowService.RegisterUiExtension("MainWindow_RoundToolBarExtension", new UiExtension(x =>
            new ProjectExplorerMainWindowToolBarExtension
            {
                DataContext = vm
            }));

        windowService.RegisterMenuItem("MainWindow_MainMenu", new MenuItemViewModel("File")
        {
            Priority = -10,
            Header = "File"
        });

        windowService.RegisterMenuItem("MainWindow_MainMenu/File/Open",
            new MenuItemViewModel("File")
            {
                Header = "File",
                Command = new RelayCommand(() => _ = vm.OpenFileDialogAsync()),
                IconObservable = Application.Current!.GetResourceObservable("VsImageLib.NewFileCollection16X")
            });

        windowService.RegisterMenuItem("MainWindow_MainMenu/File/New",
            new MenuItemViewModel("File")
            {
                Header = "File",
                Command = new RelayCommand(() => _ = vm.ImportFileDialogAsync()),
                IconObservable = Application.Current!.GetResourceObservable("VsImageLib.NewFileCollection16X")
            });

        windowService.RegisterMenuItem("MainWindow_MainMenu/View/Tool Windows",
            new MenuItemViewModel("Project Explorer")
            {
                Header = "Project Explorer",
                Command =
                    new RelayCommand(() => dockService.Show(context.Resolve<IProjectExplorerService>())),
                IconObservable = Application.Current!.GetResourceObservable(ProjectExplorerViewModel.IconKey)
            });
    }
}