using SamLabs.Gfx.Viewer.ECS.Components;
using OpenTK.Graphics.OpenGL;

namespace SamLabs.Gfx.Viewer.Rendering.Engine
{
    public static class MeshRenderer
    {
        public static void Draw(in GlMeshDataComponent mesh)
        {
            using (new VAOHandle(mesh.Vao))
            {
                if (mesh.Ebo > 0)
                    GL.DrawElements(mesh.PrimitiveType, mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
                else
                    GL.DrawArrays(mesh.PrimitiveType, 0, mesh.VertexCount);
            }
        }
    }
}