using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.OpenGL;

[RenderPassAttributes.RenderOrder(SystemOrders.MainStart)]
public class GLMainRenderPassStartSystem : RenderSystem
{
    public override int SystemPosition => SystemOrders.MainStart;

    public GLMainRenderPassStartSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry) : base(
        entityRegistry, componentRegistry)
    {
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        OpenTK.Graphics.OpenGL.GL.BindFramebuffer(FramebufferTarget.Framebuffer, renderContext.MainViewFrameBuffer);
        OpenTK.Graphics.OpenGL.GL.Enable(EnableCap.DepthTest);
        OpenTK.Graphics.OpenGL.GL.Enable(EnableCap.Blend);
        OpenTK.Graphics.OpenGL.GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        OpenTK.Graphics.OpenGL.GL.Enable(EnableCap.LineSmooth);
        OpenTK.Graphics.OpenGL.GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
        OpenTK.Graphics.OpenGL.GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        OpenTK.Graphics.OpenGL.GL.Viewport(0, 0, renderContext.ViewWidth, renderContext.ViewHeight);
        OpenTK.Graphics.OpenGL.GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }
}