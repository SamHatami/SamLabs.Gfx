using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Viewer.Framework;
using SamLabs.Gfx.Viewer.Primitives;

namespace SamLabs.Gfx.Viewer;

public class RenderServiceModule : IServiceModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<ShaderManager>();
        services.AddSingleton<UniformBufferManager>();
        services.AddSingleton<Renderer>();
        services.AddSingleton<ISceneManager, SceneManager>();
        
        return services;
    }
}