using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Core.Framework;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.SceneGraph;

namespace SamLabs.Gfx.Engine.Core.ServiceModules;

public class RenderServiceModule : IServiceModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<MaterialLibrary>();
        services.AddSingleton<ShaderService>();
        services.AddSingleton<UniformBufferService>();
        services.AddSingleton<FrameBufferService>();
        services.AddSingleton<IRenderer,OpenGLRenderer>();
        services.AddSingleton<ISceneManager, SceneManager>();
        return services;
    }
}