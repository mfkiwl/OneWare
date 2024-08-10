using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OneWare.Essentials.Helpers;
using OneWare.Essentials.LanguageService;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
// ReSharper disable InconsistentNaming

namespace OneWare.AiAssistant;

public class LanguageServiceAi : LanguageServiceLsp
{
    public LanguageServiceAi(string workspace, ISettingsService settingsService)
        : base(AiAssistantModule.LspName, workspace)
    {
        ExecutablePath = "/home/hmenn/.homes/fedora-dev/.cargo/bin/lsp-ai";
    }

    public override void SetCustomOptions(LanguageClientOptions options)
    {
        base.SetCustomOptions(options);

        options.WithInitializationOptions(new AiInitialisationOptions()
        {
            memory = new AiInitialisationMemory(),
            models = new AiInitialisationModels()
            {
                model1 = new AiInitialisationModel()
                {
                    type = "ollama",
                    model = "deepseek-coder"
                }
            },
            completion = new AiInitialisationCompletion()
            {
                model = "model1"
            }
        });
    }

    public override ITypeAssistance GetTypeAssistance(IEditor editor)
    {
        return new TypeAssistanceAi(editor, this);
    }
    
    private class AiInitialisationOptions
    {
        public AiInitialisationMemory? memory { get; init; }
        
        public AiInitialisationModels? models { get; init; }
        
        public AiInitialisationCompletion? completion { get; init; }
    }

    private class AiInitialisationMemory
    {
        public object file_store { get; init; } = new object();
    }
    
    private class AiInitialisationModels
    {
        public AiInitialisationModel? model1 { get; init; }
    }
    
    private class AiInitialisationCompletion
    {
        public string? model { get; init; }
        
        public object? parameters { get; init; } = new object();
    }
    
    private class AiInitialisationModel
    {
        public string? type { get; init; }
        public string? model { get; init; }
    }
}

