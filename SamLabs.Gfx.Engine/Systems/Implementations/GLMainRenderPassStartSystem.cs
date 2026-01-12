using System.ComponentModel;
using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Implementations;

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
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, renderContext.MainViewFrameBuffer);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.LineSmooth);
        GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Viewport(0, 0, renderContext.ViewWidth, renderContext.ViewHeight);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }
}