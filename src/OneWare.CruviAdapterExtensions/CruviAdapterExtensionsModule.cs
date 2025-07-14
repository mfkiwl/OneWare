using OneWare.UniversalFpgaProjectSystem.Fpga;
using OneWare.UniversalFpgaProjectSystem.Services;
using Autofac;
using OneWare.Core.ModuleLogic;


namespace OneWare.CruviAdapterExtensions;

public class CruviAdapterExtensionsModule : IAutofacModule
{
    public void RegisterTypes(ContainerBuilder builder)
    {
    }

    public void OnInitialized(IComponentContext context)
    {
        context.Resolve<FpgaService>().RegisterFpgaExtensionPackage(new GenericFpgaExtensionPackage("CRUVI_LS to PMOD Adapter", "CRUVI_LS", "avares://OneWare.CruviAdapterExtensions/Assets/CRUVI_LS/CRUVI_LS to PMOD Adapter"));
        context.Resolve<FpgaService>().RegisterFpgaExtensionPackage(new GenericFpgaExtensionPackage("PMOD to CRUVI_LS Adapter", "PMOD", "avares://OneWare.CruviAdapterExtensions/Assets/PMOD/PMOD to CRUVI_LS Adapter"));
    }
}