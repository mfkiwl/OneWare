using OneWare.Essentials.Services;
using Autofac;
using OneWare.Core.ModuleLogic;


namespace OneWare.Json;

public class JsonModule : IAutofacModule
{
    public void RegisterTypes(ContainerBuilder builder)
    {
    }

    public void OnInitialized(IComponentContext context)
    {
        context.Resolve<ILanguageManager>()
            .RegisterStandaloneTypeAssistance(typeof(TypeAssistanceJson), ".json");
    }
}