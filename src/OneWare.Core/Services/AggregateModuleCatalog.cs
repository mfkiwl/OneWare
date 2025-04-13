namespace OneWare.Core.Services;

/// <summary>
/// Aggregate module catalog that combines multiple catalogs
/// This replaces Prism's AggregateModuleCatalog
/// </summary>
public class AggregateModuleCatalog : IModuleCatalog
{
    private readonly List<IModuleCatalog> _catalogs = new();
    private readonly ModuleCatalog _defaultCatalog = new();

    /// <summary>
    /// Adds a module catalog to the aggregate catalog.
    /// </summary>
    /// <param name="catalog">The catalog to add.</param>
    public void AddCatalog(IModuleCatalog catalog)
    {
        _catalogs.Add(catalog);
    }

    /// <inheritdoc />
    public void AddModule<T>() where T : IModule, new()
    {
        _defaultCatalog.AddModule<T>();
    }
    
    /// <summary>
    /// Adds a Prism module to the catalog
    /// </summary>
    /// <typeparam name="T">The type of module to add</typeparam>
    public void AddPrismModule<T>() where T : Prism.Modularity.IModule, new()
    {
        _defaultCatalog.AddPrismModule<T>();
    }

    /// <inheritdoc />
    public IEnumerable<Type> GetModules()
    {
        var modules = new List<Type>();
        modules.AddRange(_defaultCatalog.GetModules());

        foreach (var catalog in _catalogs)
        {
            modules.AddRange(catalog.GetModules());
        }

        return modules;
    }
}