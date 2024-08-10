using AvaloniaEdit.Indentation.CSharp;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OneWare.Essentials.EditorExtensions;
using OneWare.Essentials.LanguageService;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using Prism.Ioc;

namespace OneWare.AiAssistant;

internal class TypeAssistanceAi : TypeAssistanceLanguageService
{
    public TypeAssistanceAi(IEditor editor, LanguageServiceAi ls) : base(editor, ls)
    {
        CodeBox.TextArea.IndentationStrategy = IndentationStrategy = new CSharpIndentationStrategy(CodeBox.Options);
        LineCommentSequence = "//";
    }
}