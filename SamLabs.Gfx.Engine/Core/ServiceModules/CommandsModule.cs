using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Core.Framework;
using SamLabs.Gfx.Engine.Commands;

namespace SamLabs.Gfx.Engine.Core.ServiceModules;

public class CommandsModule : IServiceModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<CommandManager>();

        RegisterCommands(services);

        return services;
    }

    private void RegisterCommands(IServiceCollection services)
    {
        //Creational commands
        services.AddTransient<AddBoxCommand>();
        services.AddTransient<AddConstructionPlaneCommand>();
        services.AddTransient<AddImportedFileCommand>();

        //Modification commands
        services.AddTransient<RemoveRenderableCommand>();
        
        //Register where on which panel each command should be displayed?
    }
}