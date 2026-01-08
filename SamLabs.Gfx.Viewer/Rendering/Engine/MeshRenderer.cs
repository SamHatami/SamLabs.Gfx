using SamLabs.Gfx.Viewer.ECS.Components;
using OpenTK.Graphics.OpenGL;

namespace SamLabs.Gfx.Viewer.Rendering.Engine
{
    public static class MeshRenderer
    {
        
        public static MeshRenderContext Begin(in GlMeshDataComponent mesh)
        {
            return new MeshRenderContext(mesh);
        }
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
    
    public ref struct MeshRenderContext
    {
        private readonly GlMeshDataComponent _mesh;

        public MeshRenderContext(in GlMeshDataComponent mesh)
        {
            _mesh = mesh;
            GL.BindVertexArray(mesh.Vao);
        }

        public void Dispose()
        {
            GL.BindVertexArray(0);
        }

        // Pass 1: Faces
        public MeshRenderContext Faces()
        {
            if (_mesh.Ebo > 0)
            {
                // Ensure we bind the Face EBO
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _mesh.Ebo);
                GL.DrawElements(PrimitiveType.Triangles, _mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
            }
            return this;
        }

        // Pass 2: Edges
        public MeshRenderContext Edges()
        {
            if (_mesh.EdgeEbo > 0)
            {
                // Bind Edge EBO
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _mesh.EdgeEbo);
                GL.DrawElements(PrimitiveType.Lines, _mesh.EdgeIndexCount, DrawElementsType.UnsignedInt, 0);
            }
            return this;
        }

        // Pass 3: Vertices
        public MeshRenderContext Vertices(float size = 5.0f)
        {
            GL.PointSize(size);
            GL.DrawArrays(PrimitiveType.Points, 0, _mesh.VertexCount);
            GL.PointSize(1.0f);
            return this;
        }
    }
}