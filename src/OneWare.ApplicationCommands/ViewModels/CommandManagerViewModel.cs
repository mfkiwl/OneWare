using System.Collections.ObjectModel;
using Avalonia.Input;
using Avalonia.LogicalTree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OneWare.ApplicationCommands.Tabs;
using OneWare.ApplicationCommands.Views;
using OneWare.Essentials.Commands;
using OneWare.Essentials.Controls;
using OneWare.Essentials.Helpers;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;

namespace OneWare.ApplicationCommands.ViewModels;

public partial class CommandManagerViewModel : FlexibleWindowViewModelBase
{
    private readonly IApplicationCommandService _applicationCommandService;

    private readonly IProjectExplorerService _projectExplorerService;
    private readonly IWindowService _windowService;
    private readonly IDockService _dockService;

    private readonly PlatformHelper _platformHelper;

    [ObservableProperty] private CommandManagerTabBase _selectedTab;

    public CommandManagerViewModel(ILogical logical, 
                                   IApplicationCommandService commandService,
                                   IProjectExplorerService projectExplorerService,
                                   PlatformHelper platformHelper,
                                   IWindowService windowService,
                                   IDockService dockService)
    {
        ActiveFocus = logical;
        _projectExplorerService = projectExplorerService;
        _applicationCommandService = commandService;
        _windowService = windowService;
        _platformHelper = platformHelper;
        _dockService = dockService;

        Tabs.Add(new CommandManagerAllTab(logical)
        {
            Items = new ObservableCollection<IApplicationCommand>(GetOpenFileCommands()
                .Concat(commandService.ApplicationCommands))
        });
        Tabs.Add(new CommandManagerFilesTab(logical)
        {
            Items = GetOpenFileCommands()
        });
        Tabs.Add(new CommandManagerActionsTab(logical)
        {
            Items = commandService.ApplicationCommands
        });

        _selectedTab = Tabs.Last();
        _dockService = dockService;

    }

    public KeyGesture ChangeShortcutGesture => new(Key.Enter, _platformHelper.ControlKey);

    public ILogical ActiveFocus { get; }
    public ObservableCollection<CommandManagerTabBase> Tabs { get; } = new();

    private ObservableCollection<IApplicationCommand> GetOpenFileCommands()
    {
        var collection = new ObservableCollection<IApplicationCommand>();
        foreach (var project in _projectExplorerService.Projects)
        {
            foreach (var file in project.Files)
            {
                collection.Add(new OpenFileApplicationCommand(file, _dockService));
            }
        }
        return collection;
    }

    public void ExecuteSelection(FlexibleWindow window)
    {
        if (SelectedTab?.SelectedItem?.Command.Execute(ActiveFocus) ?? false)
            Close(window);
    }

    [RelayCommand]
    private async Task ChangeShortcutAsync(FlexibleWindow window)
    {
        if (SelectedTab is not CommandManagerActionsTab || SelectedTab.SelectedItem == null) return;

        window.CloseOnDeactivated = false;

        await _windowService.ShowDialogAsync(new AssignGestureView
        {
            DataContext = new AssignGestureViewModel(SelectedTab.SelectedItem.Command)
        }, window.Host);

        _applicationCommandService.SaveKeyConfiguration();

        window.Host?.Activate();
        window.CloseOnDeactivated = true;
    }
}