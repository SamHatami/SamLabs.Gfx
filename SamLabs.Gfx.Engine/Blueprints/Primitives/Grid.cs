using System.Drawing;
using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Engine.Rendering.Abstractions;

namespace SamLabs.Gfx.Engine.Blueprints.Primitives;

public class Grid : IGrid
{
    private readonly int _linesPerSide;
    private readonly float _spacing;
    private int _vao;
    private int _vbo;
    public int Id { get; }

    private int _shaderProgram;

    // vertices as flat float array [x,y,z, x,y,z, ...]
    private float[]? _vertices;

    public int Spacing { get; }
    public int Size { get; }
    public bool ShowAxis { get; }
    public bool ShowGrid { get; }
    public Color Color { get; }

    public Grid(int linesPerSide = 40, float spacing = 1.0f)
    {
        _linesPerSide = linesPerSide;
        _spacing = spacing;
    }

    public void ApplyShader(int shaderProgram)
    {
        _shaderProgram = shaderProgram;        
    }
    public void InitializeGL()
    {
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _vertices = GetVertices();
        GL.BindVertexArray(_vao);
        Upload();

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false,
            3 * sizeof(float), 0);

        GL.BindVertexArray(0);
    }

    public void Upload()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsage.StaticDraw);
    }

    public float[] GetVertices()
    {
        if (_vertices != null) return _vertices;

        var half = _linesPerSide * _spacing * 0.5f;
        var count = (_linesPerSide + 1) * 4; // each line 2 vertices, but we count all
        var verts = new List<float>();

        for (var i = 0; i <= _linesPerSide; i++)
        {
            var x = (i * _spacing) - half;
            // line parallel to Z (vary X)
            verts.Add(x);
            verts.Add(0);
            verts.Add(-half);
            verts.Add(x);
            verts.Add(0);
            verts.Add(half);

            var z = (i * _spacing) - half;
            // line parallel to X (vary Z)
            verts.Add(-half);
            verts.Add(0);
            verts.Add(z);
            verts.Add(half);
            verts.Add(0);
            verts.Add(z);
        }

        _vertices = verts.ToArray();
        return _vertices;
    }

    public void DrawPickingId() {}

    public void Draw()
    {
        if (_shaderProgram == 0) return;

        GL.UseProgram(_shaderProgram);
        GL.BindVertexArray(_vao);
        // GL.UniformMatrix4f(_mvpLocation, 1, false, ref model);

        GL.DrawArrays(PrimitiveType.Lines, 0, _vertices.Length / 3);
        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }
    
    



    public void Dispose()
    {
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
    }
}