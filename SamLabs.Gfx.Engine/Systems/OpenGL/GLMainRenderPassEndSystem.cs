using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.OpenGL;

[RenderPassAttributes.RenderOrder(SystemOrders.MainEnd)]
public class GLMainRenderPassEndSystem : RenderSystem
{
    public override int SystemPosition => SystemOrders.MainEnd;

    public GLMainRenderPassEndSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry) : base(
        entityRegistry, componentRegistry)
    {
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        OpenTK.Graphics.OpenGL.GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        OpenTK.Graphics.OpenGL.GL.Disable(EnableCap.Blend);
        OpenTK.Graphics.OpenGL.GL.Disable(EnableCap.LineSmooth);
        OpenTK.Graphics.OpenGL.GL.Disable(EnableCap.DepthTest);
        OpenTK.Graphics.OpenGL.GL.BlendFunc(BlendingFactor.One, BlendingFactor.Zero);
    }
}