﻿using DynamicData.Binding;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;

namespace OneWare.ProjectSystem.Models;

public class ProjectFile : ProjectEntry, IProjectFile
{
    public ProjectFile(string header, IProjectFolder topFolder) : base(header, topFolder)
    {
        IDisposable? fileSubscription = null;

        this.WhenValueChanged(x => x.FullPath).Subscribe(x =>
        {
            fileSubscription?.Dispose();
            var observable = ContainerLocator.Container.Resolve<IFileIconService>().GetFileIcon(Extension);
            fileSubscription = observable?.Subscribe(icon => { Icon = icon; });
        });
    }

    public DateTime LastSaveTime { get; set; } = DateTime.MinValue;
    public string Extension => Path.GetExtension(FullPath);
}