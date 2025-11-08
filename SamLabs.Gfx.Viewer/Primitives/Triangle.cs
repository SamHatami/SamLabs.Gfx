using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Geometry;
using SamLabs.Gfx.Viewer.Framework;
using SamLabs.Gfx.Viewer.Geometry;

namespace SamLabs.Gfx.Viewer.Primitives;

public class Triangle : GlMesh, IRenderable
{
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

    public void Draw(Matrix4 viewMatrix, Matrix4 projectionMatrix)
    {
        Draw();
    }
    
    public void Draw()
    {
        _shaderProgram = ShaderManager.GetShaderProgram("flat");
        GL.UseProgram(_shaderProgram);
        base.Draw();
    }

    public void Dispose()
    {
        GL.DeleteProgram(_shaderProgram);
    } 
    
}