using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Data.Core.Plugins;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaEdit.Rendering;
using CommunityToolkit.Mvvm.Input;
using OneWare.ApplicationCommands.Services;
using OneWare.CloudIntegration;
using OneWare.Core.ModuleLogic;
using OneWare.Core.Services;
using OneWare.Core.ViewModels.DockViews;
using OneWare.Core.ViewModels.Windows;
using OneWare.Core.Views.Windows;
using OneWare.Debugger;
using OneWare.ErrorList;
using OneWare.Essentials.Commands;
using OneWare.Essentials.Helpers;
using OneWare.Essentials.LanguageService;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using OneWare.FolderProjectSystem;
using OneWare.ImageViewer;
using OneWare.Json;
using OneWare.LibraryExplorer;
using OneWare.Output;
using OneWare.ProjectExplorer;
using OneWare.ProjectSystem.Services;
using OneWare.SearchList;
using OneWare.Settings.ViewModels;
using OneWare.Settings.Views;
using OneWare.Toml;
using TextMateSharp.Grammars;
using IContainer = Autofac.IContainer;
using ModuleLogic = OneWare.Core.ModuleLogic;

namespace OneWare.Core;

public class App : AvaloniaAutofacApplication
{
    protected bool _tempMode = false;

    protected virtual string GetDefaultLayoutName => "Default";

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        base.Initialize();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        //Services
        containerRegistry.RegisterSingleton<IPluginService, PluginService>();
        containerRegistry.RegisterSingleton<IHttpService, HttpService>();
        containerRegistry.RegisterSingleton<IApplicationCommandService, ApplicationCommandService>();
        containerRegistry.RegisterSingleton<IProjectManagerService, ProjectManagerService>();
        containerRegistry.RegisterSingleton<ILanguageManager, LanguageManager>();
        containerRegistry.RegisterSingleton<IApplicationStateService, ApplicationStateService>();
        containerRegistry.RegisterSingleton<IDockService, DockService>();
        containerRegistry.RegisterSingleton<IWindowService, WindowService>();
        containerRegistry.RegisterSingleton<IModuleTracker, ModuleTracker>();
        containerRegistry.RegisterSingleton<BackupService>();
        containerRegistry.RegisterSingleton<IChildProcessService, ChildProcessService>();
        containerRegistry.RegisterSingleton<IFileIconService, FileIconService>();
        containerRegistry.RegisterSingleton<IEnvironmentService, EnvironmentService>();

        //ViewModels - Singletons
        containerRegistry.RegisterSingleton<MainWindowViewModel>();
        containerRegistry.RegisterSingleton<MainDocumentDockViewModel>();

        //ViewModels Transients
        containerRegistry.Register<WelcomeScreenViewModel>();
        containerRegistry.Register<EditViewModel>();
        containerRegistry.Register<ChangelogViewModel>();
        containerRegistry.Register<AboutViewModel>();

