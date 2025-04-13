using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Media;
using ImTools;
using OneWare.Core.Data;
using OneWare.Core.Services;
using OneWare.Core.Views.Windows;
using OneWare.Demo.Desktop.ViewModels;
using OneWare.Demo.Desktop.Views;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Services;
using OneWare.PackageManager;
using OneWare.SourceControl;
using OneWare.TerminalManager;

namespace OneWare.Demo.Desktop;

public class DesktopDemoApp : DemoApp
{
    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        base.ConfigureModuleCatalog(moduleCatalog);

        moduleCatalog.AddPrismModule<PackageManagerModule>();
        moduleCatalog.AddPrismModule<TerminalManagerModule>();
        moduleCatalog.AddPrismModule<SourceControlModule>();

        try
        {
            var plugins = Directory.GetDirectories(Paths.PluginsDirectory);
            foreach (var module in plugins) ContainerProvider.Resolve<IPluginService>().AddPlugin(module);
        }
        catch (Exception e)
        {
            ContainerProvider.Resolve<ILogger>().Error(e.Message, e);
        }

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
    }

    protected override async Task LoadContentAsync()
    {
        var arguments = Environment.GetCommandLineArgs();

        Window? splashWindow = null;
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime)
        {
            splashWindow = new SplashWindow
            {
                DataContext = ContainerProvider.Resolve<SplashWindowViewModel>()
            };
            splashWindow.Show();
        }

        if (arguments.Length > 1 && !arguments[1].StartsWith("--"))
        {
            var fileName = arguments[1];
            //Check file exists
            if (File.Exists(fileName))
            {
                if (Path.GetExtension(fileName).StartsWith(".", StringComparison.OrdinalIgnoreCase))
                {
                    var file = ContainerProvider.Resolve<IProjectExplorerService>().GetTemporaryFile(fileName);
                    _ = ContainerProvider.Resolve<IDockService>().OpenFileAsync(file);
                }
                else
                {
                    ContainerProvider.Resolve<ILogger>()?.Log("Could not load file " + fileName);
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
            }
        }

        await Task.Delay(1000);
        splashWindow?.Close();

        try
        {
            var settingsService = ContainerProvider.Resolve<ISettingsService>();
            ContainerProvider.Resolve<ILogger>()?.Log("Loading last projects finished!", ConsoleColor.Cyan);

            if (settingsService.GetSettingValue<string>("LastVersion") != Global.VersionCode)
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
        }
        catch (Exception e)
        {
            ContainerProvider.Resolve<ILogger>().Error(e.Message, e);
        }
    }
}