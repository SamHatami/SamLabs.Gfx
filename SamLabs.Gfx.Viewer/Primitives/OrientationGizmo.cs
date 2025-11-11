using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Viewer.Geometry;

namespace SamLabs.Gfx.Viewer.Primitives;

public class OrientationGizmo : IRenderable, IDisposable
{
    private int _vao;
    private int _vbo;
    private float[] _vertices;


    public OrientationGizmo()
    {
        
    }
    
    public void InitializeGL()
    {
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        // _vertices = GetVertices();

        GL.BindVertexArray(_vao);
        Upload();

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false,
            3 * sizeof(float), 0);

        GL.BindVertexArray(0);
    }
    
    public void Dispose()
    {
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
    }
    
    public void Upload()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsage.StaticDraw);
    }
    
    public void Draw()
    {
        
    }
}