using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using OneWare.SourceControl.Settings;
using OneWare.SourceControl.ViewModels;
using OneWare.SourceControl.Views;
using Autofac;
using OneWare.Core.ModuleLogic;

namespace OneWare.SourceControl;

public class SourceControlModule : IAutofacModule
{
    public const string GitHubAccountNameKey = "SourceControl_GitHub_AccountName";
    
    public void RegisterTypes(ContainerBuilder builder)
    {
        builder.RegisterType<CompareFileViewModel>().AsSelf();
        builder.RegisterType<SourceControlViewModel>().AsSelf().SingleInstance();
        builder.RegisterType<GitHubAccountSettingViewModel>().AsSelf().SingleInstance();
    }

    public void OnInitialized(IComponentContext context)
    {
        var settingsService = context.Resolve<ISettingsService>();
        var windowService = context.Resolve<IWindowService>();

        if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            Environment.SetEnvironmentVariable("GCM_CREDENTIAL_STORE", "secretservice");

        settingsService.RegisterCustom("Team Explorer", "GitHub", GitHubAccountNameKey, new GitHubAccountSetting());
        
        settingsService.RegisterSettingCategory("Team Explorer", 10, "VsImageLib.Team16X");
        settingsService.RegisterTitled("Team Explorer", "Fetch", "SourceControl_AutoFetchEnable",
            "Auto fetch", "Fetch for changed automatically", true);
        settingsService.RegisterTitledSlider("Team Explorer", "Fetch", "SourceControl_AutoFetchDelay",
            "Auto fetch interval", "Interval in seconds", 60, 5, 60, 5);
        settingsService.RegisterTitled("Team Explorer", "Polling", "SourceControl_PollChangesEnable",
            "Poll for changes", "Fetch for changed files automatically", true);
        settingsService.RegisterTitledSlider("Team Explorer", "Polling", "SourceControl_PollChangesDelay",
            "Poll changes interval", "Interval in seconds", 5, 1, 60, 1);
        

        var dockService = context.Resolve<IDockService>();
        dockService.RegisterLayoutExtension<SourceControlViewModel>(DockShowLocation.Left);

        windowService.RegisterMenuItem("MainWindow_MainMenu/View/Tool Windows", new MenuItemViewModel("SourceControl")
        {
            Header = "Source Control",
            Command = new RelayCommand(() => dockService.Show(context.Resolve<SourceControlViewModel>())),
            IconObservable = Application.Current!.GetResourceObservable(SourceControlViewModel.IconKey)
        });

        if (context.Resolve<SourceControlViewModel>() is not { } vm) return;

        windowService.RegisterUiExtension("MainWindow_BottomRightExtension", new UiExtension(x =>
            new SourceControlMainWindowBottomRightExtension
            {
                DataContext = vm
            }));
    }
}