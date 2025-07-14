using OneWare.Essentials.Services;
using OneWare.ImageViewer.ViewModels;
using Autofac;
using OneWare.Core.ModuleLogic;


namespace OneWare.ImageViewer;

public class ImageViewerModule : IAutofacModule
{
    public void RegisterTypes(ContainerBuilder builder)
    {
        builder.RegisterType<ImageViewModel>().AsSelf();
    }

    public void OnInitialized(IComponentContext context)
    {
        context.Resolve<IDockService>().RegisterDocumentView<ImageViewModel>(".svg", ".jpg", ".png", ".jpeg");
    }
}