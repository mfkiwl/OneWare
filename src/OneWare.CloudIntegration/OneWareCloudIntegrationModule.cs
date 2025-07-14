using System.Runtime.InteropServices;
using OneWare.CloudIntegration.Services;
using OneWare.CloudIntegration.Settings;
using OneWare.CloudIntegration.ViewModels;
using OneWare.CloudIntegration.Views;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using Autofac;
using OneWare.Core.ModuleLogic;

namespace OneWare.CloudIntegration;

public class OneWareCloudIntegrationModule : IAutofacModule
{
    public const string OneWareCloudHostKey = "General_OneWareCloud_Host";
    public const string OneWareAccountEmailKey = "General_OneWareCloud_AccountEmail";
    public const string CredentialStore = "https://cloud.one-ware.com";
    
    public void RegisterTypes(ContainerBuilder builder)
    {
        builder.RegisterType<OneWareCloudAccountSettingViewModel>().AsSelf().SingleInstance();
        builder.RegisterType<OneWareCloudLoginService>().AsSelf().SingleInstance();
        builder.RegisterType<OneWareCloudNotificationService>().AsSelf().SingleInstance();
    }

    public void OnInitialized(IComponentContext context)
    {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            Environment.SetEnvironmentVariable("GCM_CREDENTIAL_STORE", "secretservice");
        
        context.Resolve<ISettingsService>().RegisterSetting("General", "OneWare Cloud", OneWareCloudHostKey, new TextBoxSetting("Host", "https://cloud.one-ware.com", "https://cloud.one-ware.com"));
        
        context.Resolve<ISettingsService>().RegisterCustom("General", "OneWare Cloud", OneWareAccountEmailKey, new OneWareCloudAccountSetting());
        
        context.Resolve<IWindowService>().RegisterUiExtension("MainWindow_BottomRightExtension", new UiExtension(x =>
            new CloudIntegrationMainWindowBottomRightExtension()
            {
                DataContext = context.Resolve<CloudIntegrationMainWindowBottomRightExtensionViewModel>()
            }));
    }
}