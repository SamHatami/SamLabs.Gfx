using System;
using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Core.Framework;
using SamLabs.Gfx.Geometry;
using SamLabs.Gfx.StandAlone.ViewModels;
using SamLabs.Gfx.Viewer.Core.ServiceModules;
using Serilog;
namespace SamLabs.Gfx.StandAlone;

public class CompositionRoot
{
    private ILogger _logger;
    private readonly ServiceCollection Services = [];

    public IServiceProvider ConfigureServices()
    {
        RegisterLogger();
        RegisterServiceModules();
        RegisterViewModels();

        return Services.BuildServiceProvider();
    }

    private void RegisterViewModels()
    {
        Services.AddTransient<MainWindowViewModel>();
    }

    private void RegisterLogger()
    {
        _logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("SamLabGfx_Log.txt")
            .CreateLogger();
        
        Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(_logger, dispose: true));
    }
    
    private void RegisterServiceModules()
    {
        var modules = new IServiceModule[]
        {
            new RenderServiceModule(),
            new GeometryServiceModule(),
            new EntityComponentSystemModule(),
            new CommandsModule()
        };

        foreach (var serviceModule in modules)
        {
            serviceModule.RegisterServices(Services);
        }
    }
}
