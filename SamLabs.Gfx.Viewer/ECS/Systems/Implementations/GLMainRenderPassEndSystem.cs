using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

[RenderPassAttributes.RenderOrder(SystemOrders.MainEnd)]
public class GLMainRenderPassEndSystem: RenderSystem
{
    public override int SystemPosition => SystemOrders.MainEnd;
    public GLMainRenderPassEndSystem(EntityManager entityManager) : base(entityManager)
    {
    }

    public override void Update(FrameInput frameInput,RenderContext renderContext)
    {

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        GL.Disable(EnableCap.Blend);
        GL.Disable(EnableCap.LineSmooth);
        GL.Disable(EnableCap.DepthTest); 
        GL.BlendFunc(BlendingFactor.One, BlendingFactor.Zero);
    }
}