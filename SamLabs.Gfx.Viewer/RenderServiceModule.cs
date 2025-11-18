using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Core.Framework;
using SamLabs.Gfx.Viewer.Display;
using SamLabs.Gfx.Viewer.Interfaces;
using SamLabs.Gfx.Viewer.Scenes;

namespace SamLabs.Gfx.Viewer;

public class RenderServiceModule : IServiceModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<ShaderManager>();
        services.AddSingleton<UniformBufferManager>();
        services.AddSingleton<FrameBufferHandler>();
        services.AddSingleton<IRenderer,RenderSystem>();
        services.AddSingleton<ISceneManager, SceneManager>();
        
        return services;
    }
}