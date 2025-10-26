
using OpenTK.Graphics.OpenGL;

namespace SamLabs.Gfx.Viewer.Geometry;

public class GlMesh
{
    private readonly float[] _vertices;
    private readonly int[] _indices;
    private readonly int _vao;
    private readonly int _vbo;
    private int _ebo;
    private int _vertexCount;

    public GlMesh(float[] vertices, int[] indices)
    {
        _vertices = vertices;
        _indices = indices;
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsage.StaticDraw );
        
        SetupVertexAttributes();
        IndexVertices();
        
        GL.BindVertexArray(0);
    }

    private void IndexVertices()
    {
        if (_indices != null) {
            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint),
                _indices, BufferUsage.StaticDraw );
            _vertexCount = _indices.Length;
        } else {
            _vertexCount = _vertices.Length / 3; // Assuming vec3 positions
        }
        
        GL.BindVertexArray(0);
    }

    private void SetupVertexAttributes() {
        // Position
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 
            8 * sizeof(float), 0);
        
        // Normal
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false,
            8 * sizeof(float), 3 * sizeof(float));
        
        // TexCoord
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false,
            8 * sizeof(float), 6 * sizeof(float));
    }

    public void Draw()
    {
        GL.BindVertexArray(_vao);
    
        if (_indices != null && _indices.Length > 0)
        {
            GL.DrawElements(PrimitiveType.Triangles, _vertexCount, DrawElementsType.UnsignedInt, 0);
        }
        else
        {
            GL.DrawArrays(PrimitiveType.Triangles, 0, _vertexCount);
        }
    
        GL.BindVertexArray(0);
    }
    public void Dispose() {
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
        if (_ebo != 0) GL.DeleteBuffer(_ebo);
    }
}