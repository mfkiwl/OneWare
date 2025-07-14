using Autofac;

namespace OneWare.Core;

public static class ContainerLocator
{
    private static IContainer? _container;

    public static IContainer Container => _container ?? throw new InvalidOperationException("Container not initialized");

    public static IContainer Current => Container;

    internal static void SetContainer(IContainer container)
    {
        _container = container;
    }
}
