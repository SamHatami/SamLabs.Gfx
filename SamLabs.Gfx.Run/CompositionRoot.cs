using Microsoft.Extensions.DependencyInjection;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Geometry;
using SamLabs.Gfx.Viewer;
using Serilog;

namespace SamLabs.Gfx.Run;

public static class CompositionRoot
{
    private static readonly ServiceCollection Services = [];

    public static IServiceProvider ConfigureServices()
    {
        RegisterLogger();
        RegisterGlContextAndWindow();
        RegisterServiceModules();

        return Services.BuildServiceProvider();
    }

    private static void RegisterLogger()
    {
        var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("SamLabGfx_Log.txt")
            .CreateLogger();
        
        Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger, dispose: true));
    }

    private static void RegisterGlContextAndWindow()
    {
        var nativeSettings = new NativeWindowSettings()
        {
            ClientSize = new Vector2i(1280, 720),
            Title = "SamLabs.Gfx.Viewer",
        };

        var window = new GameWindow(GameWindowSettings.Default, nativeSettings);

        Services.AddSingleton(window);
    }

    private static void RegisterServiceModules()
    {
        var modules = new IServiceModule[]
        {
            new RenderServiceModule(),
            new GeometryServiceModule()
        };

        foreach (var serviceModule in modules)
        {
            serviceModule.RegisterServices(Services);
        }
    }
}
