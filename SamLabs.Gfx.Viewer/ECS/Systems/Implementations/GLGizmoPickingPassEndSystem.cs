using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

[RenderPassAttributes.GizmoPickingEnd]
public class GLGizmoRenderPassEndSystem: RenderSystem
{
    public override int RenderPosition => RenderOrders.GizmoPickingEnd;
    public GLGizmoRenderPassEndSystem(ComponentManager componentManager, EntityManager entityManager) : base(componentManager, entityManager)
    {
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        GL.Disable(EnableCap.ScissorTest);
        GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
    }


}