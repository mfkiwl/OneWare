namespace OneWare.Core.Services;

/// <summary>
/// Interface for the module catalog
/// This is a replacement for Prism's IModuleCatalog
/// </summary>
public interface IModuleCatalog
{
    /// <summary>
    /// Adds a module to the catalog.
    /// </summary>
    /// <typeparam name="T">The type of the module to add.</typeparam>
    void AddModule<T>() where T : IModule, new();
    
    /// <summary>
    /// Gets all modules in the catalog.
    /// </summary>
    /// <returns>The collection of modules.</returns>
    IEnumerable<Type> GetModules();
}