        //Windows
        containerRegistry.RegisterSingleton<MainWindow>();
        containerRegistry.RegisterSingleton<MainView>();
    }

    protected override AvaloniaObject CreateShell()
    {
        //Register IDE Settings
        var settingsService = ContainerProvider.Resolve<ISettingsService>();
        var paths = ContainerProvider.Resolve<IPaths>();

        Name = paths.AppName;

        NativeMenu.SetMenu(this, new NativeMenu
        {
            Items =
            {
                new NativeMenuItem
                {
                    Header = $"About {Name}",
                    Command = new RelayCommand(() => ContainerProvider.Resolve<IWindowService>().Show(
                        new AboutView
                        {
                            DataContext = ContainerProvider.Resolve<AboutViewModel>()
                        }))
                },
                new NativeMenuItemSeparator(),
                new NativeMenuItem
                {
                    Header = "Settings",
                    Command = new AsyncRelayCommand(() => ContainerProvider.Resolve<IWindowService>().ShowDialogAsync(
                        new ApplicationSettingsView
                        {
                            DataContext = ContainerProvider.Resolve<ApplicationSettingsViewModel>()
                        }))
                }
            }
        });

        //General
        settingsService.RegisterSettingCategory("General", 0, "Material.ToggleSwitchOutline");

        //Editor settings
        settingsService.RegisterSettingCategory("Editor", 0, "BoxIcons.RegularCode");

        settingsService.RegisterSettingCategory("Tools", 0, "FeatherIcons.Tool");

        settingsService.RegisterSettingCategory("Languages", 0, "FluentIcons.ProofreadLanguageRegular");
        
        settingsService.RegisterSetting("Editor", "Appearance", "Editor_FontFamily", 
            new ComboBoxSetting("Editor Font Family", "JetBrains Mono NL", ["JetBrains Mono NL", "IntelOne Mono", "Consolas", "Comic Sans MS", "Fira Code"]));
        
        settingsService.RegisterSetting("Editor", "Appearance", "Editor_FontSize", 
            new ComboBoxSetting("Font Size", 15,Enumerable.Range(10, 30).Cast<object>()));

        settingsService.RegisterSetting("Editor", "Appearance", "Editor_SyntaxTheme_Dark", 
            new ComboBoxSetting("Editor Theme Dark", ThemeName.DarkPlus,Enum.GetValues<ThemeName>().Cast<object>())
            {
                HoverDescription = "Sets the theme for Syntax Highlighting in Dark Mode"
            });
        
        settingsService.RegisterSetting("Editor", "Appearance", "Editor_SyntaxTheme_Light", 
            new ComboBoxSetting("Editor Theme Light", ThemeName.LightPlus,Enum.GetValues<ThemeName>().Cast<object>())
            {
                HoverDescription = "Sets the theme for Syntax Highlighting in Light Mode"
            });
        
        settingsService.RegisterSetting("Editor", "Formatting", "Editor_UseAutoFormatting",
            new CheckBoxSetting("Use Auto Formatting", true));
        
        settingsService.RegisterSetting("Editor", "Formatting", "Editor_UseAutoBracket", new CheckBoxSetting("Use Auto Bracket", true));

        settingsService.RegisterTitled("Editor", "Folding", "Editor_UseFolding", "Use Folding",
            "Use Folding in Editor", true);

        settingsService.RegisterTitled("Editor", "Backups", BackupService.KeyBackupServiceEnable,
            "Use Automatic Backups", "Use Automatic Backups in case the IDE crashes",
            ApplicationLifetime is IClassicDesktopStyleApplicationLifetime);
        settingsService.RegisterTitledCombo("Editor", "Backups", BackupService.KeyBackupServiceInterval,
            "Auto backup interval (s)",
            "Interval the IDE uses to save files for backup", 30, 5, 10, 15, 30, 60, 120);

        settingsService.RegisterTitled("Editor", "External Changes", "Editor_DetectExternalChanges",
            "Detect external changes", "Detects changes that happen outside of the IDE", true);
        settingsService.RegisterTitled("Editor", "External Changes", "Editor_NotifyExternalChanges",
            "Notify external changes", "Notifies the user when external happen and ask for reload", false);

        //TypeAssistance
        settingsService.RegisterTitled("Editor", "Assistance", "TypeAssistance_EnableHover",
            "Enable Hover Information", "Enable Hover Information", true);
        settingsService.RegisterTitled("Editor", "Assistance", "TypeAssistance_EnableAutoCompletion",
            "Enable Code Suggestions", "Enable completion suggestions", true);
        settingsService.RegisterTitled("Editor", "Assistance", "TypeAssistance_EnableAutoFormatting",
            "Enable Auto Formatting", "Enable automatic formatting", true);

        var windowService = ContainerProvider.Resolve<IWindowService>();
        var commandService = ContainerProvider.Resolve<IApplicationCommandService>();

        windowService.RegisterMenuItem("MainWindow_MainMenu", new MenuItemViewModel("Help")
        {
            Header = "Help",
            Priority = 1000
        });
        windowService.RegisterMenuItem("MainWindow_MainMenu", new MenuItemViewModel("Code")
        {
            Header = "Code",
            Priority = 100
        });
        windowService.RegisterMenuItem("MainWindow_MainMenu/Help", new MenuItemViewModel("Changelog")
        {
            Header = "Changelog",
            IconObservable = Current!.GetResourceObservable("VsImageLib2019.StatusUpdateGrey16X"),
            Command = new RelayCommand(() => windowService.Show(new ChangelogView
            {
                DataContext = ContainerProvider.Resolve<ChangelogViewModel>()
            }))
        });
        windowService.RegisterMenuItem("MainWindow_MainMenu/Help", new MenuItemViewModel("About")
        {
            Header = $"About {paths.AppName}",
            Command = new RelayCommand(() => windowService.Show(new AboutView
            {
                DataContext = ContainerProvider.Resolve<AboutViewModel>()
            }))
        });
        windowService.RegisterMenuItem("MainWindow_MainMenu", new MenuItemViewModel("Extras")
        {
            Header = "Extras",
            Priority = 900
        });
        windowService.RegisterMenuItem("MainWindow_MainMenu/Extras", new MenuItemViewModel("Settings")
        {
            Header = "Settings",
            IconObservable = Current!.GetResourceObservable("Material.SettingsOutline"),
            Command = new AsyncRelayCommand(() => windowService.ShowDialogAsync(new ApplicationSettingsView
            {
                DataContext = ContainerProvider.Resolve<ApplicationSettingsViewModel>()
            }))
        });
        windowService.RegisterMenuItem("MainWindow_MainMenu/Code", new MenuItemViewModel("Format")
        {
            Header = "Format",
            IconObservable = Current!.GetResourceObservable("BoxIcons.RegularCode"),
            Command = new RelayCommand(
                () => (ContainerProvider.Resolve<IDockService>().CurrentDocument as EditViewModel)?.Format(),
                () => ContainerProvider.Resolve<IDockService>().CurrentDocument is EditViewModel),
            InputGesture = new KeyGesture(Key.Enter, KeyModifiers.Control | KeyModifiers.Alt)
        });

        windowService.RegisterMenuItem("MainWindow_MainMenu/File", new MenuItemViewModel("Save")
        {
            Command = new AsyncRelayCommand(
                () => ContainerProvider.Resolve<IDockService>().CurrentDocument!.SaveAsync(),
                () => ContainerProvider.Resolve<IDockService>().CurrentDocument is not null),
            Header = "Save Current",
            InputGesture = new KeyGesture(Key.S, PlatformHelper.ControlKey),
            IconObservable = Current!.GetResourceObservable("VsImageLib.Save16XMd")
        });

        windowService.RegisterMenuItem("MainWindow_MainMenu/File", new MenuItemViewModel("Save All")
        {
            Command = new RelayCommand(
                () =>
                {
                    foreach (var file in ContainerProvider.Resolve<IDockService>().OpenFiles) _ = file.Value.SaveAsync();
                }),
            Header = "Save All",
            InputGesture = new KeyGesture(Key.S, PlatformHelper.ControlKey | KeyModifiers.Shift),
            IconObservable = Current!.GetResourceObservable("VsImageLib.SaveAll16X")
        });

        var applicationCommandService = ContainerProvider.Resolve<IApplicationCommandService>();

        applicationCommandService.RegisterCommand(new SimpleApplicationCommand("Active light theme",
            () =>
            {
                settingsService.SetSettingValue("General_SelectedTheme", "Light");
                settingsService.Save(paths.SettingsPath);
            },
            () => settingsService.GetSettingValue<string>("General_SelectedTheme") != "Light"));
        
        applicationCommandService.RegisterCommand(new SimpleApplicationCommand("Active dark theme",
            () =>
            {
                settingsService.SetSettingValue("General_SelectedTheme", "Dark");
                settingsService.Save(paths.SettingsPath);
            },
            () => settingsService.GetSettingValue<string>("General_SelectedTheme") != "Dark"));
        
        // applicationCommandService.RegisterCommand(new SimpleApplicationCommand("Show Success Notification",
        //     () => Container.Resolve<IWindowService>().ShowNotification("Test", "TestMessage", NotificationType.Success)));
        //
        // applicationCommandService.RegisterCommand(new SimpleApplicationCommand("Show Test Notification",
        //     () => Container.Resolve<IWindowService>().ShowNotificationWithButton("Test", "TestMessage", "Open", () => Console.WriteLine(""), null,  NotificationType.Warning)));
        
        //AvaloniaEdit Hyperlink support
        VisualLineLinkText.OpenUriEvent.AddClassHandler<Window>((window, args) =>
        {
            var link = args.Uri.ToString();
            PlatformHelper.OpenHyperLink(link);
        });

        if (ApplicationLifetime is ISingleViewApplicationLifetime)
        {
            var mainView = ContainerProvider.Resolve<MainView>();
            mainView.DataContext = ContainerProvider.Resolve<MainWindowViewModel>();
            return mainView;
        }

        var mainWindow = ContainerProvider.Resolve<MainWindow>();

        mainWindow.Closing += (o, i) => _ = TryShutDownAsync(o, i);

        return mainWindow;
    }

    protected override IModuleCatalog CreateModuleCatalog()
    {
        return new ModuleLogic.AggregateModuleCatalog();
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        // Add modules
        moduleCatalog.AddModule(typeof(SearchListModule));
        moduleCatalog.AddModule(typeof(ErrorListModule));
        moduleCatalog.AddModule(typeof(OutputModule));
        moduleCatalog.AddModule(typeof(ProjectExplorerModule));
        moduleCatalog.AddModule(typeof(LibraryExplorerModule));
        moduleCatalog.AddModule(typeof(FolderProjectSystemModule));
        moduleCatalog.AddModule(typeof(ImageViewerModule));
        moduleCatalog.AddModule(typeof(JsonModule));
        moduleCatalog.AddModule(typeof(TomlModule));
        moduleCatalog.AddModule(typeof(DebuggerModule));
        moduleCatalog.AddModule(typeof(OneWareCloudIntegrationModule));
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime)
        {
            DisableAvaloniaDataAnnotationValidation();
            
            var mainWindow = ContainerProvider.Resolve<MainWindow>();
            mainWindow.NotificationManager = new WindowNotificationManager(mainWindow)
            {
                Position = NotificationPosition.TopRight,
                Margin = new Thickness(0, 55, 5, 0),
                BorderThickness = new Thickness(0),
                MaxItems = 3
            };
        }

        ContainerProvider.Resolve<IApplicationCommandService>().LoadKeyConfiguration();

        ContainerProvider.Resolve<ISettingsService>().GetSettingObservable<string>("General_SelectedTheme").Subscribe(x =>
        {
            TypeAssistanceIconStore.Instance.Load();
        });

        ContainerProvider.Resolve<ILogger>().Log("Framework initialization complete!", ConsoleColor.Green);
        ContainerProvider.Resolve<BackupService>().LoadAutoSaveFile();
        ContainerProvider.Resolve<IDockService>().LoadLayout(GetDefaultLayoutName);
        ContainerProvider.Resolve<BackupService>().Init();

        ContainerProvider.Resolve<ISettingsService>().GetSettingObservable<string>("Editor_FontFamily").Subscribe(x =>
        {
            if (FontManager.Current.SystemFonts.Contains(x))
            {
                Resources["EditorFont"] = new FontFamily(x);
                return;
            }

            var findFont = this.TryFindResource(x, out var resourceFont);
            if (findFont && resourceFont is FontFamily fFamily) Resources["EditorFont"] = this.FindResource(x);
        });

        ContainerProvider.Resolve<ISettingsService>().GetSettingObservable<int>("Editor_FontSize").Subscribe(x =>
        {
            Resources["EditorFontSize"] = (double)x;
        });

        _ = LoadContentAsync();

        base.OnFrameworkInitializationCompleted();
    }

    protected virtual Task LoadContentAsync()
    {
        return Task.CompletedTask;
    }

    private async Task TryShutDownAsync(object? sender, CancelEventArgs e)
    {
        e.Cancel = true;
        try
        {
            var unsavedFiles = new List<IExtendedDocument>();

            foreach (var tab in ContainerProvider.Resolve<IDockService>().OpenFiles)
                if (tab.Value is { IsDirty: true } evm)
                    unsavedFiles.Add(evm);

            var mainWin = ContainerProvider.Resolve<MainWindow>() as Window;
            if (mainWin == null) throw new NullReferenceException(nameof(mainWin));
            var shutdownReady = await WindowHelper.HandleUnsavedFilesAsync(unsavedFiles, mainWin);

            if (shutdownReady) await ShutdownAsync();
        }
        catch (Exception ex)
        {
            ContainerProvider.Resolve<ILogger>().Error(ex.Message, ex);
        }
    }

    private async Task ShutdownAsync()
    {
        if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime cds)
            foreach (var win in cds.Windows)
                win.Hide();

        //DockService.ProjectFiles.SaveLastProjectData();
        ContainerProvider.Resolve<BackupService>().CleanUp();

        await ContainerProvider.Resolve<LanguageManager>().CleanResourcesAsync();

        if (!_tempMode) await ContainerProvider.Resolve<IProjectExplorerService>().SaveLastProjectsFileAsync();

        //if (LaunchUpdaterOnExit) Global.PackageManagerViewModel.VhdPlusUpdaterModel.LaunchUpdater(); TODO

        ContainerProvider.Resolve<ILogger>()?.Log("Closed!", ConsoleColor.DarkGray);

        //Save active layout
        if (!_tempMode) ContainerProvider.Resolve<IDockService>().SaveLayout();

        //Save settings
        ContainerProvider.Resolve<ISettingsService>().Save(ContainerProvider.Resolve<IPaths>().SettingsPath);

        //Execute ShutdownActions, like starting the updater
        ContainerProvider.Resolve<IApplicationStateService>().ExecuteShutdownActions();

        Environment.Exit(0);
    }
    
    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}