using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using OneWare.Essentials.Helpers;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using OneWare.SearchList.ViewModels;
using Autofac;
using OneWare.Core.ModuleLogic;

namespace OneWare.SearchList;

public class SearchListModule : IAutofacModule
{
    public void RegisterTypes(ContainerBuilder builder)
    {
        builder.RegisterType<SearchListViewModel>().AsSelf().SingleInstance();
    }

    public void OnInitialized(IComponentContext context)
    {
        var windowService = context.Resolve<IWindowService>();
        var dockService = context.Resolve<IDockService>();
        
        windowService.RegisterMenuItem("MainWindow_MainMenu/View/Tool Windows", new MenuItemViewModel("Search")
        {
            Header = "Search",
            Command = new RelayCommand(() =>
            {
                var vm = context.Resolve<SearchListViewModel>();
                vm.SearchString = string.Empty;
                dockService.Show(vm);
            }),
            IconObservable = Application.Current!.GetResourceObservable(SearchListViewModel.IconKey),
            InputGesture = new KeyGesture(Key.F, KeyModifiers.Shift | PlatformHelper.ControlKey)
        });
    }
}