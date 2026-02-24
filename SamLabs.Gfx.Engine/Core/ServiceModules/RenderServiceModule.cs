using Microsoft.Extensions.DependencyInjection;
using SamLabs.Gfx.Core.Framework;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.SceneGraph;
using SamLabs.Gfx.Engine.Systems.Selection;

namespace SamLabs.Gfx.Engine.Core.ServiceModules;

public class RenderServiceModule : IServiceModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<MaterialLibrary>();
        services.AddSingleton<ShaderService>();
        services.AddSingleton<UniformBufferService>();
        services.AddSingleton<FrameBufferService>();
        services.AddSingleton<IGraphicsBackend, OpenGLGraphicsBackend>();
        services.AddSingleton<IPickingOutput, EcsPickingOutput>();
        services.AddSingleton<IRenderer,OpenGLRenderer>();
        services.AddSingleton<ISceneManager, SceneManager>();
        return services;
    }
}