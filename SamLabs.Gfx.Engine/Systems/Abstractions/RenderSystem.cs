using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.Rendering.Engine;

namespace SamLabs.Gfx.Engine.Systems.Abstractions;

public abstract class RenderSystem
{
    public virtual int SystemPosition { get; }
    protected readonly EntityRegistry EntityRegistry;
    protected readonly IComponentRegistry ComponentRegistry;
    protected IRenderer Renderer;

    protected RenderSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry)
    {
        EntityRegistry = entityRegistry;
        ComponentRegistry = componentRegistry;
    }
    public void Initialize(IRenderer renderer)
    {
        Renderer = renderer;
    }
    
    public abstract void Update(FrameInput frameInput,RenderContext renderContext);
}