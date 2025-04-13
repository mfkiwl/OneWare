// Change line 294 in InitLayout method
HostWindowLocator = new Dictionary<string, Func<IHostWindow?>>
{
    [nameof(IDockWindow)] = () => AutofacContainerProvider.Resolve<AdvancedHostWindow>()
};

// Change line 304 in Show<T> method
public void Show<T>(DockShowLocation location = DockShowLocation.Window) where T : IDockable
{
    Show(AutofacContainerProvider.Resolve<T>(), location);
}

// Change line 293 in OpenFileAsync method
var viewModel = AutofacContainerProvider.Resolve(type, (typeof(string), pf.FullPath)) as IExtendedDocument;

// Change line 323 in GetWindowOwner method
return Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime
    ? AutofacContainerProvider.Resolve<MainWindow>()
    : null;

// Change line 359 in Show method
var mainWindow = AutofacContainerProvider.Resolve<MainWindow>();

// Change line 400 in LoadLayout method
AutofacContainerProvider.Resolve<ILogger>()
    ?.Log("Could not load layout from file! Loading default layout..." + e, ConsoleColor.Red);