using Autofac;

namespace OneWare.Core.ModuleLogic;

public interface IAutofacModule
{
    void RegisterTypes(ContainerBuilder builder);
    void OnInitialized(IComponentContext context);
}
