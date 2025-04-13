using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.WebIO;
using OneWare.Core.Services;
using OneWare.Essentials.Services;

namespace OneWare.Studio.Browser;

public class WebStudioApp : StudioApp
{
    private async void CopyAvaloniaAssetIntoFolder(Uri asset, string output)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(output)!);
            var webIoFile = await WebAssetLoader.LoadContentAsync(asset);
            File.WriteAllBytes(output, webIoFile.Content!);
        }
        catch (Exception e)
        {
            ContainerProvider.Resolve<ILogger>()?.Error(e.Message, e);
        }
    }

    protected override async Task LoadContentAsync()
    {
        try
        {
            var testProj = Path.Combine(ContainerProvider.Resolve<IPaths>().ProjectsDirectory, "DemoProject");

            Directory.CreateDirectory(testProj);
            
            //Highlighting will not work with NET9, wait for NET10
            CopyAvaloniaAssetIntoFolder(new Uri("avares://OneWare.Studio.Browser/Assets/DemoFiles/DemoProject.fpgaproj"), Path.Combine(testProj, "DemoProject.fpgaproj"));
            //CopyAvaloniaAssetIntoFolder(new Uri("avares://OneWare.Studio.Browser/Assets/DemoFiles/CppTest.cpp"), Path.Combine(testProj, "CPP", "CppTest.cpp"));
            CopyAvaloniaAssetIntoFolder(new Uri("avares://OneWare.Studio.Browser/Assets/DemoFiles/VhdlTest.vhd"), Path.Combine(testProj, "VHDL", "VhdlTest.vhd"));
            CopyAvaloniaAssetIntoFolder(new Uri("avares://OneWare.Studio.Browser/Assets/DemoFiles/VerilogTest.v"), Path.Combine(testProj, "Verilog", "VerilogTest.v"));
            CopyAvaloniaAssetIntoFolder(new Uri("avares://OneWare.Studio.Browser/Assets/DemoFiles/IceStorm.xdc"), Path.Combine(testProj, "Constraints", "IceStorm.xdc"));

            await Task.Delay(500);
            
            var key = ContainerProvider.Resolve<IApplicationStateService>().AddState("Loading demo project...", Essentials.Enums.AppState.Loading);

            await ContainerProvider.Resolve<IProjectExplorerService>().LoadProjectAsync(Path.Combine(testProj, "DemoProject.fpgaproj"),
                ContainerProvider.Resolve<IProjectManagerService>().GetManagerByExtension(".fpgaproj")!);
            
            ContainerProvider.Resolve<IDockService>().InitializeContent();
            
            ContainerProvider.Resolve<IApplicationStateService>().RemoveState(key, "Demo project loaded!");
            
            ContainerProvider.Resolve<ILogger>().Log("Demo project loaded!", ConsoleColor.Cyan);
        }
        catch (Exception e)
        {
            ContainerProvider.Resolve<ILogger>().Error(e.Message, e);
        }
    }
}