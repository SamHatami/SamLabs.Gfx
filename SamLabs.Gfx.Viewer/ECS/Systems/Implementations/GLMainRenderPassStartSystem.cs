using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

[RenderPassAttributes.RenderOrder(RenderOrders.MainStart)]
public class GLMainRenderPassStartSystem: RenderSystem
{
    public override int RenderPosition => RenderOrders.MainStart;
    public GLMainRenderPassStartSystem(ComponentManager componentManager, EntityManager entityManager) : base(componentManager, entityManager)
    {
    }

    public override void Update(FrameInput frameInput,RenderContext renderContext)
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, renderContext.MainViewFrameBuffer);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.LineSmooth);
        GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Viewport(0, 0, renderContext.ViewWidth,renderContext.ViewHeight);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }
}