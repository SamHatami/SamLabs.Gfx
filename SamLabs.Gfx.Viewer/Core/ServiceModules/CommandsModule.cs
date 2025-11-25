using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Core.Framework;
using SamLabs.Gfx.Viewer.Commands;

namespace SamLabs.Gfx.Viewer.Core.ServiceModules;

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
        services.AddTransient<AddImportedFileCommand>();

        //Modification commands
        services.AddTransient<RemoveRenderableCommand>();
        
        //Register where on which panel each command should be displayed?
    }
}