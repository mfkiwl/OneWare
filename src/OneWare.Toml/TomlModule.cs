using OneWare.Essentials.Services;
using Autofac;
using OneWare.Core.ModuleLogic;


namespace OneWare.Toml;

public class TomlModule : IAutofacModule
{
    public void RegisterTypes(ContainerBuilder builder)
    {
    }

    public void OnInitialized(IComponentContext context)
    {
        context.Resolve<ILanguageManager>()
            .RegisterStandaloneTypeAssistance(typeof(TypeAssistanceToml), ".toml");
        context.Resolve<ILanguageManager>()
            .RegisterTextMateLanguage("toml", "avares://OneWare.Toml/Assets/TOML.tmLanguage.json", ".toml");
    }
}