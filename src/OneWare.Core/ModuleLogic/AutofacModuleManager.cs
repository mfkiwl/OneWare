using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using OneWare.Essentials.Services;

namespace OneWare.Core.ModuleLogic;

public class AutofacModuleManager
{
    private readonly List<IAutofacModule> _modules = new();
    private readonly ContainerBuilder _builder;
    private IContainer? _container;

    public AutofacModuleManager(ContainerBuilder builder)
    {
        _builder = builder;
    }

    public void AddModule<T>() where T : IAutofacModule, new()
    {
        var module = new T();
        _modules.Add(module);
        module.RegisterTypes(_builder);
    }

    public void AddModulesFromAssembly(Assembly assembly)
    {
        var moduleTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IAutofacModule).IsAssignableFrom(t))
            .ToList();

        foreach (var moduleType in moduleTypes)
        {
            if (System.Activator.CreateInstance(moduleType) is IAutofacModule module)
            {
                _modules.Add(module);
                module.RegisterTypes(_builder);
            }
        }
    }

    public void InitializeModules(IContainer container)
    {
        _container = container;
        foreach (var module in _modules)
        {
            try
            {
                module.OnInitialized(container);
            }
            catch (System.Exception ex)
            {
                // Log error but continue with other modules
                container.Resolve<ILogger>()?.Error($"Failed to initialize module {module.GetType().Name}: {ex.Message}", ex);
            }
        }
    }

    public IContainer Container => _container ?? throw new InvalidOperationException("Container not yet built");
}
