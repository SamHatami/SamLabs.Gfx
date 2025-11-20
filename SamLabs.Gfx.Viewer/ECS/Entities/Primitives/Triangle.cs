using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Geometry;
using SamLabs.Gfx.Viewer.Geometry;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Entities.Primitives;

public class Triangle : GlMesh, IRenderable

{
    public int Id { get; }
    
    private int _shaderProgram;
    public Triangle(Vertex[] vertices, int[] indices) : base(vertices, indices)
    {
        
    }
    public static Triangle CreateSimpleTriangle()
    {
        var vertices = new Vertex[3];
        
        vertices[0] = new Vertex(new Vector3(-0.5f, -0.5f, 0f));
        vertices[1] = new Vertex(new Vector3(0.5f, -0.5f, 0f));
        vertices[2] = new Vertex(new Vector3(0f, 0.5f, 0f));
        
        return new Triangle(vertices, new int[3] {0, 1, 2});
    }


    public void DrawPickingId()
    {
    }

    public void Draw()
    {
        _shaderProgram = ShaderService.GetShaderProgram("simple");
        GL.UseProgram(_shaderProgram);
        base.Draw();
        GL.UseProgram(0);
    }

    public void Dispose()
    {
        GL.DeleteProgram(_shaderProgram);
    } 
    
}