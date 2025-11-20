using Assimp;
using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Geometry;

namespace SamLabs.Gfx.Viewer.Geometry;


//Will become obsolete when its moved to a component
[Obsolete]
public class GlMesh : Mesh, IDisposable
{
    private readonly int _vao;
    private readonly int _vbo;
    private int _ebo;
    private int _vertexCount;

    public GlMesh(Vertex[] vertices, int[] indices):base(vertices, indices)
    {
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * Vertex.SizeOf, Vertices, BufferUsage.StaticDraw );
        
        SetupVertexAttributes();
        IndexVertices();
        
        GL.BindVertexArray(0);
    }

    private void IndexVertices()
    {
        if (Indices != null) 
        {
            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint),
                Indices, BufferUsage.StaticDraw );
            _vertexCount = Indices.Length;
        } 
        else 
        {
            _vertexCount = Vertices.Length; // Assuming vec3 positions
        }
        
    }

    private void SetupVertexAttributes() {
        // Position
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 
            Vertex.SizeOf, 0);
        
        // Normal
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false,
            Vertex.SizeOf, 3 * sizeof(float));
        
        // TexCoord
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false,
            Vertex.SizeOf, 6 * sizeof(float));
    }

    public void Draw()
    {
        GL.BindVertexArray(_vao);
    
        if (Indices != null && Indices.Length > 0)
        {
            GL.DrawElements(OpenTK.Graphics.OpenGL.PrimitiveType.Triangles, _vertexCount, DrawElementsType.UnsignedInt, 0);
        }
        else
        {
            GL.DrawArrays(OpenTK.Graphics.OpenGL.PrimitiveType.Triangles, 0, _vertexCount);
        }
    
        GL.BindVertexArray(0);
    }
    public void Dispose() {
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
        if (_ebo != 0) GL.DeleteBuffer(_ebo);
    }
}