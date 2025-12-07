using SamLabs.Gfx.Viewer.ECS.Interfaces;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;

public abstract class PreRenderSystem : ISystem
{
    protected readonly ComponentManager ComponentManager;
    protected IRenderer Renderer;

    protected PreRenderSystem(ComponentManager componentManager)
    {
        ComponentManager = componentManager;
    }

    public void Initialize(IRenderer renderer)
    {
        Renderer = renderer;
    } 

    public abstract void Update(FrameInput frameInput,RenderContext renderContext);
}