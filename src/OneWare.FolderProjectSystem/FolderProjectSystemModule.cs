using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using OneWare.FolderProjectSystem.Models;
using Autofac;
using OneWare.Core.ModuleLogic;


namespace OneWare.FolderProjectSystem;

public class FolderProjectSystemModule : IAutofacModule
{
    public void RegisterTypes(ContainerBuilder builder)
    {
    }

    public void OnInitialized(IComponentContext context)
    {
        var manager = context.Resolve<FolderProjectManager>();

        containerProvider
            .Resolve<IProjectManagerService>()
            .RegisterProjectManager(FolderProjectRoot.ProjectType, manager);

        context.Resolve<IWindowService>().RegisterMenuItem("MainWindow_MainMenu/File/Open",
            new MenuItemViewModel("Folder")
            {
                Header = "Folder",
                Command = new RelayCommand(() =>
                    _ = context.Resolve<IProjectExplorerService>().LoadProjectFolderDialogAsync(manager)),
                IconObservable = Application.Current!.GetResourceObservable("VsImageLib.OpenFolder16X")
            });
    }
}