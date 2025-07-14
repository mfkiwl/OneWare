using OneWare.Cyc5000.ViewModels;
using OneWare.UniversalFpgaProjectSystem.Services;
using Autofac;
using OneWare.Core.ModuleLogic;


namespace OneWare.Cyc5000;

public class Cyc5000Module : IAutofacModule
{
    public void RegisterTypes(ContainerBuilder builder)
    {
    }

    public void OnInitialized(IComponentContext context)
    {
        context.Resolve<FpgaService>().RegisterFpgaPackage(new Cyc5000FpgaPackage());
    }
}