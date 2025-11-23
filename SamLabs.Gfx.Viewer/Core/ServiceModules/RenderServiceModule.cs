using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Core.Framework;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;
using SamLabs.Gfx.Viewer.Rendering.Engine;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.Core.ServiceModules;

public class RenderServiceModule : IServiceModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<ShaderService>();
        services.AddSingleton<UniformBufferService>();
        services.AddSingleton<FrameBufferService>();
        services.AddSingleton<IRenderer,OpenGLRenderer>();
        services.AddSingleton<ISceneManager, SceneManager>();
        
        return services;
    }
}