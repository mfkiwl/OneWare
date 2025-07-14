#!/bin/bash

# List of modules to update
modules=(
    "OneWare.Output"
    "OneWare.ProjectExplorer"
    "OneWare.LibraryExplorer"
    "OneWare.FolderProjectSystem"
    "OneWare.ImageViewer"
    "OneWare.Json"
    "OneWare.Toml"
    "OneWare.Debugger"
    "OneWare.CloudIntegration"
    "OneWare.ChatBot"
    "OneWare.TerminalManager"
    "OneWare.SerialMonitor"
    "OneWare.PackageManager"
    "OneWare.OssCadSuiteIntegration"
    "OneWare.UniversalFpgaProjectSystem"
    "OneWare.Vhdl"
    "OneWare.Verilog"
    "OneWare.Cpp"
    "OneWare.Cyc5000"
    "OneWare.CruviAdapterExtensions"
)

echo "Updating modules to use Autofac..."

for module in "${modules[@]}"; do
    module_file="src/${module}/${module##*.}Module.cs"
    if [ -f "$module_file" ]; then
        echo "Updating $module_file"
        
        # Replace using statements
        sed -i 's/using Prism\.Ioc;/using Autofac;\nusing OneWare.Core.ModuleLogic;/' "$module_file"
        sed -i 's/using Prism\.Modularity;//' "$module_file"
        
        # Replace IModule with IAutofacModule
        sed -i 's/: IModule/: IAutofacModule/' "$module_file"
        
        # Replace method signatures
        sed -i 's/RegisterTypes(IContainerRegistry containerRegistry)/RegisterTypes(ContainerBuilder builder)/' "$module_file"
        sed -i 's/OnInitialized(IContainerProvider containerProvider)/OnInitialized(IComponentContext context)/' "$module_file"
        
        # Replace registration methods
        sed -i 's/containerRegistry\.RegisterSingleton</builder.RegisterType</g' "$module_file"
        sed -i 's/containerRegistry\.Register</builder.RegisterType</g' "$module_file"
        
        # Replace resolution methods
        sed -i 's/containerProvider\.Resolve</context.Resolve</g' "$module_file"
        
        echo "Updated $module_file"
    else
        echo "File $module_file not found"
    fi
done

echo "Module updates completed!"
