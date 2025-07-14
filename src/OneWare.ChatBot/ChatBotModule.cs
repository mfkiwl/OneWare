using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using OneWare.ChatBot.ViewModels;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using Autofac;
using OneWare.Core.ModuleLogic;


namespace OneWare.ChatBot;

public class ChatBotModule : IAutofacModule
{
    public void RegisterTypes(ContainerBuilder builder)
    {
        builder.RegisterType<ChatBotViewModel>();
    }

    public void OnInitialized(IComponentContext context)
    {
        var dockService = context.Resolve<IDockService>();
        var windowService = context.Resolve<IWindowService>();
        
        dockService.RegisterLayoutExtension<ChatBotViewModel>(DockShowLocation.Bottom);
        
        windowService.RegisterMenuItem("MainWindow_MainMenu/View/Tool Windows", new MenuItemViewModel("ChatBot")
        {
            Header = "OneAI Chat",
            Command = new RelayCommand(() => dockService.Show(context.Resolve<ChatBotViewModel>())),
            IconObservable = Application.Current!.GetResourceObservable(ChatBotViewModel.IconKey) ,
        });
    }
}