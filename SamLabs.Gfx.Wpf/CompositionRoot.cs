using Microsoft.Extensions.DependencyInjection;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Geometry;
using SamLabs.Gfx.Viewer;
using SamLabs.Gfx.Wpf;
using SamLabs.Gfx.Wpf.ViewModels;
using Serilog;

namespace SamLabs.Gfx.Run;

public static class CompositionRoot
{
    private static ILogger _logger;
    private static readonly ServiceCollection Services = [];

    public static IServiceProvider Configure()
    {

        RegisterLogger();
        RegisterServiceModules();
        RegisterViews();
        RegisterViewModels();
        return Services.BuildServiceProvider();
    }

    private static void RegisterViewModels()
    {
        Services.AddSingleton<MainViewModel>();
    }

    private static void RegisterViews()
    {
        Services.AddTransient<MainView>();
    }

    private static void RegisterLogger()
    {
        _logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("SamLabGfx_Log.txt")
            .CreateLogger();
        
        Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(_logger, dispose: true));
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
