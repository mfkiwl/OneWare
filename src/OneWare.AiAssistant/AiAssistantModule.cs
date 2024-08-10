using OneWare.Essentials.Helpers;
using OneWare.Essentials.PackageManager;
using OneWare.Essentials.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace OneWare.AiAssistant;

public class AiAssistantModule : IModule
{
    public const string LspName = "ai-assistant";

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        containerProvider.Resolve<IErrorService>().RegisterErrorSource(LspName);

        containerProvider.Resolve<ILanguageManager>()
            .RegisterService(typeof(LanguageServiceAi), true, ".java");
    }
}