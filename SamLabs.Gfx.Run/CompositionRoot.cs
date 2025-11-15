using Microsoft.Extensions.DependencyInjection;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Geometry;
using SamLabs.Gfx.Viewer;
using SamLabs.Gfx.Viewer.Display;
using Serilog;
using Serilog.Core;

namespace SamLabs.Gfx.Run;

public static class CompositionRoot
{
    private static ILogger _logger;
    private static readonly ServiceCollection Services = [];

    public static IServiceProvider ConfigureServices()
    {
        RegisterLogger();
        RegisterServiceModules();
        RegisterGlContextAndWindow();

        return Services.BuildServiceProvider();
    }

    private static void RegisterLogger()
    {
        _logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("SamLabGfx_Log.txt")
            .CreateLogger();
        
        Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(_logger, dispose: true));
    }

    private static void RegisterGlContextAndWindow()
    {
        var nativeSettings = new NativeWindowSettings()
        {
            ClientSize = new Vector2i(1280, 720),
            Title = "SamLabs.Gfx.Viewer",
            Flags = ContextFlags.Debug
        };
        
        

        var window = new ViewerWindow(GameWindowSettings.Default, nativeSettings);

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
