using SamLabs.Gfx.Engine.Components.Common;

namespace SamLabs.Gfx.Engine.Rendering.Engine;

[Obsolete("MeshRenderer is superseded by IGraphicsBackend.DrawMesh and should not be used.")]
public static class MeshRenderer
{
    public static void Draw(in GlMeshDataComponent mesh)
    {
    }
}
