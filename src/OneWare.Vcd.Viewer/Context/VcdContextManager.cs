﻿using System.Text.Json;
using Microsoft.Extensions.Logging;
using OneWare.Essentials.Services;
using Prism.Ioc;

namespace OneWare.Vcd.Viewer.Context;

public static class VcdContextManager
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public static async Task<VcdContext?> LoadContextAsync(string path)
    {
        if (File.Exists(path))
            try
            {
                await using var stream = File.OpenRead(path);
                return await JsonSerializer.DeserializeAsync<VcdContext>(stream, Options);
            }
            catch (Exception e)
            {
                ContainerLocator.Container.Resolve<ILogger>().LogError(e, e.Message);
            }

        return null;
    }

    public static async Task<bool> SaveContextAsync(string path, VcdContext context)
    {
        try
        {
            await using var stream = File.OpenWrite(path);
            stream.SetLength(0);
            await JsonSerializer.SerializeAsync(stream, context, Options);
            return true;
        }
        catch (Exception e)
        {
            ContainerLocator.Container.Resolve<ILogger>().LogError(e, e.Message);
            return false;
        }
    }
}