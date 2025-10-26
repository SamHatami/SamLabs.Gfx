using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Geometry;
using SamLabs.Gfx.Viewer;
using Serilog;

namespace SamLabs.Gfx.Run;

public class CompositionRoot
{
    private static ServiceProvider _serviceProvider;
    private static ServiceCollection _services;

    public static IServiceProvider ConfigureServices()
    {
        _services = new ServiceCollection();

        var serilogLogger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("SamLabGfx_Log.txt").CreateLogger();
        _services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(serilogLogger, dispose: true));
        RegisterServices(RegisterServiceModules(),_services);
        _serviceProvider = _services.BuildServiceProvider();
        return _serviceProvider;
    }

    private static IServiceModule[] RegisterServiceModules()
    {
        return new IServiceModule[]
        {
            new RenderServiceModule(),
            new GeometryServiceModule()
            //new UIServiceModule()
        };
    }

    private static void RegisterServices(IServiceModule[] serviceModules, IServiceCollection services)
    {

        for (var i = 0; i < serviceModules.Length; i++)
        {
            serviceModules[i].RegisterServices(services);
        }
        
    }
}