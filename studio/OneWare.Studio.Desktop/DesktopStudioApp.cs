using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Media;
using Avalonia.Threading;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using ImTools;
using OneWare.Core.Data;
using OneWare.Core.Services;
using OneWare.Core.ViewModels.Windows;
using OneWare.Core.Views.Windows;
using OneWare.Cpp;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Services;
using OneWare.OssCadSuiteIntegration;
using OneWare.PackageManager;
using OneWare.PackageManager.ViewModels;
using OneWare.PackageManager.Views;
using OneWare.SerialMonitor;
using OneWare.SourceControl;
using OneWare.Studio.Desktop.ViewModels;
using OneWare.Studio.Desktop.Views;
using OneWare.TerminalManager;
using OneWare.Updater;
using OneWare.Updater.ViewModels;
using OneWare.Updater.Views;
using OneWare.Verilog;
using OneWare.Vhdl;

namespace OneWare.Studio.Desktop;

public class DesktopStudioApp : StudioApp
{
    private Window? _splashWindow;

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        base.ConfigureModuleCatalog(moduleCatalog);
        moduleCatalog.AddPrismModule<UpdaterModule>();
        moduleCatalog.AddPrismModule<PackageManagerModule>();
        moduleCatalog.AddPrismModule<TerminalManagerModule>();
        moduleCatalog.AddPrismModule<SourceControlModule>();
        moduleCatalog.AddPrismModule<SerialMonitorModule>();
        moduleCatalog.AddPrismModule<CppModule>();
        moduleCatalog.AddPrismModule<VhdlModule>();
        moduleCatalog.AddPrismModule<VerilogModule>();
        moduleCatalog.AddPrismModule<OssCadSuiteIntegrationModule>();
        try
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length > 1)
            {
                var m = commandLineArgs.IndexOf(x => x == "--modules");
                if (m >= 0 && m < commandLineArgs.Length - 1)
                {
                    var path = commandLineArgs[m + 1];
                    ContainerProvider.Resolve<IPluginService>().AddPlugin(path);
                }
            }
            var plugins = Directory.GetDirectories(Paths.PluginsDirectory);
            foreach (var module in plugins) ContainerProvider.Resolve<IPluginService>().AddPlugin(module);
        }
        catch (Exception e)
        {
            ContainerProvider.Resolve<ILogger>().Error(e.Message, e);
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime &&
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _splashWindow = new SplashWindow
            {
                DataContext = ContainerProvider.Resolve<SplashWindowViewModel>()
            };
            _splashWindow.Show();
            _splashWindow.Activate();
        }
        base.OnFrameworkInitializationCompleted();
    }

    protected override async Task LoadContentAsync()
    {
        ContainerProvider.Resolve<IPackageService>().RegisterPackageRepository(
            $"https://raw.githubusercontent.com/one-ware/OneWare.PublicPackages/main/oneware-packages.json");
        var arguments = Environment.GetCommandLineArgs();
        if (arguments.Length > 1 && !arguments[1].StartsWith("--"))
        {
            var fileName = arguments[1];
            //Check file exists
            if (File.Exists(fileName))
            {
                _tempMode = true;
                var dockService = ContainerProvider.Resolve<IDockService>();
                var views = dockService.SearchView<Document>();
                foreach (var view in views.ToArray())
                    if (view is IDockable dockable)
                        dockService.CloseDockable(dockable);
                var extension = Path.GetExtension(fileName);
                var manager = ContainerProvider.Resolve<IProjectManagerService>().GetManagerByExtension(extension);
                if (manager != null)
                {
                    await ContainerProvider.Resolve<IProjectExplorerService>().LoadProjectAsync(fileName, manager);
                }
                else if (extension.StartsWith(".", StringComparison.OrdinalIgnoreCase))
                {
                    var file = ContainerProvider.Resolve<IProjectExplorerService>().GetTemporaryFile(fileName);
                    _ = ContainerProvider.Resolve<IDockService>().OpenFileAsync(file);
                }
                else
                {
                    ContainerProvider.Resolve<ILogger>()?.Warning("Could not load file/directory " + fileName);
                }
            }
        }
        else
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime)
            {
                var key = ContainerProvider.Resolve<IApplicationStateService>()
                    .AddState("Loading last projects...", AppState.Loading);
                await ContainerProvider.Resolve<IProjectExplorerService>().OpenLastProjectsFileAsync();
                ContainerProvider.Resolve<IDockService>().InitializeContent();
                ContainerProvider.Resolve<IApplicationStateService>().RemoveState(key, "Projects loaded!");
                ContainerProvider.Resolve<ILogger>()?.Log("Loading last projects finished!", ConsoleColor.Cyan);
            }
        }
        await Task.Delay(1000);
        _splashWindow?.Close();
        try
        {
            var settingsService = ContainerProvider.Resolve<ISettingsService>();
            if (Version.TryParse(settingsService.GetSettingValue<string>("LastVersion"), out var lastVersion) &&
                lastVersion < Assembly.GetExecutingAssembly().GetName().Version)
            {
                settingsService.SetSettingValue("LastVersion", Global.VersionCode);
                ContainerProvider.Resolve<IWindowService>().ShowNotificationWithButton("Update Successful!",
                    $"{ContainerProvider.Resolve<IPaths>().AppName} got updated to {Global.VersionCode}!", "View Changelog",
                    () =>
                    {
                        ContainerProvider.Resolve<IWindowService>().Show(new ChangelogView
                        {
                            DataContext = ContainerProvider.Resolve<ChangelogViewModel>()
                        });
                    },
                    Current?.FindResource("VsImageLib2019.StatusUpdateGrey16X") as IImage);
            }
            var packageService = ContainerProvider.Resolve<IPackageService>();
            await packageService.LoadPackagesAsync();
            var updatePackages = packageService.Packages
                .Where(x => x.Value.Status == PackageStatus.UpdateAvailable)
                .Select(x => x.Value)
                .ToList();
            if (updatePackages.Count > 0)
                ContainerProvider.Resolve<IWindowService>().ShowNotificationWithButton("Package Updates Available",
                    $"Updates for {string.Join(", ", updatePackages.Select(x => x.Package.Name))} available!",
                    "Download", () => ContainerProvider.Resolve<IWindowService>().Show(new PackageManagerView
                    {
                        DataContext = ContainerProvider.Resolve<PackageManagerViewModel>()
                    }),
                    Current?.FindResource("VsImageLib2019.StatusUpdateGrey16X") as IImage);
            var ideUpdater = ContainerProvider.Resolve<UpdaterViewModel>();
            var canUpdate = await ideUpdater.CheckForUpdateAsync();
            if (canUpdate)
                Dispatcher.UIThread.Post(() =>
                {
                    ContainerProvider.Resolve<IWindowService>().ShowNotificationWithButton("Update Available",
                        $"{Paths.AppName} {ideUpdater.NewVersion} is available!", "Download", () => ContainerProvider
                            .Resolve<IWindowService>().Show(new UpdaterView
                            {
                                DataContext = ContainerProvider.Resolve<UpdaterViewModel>()
                            }),
                        Current?.FindResource("VsImageLib2019.StatusUpdateGrey16X") as IImage);
                });
        }
        catch (Exception e)
        {
            ContainerProvider.Resolve<ILogger>().Error(e.Message, e);
        }
    }
}