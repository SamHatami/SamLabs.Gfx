using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.OpenGL;

[RenderPassAttributes.RenderOrder(SystemOrders.MainStart)]
public class GLMainRenderPassStartSystem : RenderSystem
{
    private readonly IGraphicsBackend _graphicsBackend;
    public override int SystemPosition => SystemOrders.MainStart;

    public GLMainRenderPassStartSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry, IGraphicsBackend graphicsBackend) : base(entityRegistry, componentRegistry)
    {
        _graphicsBackend = graphicsBackend;
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        _graphicsBackend.BeginMainRenderPass(renderContext.MainViewFrameBuffer, renderContext.ViewWidth, renderContext.ViewHeight);
    }
}
