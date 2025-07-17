﻿using System.Text.Json.Nodes;
using OneWare.Essentials.Controls;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using OneWare.Settings;
using OneWare.Settings.ViewModels;
using OneWare.Settings.ViewModels.SettingTypes;
using OneWare.UniversalFpgaProjectSystem.Models;
using OneWare.UniversalFpgaProjectSystem.Services;

namespace OneWare.UniversalFpgaProjectSystem.ViewModels;

public class UniversalFpgaProjectCreatorViewModel : FlexibleWindowViewModelBase
{
    private readonly TitledSetting _createNewFolderSetting;
    private readonly FolderPathSetting _folderPathSetting;
    private readonly FpgaService _fpgaService;
    private readonly ComboBoxSetting _loaderSetting;
    private readonly ILogger _logger;
    private readonly UniversalFpgaProjectManager _manager;

    private readonly TextBoxSetting _nameSetting;
    private readonly IProjectExplorerService _projectExplorerService;
    private readonly ComboBoxSearchSetting _templateSetting;
    private readonly ComboBoxSetting _toolchainSetting;

    public UniversalFpgaProjectCreatorViewModel(IPaths paths, IProjectExplorerService projectExplorerService,
        ILogger logger, FpgaService fpgaService, UniversalFpgaProjectManager manager)
    {
        _projectExplorerService = projectExplorerService;
        _fpgaService = fpgaService;
        _logger = logger;
        _manager = manager;
        Paths = paths;

        _nameSetting = new TextBoxSetting("Name", "", "Enter name...")
        {
            HoverDescription = "Set the name for the project"
        };

        _templateSetting = new ComboBoxSearchSetting("Template", "Empty",
            new[] { "Empty" }.Concat(fpgaService.Templates.Select(x => x.Name)))
        {
            HoverDescription = "Set the template used for this project"
        };

        _folderPathSetting = new FolderPathSetting("Location",
            paths.ProjectsDirectory, "Enter path...", paths.ProjectsDirectory, Directory.Exists)
        {
            HoverDescription = "Set the location where the new project is created"
        };

        _createNewFolderSetting = new CheckBoxSetting("Create new Folder", true)
        {
            HoverDescription = "Set if a new folder should be created in the selected location"
        };

        _toolchainSetting = new ComboBoxSetting("Toolchain",
            fpgaService.Toolchains.FirstOrDefault()?.Name ?? "Unset",
            new[] { "Unset" }.Concat(fpgaService.Toolchains
                .Select(x => x.Name)))
        {
            HoverDescription = "Set the toolchain to use for the project (can be changed later)"
        };

        _loaderSetting = new ComboBoxSetting("Loader",
            fpgaService.Loaders.FirstOrDefault()?.Name ?? "Unset",
            new[] { "Unset" }.Concat(fpgaService.Loaders
                .Select(x => x.Name)))
        {
            HoverDescription = "Set the loader to use for the project (can be changed later)"
        };

        SettingsCollection.SettingModels.Add(_nameSetting);
        SettingsCollection.SettingModels.Add(_templateSetting);
        SettingsCollection.SettingModels.Add(_folderPathSetting);
        SettingsCollection.SettingModels.Add(_createNewFolderSetting);
        SettingsCollection.SettingModels.Add(_toolchainSetting);
        SettingsCollection.SettingModels.Add(_loaderSetting);
    }

    public IPaths Paths { get; }

    public SettingsCollectionViewModel SettingsCollection { get; } = new("Project Properties")
    {
        ShowTitle = false
    };

    public async Task SaveAsync(FlexibleWindow window)
    {
        var name = (string)_nameSetting.Value;
        var folder = (string)_folderPathSetting.Value;
        var createNewFolder = (bool)_createNewFolderSetting.Value;

        if (string.IsNullOrWhiteSpace(name))
        {
            string errorMsg = "Invalid project name!";
            _logger.Error(errorMsg, null);
            UserNotification.NewError(errorMsg)
                .ViaOutput()
                .ViaWindow(window.Host)
                .Send();
            
            return;
        }

        if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
        {
            string errorMsg = "Invalid project folder!";
            _logger.Error(errorMsg, null);
            UserNotification.NewError(errorMsg)
                .ViaOutput()
                .ViaWindow(window.Host)
                .Send();
            
            return;
        }

        try
        {
            if (createNewFolder)
            {
                folder = Path.Combine(folder, name);
                Directory.CreateDirectory(folder);
            }

            var projectFile = Path.Combine(folder, name + UniversalFpgaProjectRoot.ProjectFileExtension);

            var defaultProperties = new JsonObject
            {
                ["Include"] = new JsonArray("*.vhd", "*.vhdl", "*.v", "*.vcd", "vhdl_ls.toml"),
                ["Exclude"] = new JsonArray("build")
            };
            var root = new UniversalFpgaProjectRoot(projectFile, defaultProperties);

            if (_fpgaService.Loaders.FirstOrDefault(x => x.Name == _loaderSetting.Value.ToString()) is { } loader)
                root.Loader = loader;

            if (_fpgaService.Toolchains.FirstOrDefault(x => x.Name == _toolchainSetting.Value.ToString()) is { } tc)
            {
                root.Toolchain = tc;
                tc.OnProjectCreated(root);
            }

            await _manager.SaveProjectAsync(root);

            _projectExplorerService.Insert(root);

            _projectExplorerService.ActiveProject = root;

            if (_fpgaService.Templates.FirstOrDefault(x => x.Name == _templateSetting.Value.ToString()) is { } template)
                template.FillTemplate(root);

            await _manager.SaveProjectAsync(root);

            root.IsExpanded = true;

            Close(window);

            _ = _projectExplorerService.SaveLastProjectsFileAsync();
        }
        catch (Exception e)
        {
            _logger.Error(e.Message, e);
        }
    }
}