using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

[RenderPassAttributes.GizmoPickingStart]
public class GLGizmoRenderPassStartSystem: RenderSystem
{
    public override int RenderPosition => RenderOrders.GizmoPickingStart;
    public GLGizmoRenderPassStartSystem(ComponentManager componentManager, EntityManager entityManager) : base(componentManager, entityManager)
    {
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        Renderer.ClearPickingBuffer(renderContext.ViewPort);
        Renderer.RenderToPickingBuffer(renderContext.ViewPort);
    }


}