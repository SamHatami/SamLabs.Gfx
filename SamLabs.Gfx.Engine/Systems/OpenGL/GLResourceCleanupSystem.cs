using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.OpenGL;

[RenderPassAttributes.RenderOrder(SystemOrders.CleanUp)]
public class GLResourceCleanupSystem : RenderSystem
{
    public override int SystemPosition => SystemOrders.CleanUp;

    public GLResourceCleanupSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry) : base(entityRegistry, componentRegistry)
    {
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        // Mesh GPU cleanup now handled centrally in MeshUploadSystem via IGraphicsBackend.
    }
}
