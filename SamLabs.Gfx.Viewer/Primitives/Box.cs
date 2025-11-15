using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Geometry;
using SamLabs.Gfx.Viewer.Display;
using SamLabs.Gfx.Viewer.Geometry;

namespace SamLabs.Gfx.Viewer.Primitives;

public class Box : IRenderable, ISelectable
{
    private GlMesh _mesh;
    private int _shaderProgram;
    private int _baseShaderProgram;

    public int Id { get; } 
    public bool IsSelected { get; set; }
    public Box(int size = 1)
    {
        SetupMesh(size);
        _baseShaderProgram = ShaderManager.GetShaderProgram("base");
        Id = 2;
    }

    private void SetupMesh(int size)
    {
        var vertices = new Vertex[8];
        var indices = new int[36];

        var halfSize = size * 0.5f;
        vertices[0] = new Vertex(new Vector3(halfSize, halfSize, halfSize));
        vertices[1] = new Vertex(new Vector3(halfSize, halfSize, -halfSize));
        vertices[2] = new Vertex(new Vector3(halfSize, -halfSize, -halfSize));
        vertices[3] = new Vertex(new Vector3(halfSize, -halfSize, halfSize));
        vertices[4] = new Vertex(new Vector3(-halfSize, -halfSize, halfSize));
        vertices[5] = new Vertex(new Vector3(-halfSize, -halfSize, -halfSize));
        vertices[6] = new Vertex(new Vector3(-halfSize, halfSize, -halfSize));
        vertices[7] = new Vertex(new Vector3(-halfSize, halfSize, halfSize));


        indices[0] = 0;
        indices[1] = 1;
        indices[2] = 2; // Triangle 1
        indices[3] = 2;
        indices[4] = 3;
        indices[5] = 0; // Triangle 2 

        indices[6] = 4;
        indices[7] = 5;
        indices[8] = 6; // Triangle 3 
        indices[9] = 6;
        indices[10] = 7;
        indices[11] = 4; // Triangle 4 

        indices[12] = 0;
        indices[13] = 3;
        indices[14] = 4; // Triangle 5
        indices[15] = 4;
        indices[16] = 7;
        indices[17] = 0; // Triangle 6 

        indices[18] = 1;
        indices[19] = 6;
        indices[20] = 5; // Triangle 7
        indices[21] = 5;
        indices[22] = 2;
        indices[23] = 1; // Triangle 8

        indices[24] = 7;
        indices[25] = 6;
        indices[26] = 1; // Triangle 9
        indices[27] = 1;
        indices[28] = 0;
        indices[29] = 7; // Triangle 10

        indices[30] = 3;
        indices[31] = 2;
        indices[32] = 5; // Triangle 11
        indices[33] = 5;
        indices[34] = 4;
        indices[35] = 3; // Triangle 12 
        
        _mesh = new GlMesh(vertices, indices);
    }

    public void ApplyShader(string shaderProgram)
    {
        _shaderProgram = ShaderManager.GetShaderProgram(shaderProgram);       
    }

    public void DrawPickingId()
    {
        if(_baseShaderProgram == 0)
            _baseShaderProgram = ShaderManager.GetShaderProgram("base");
        
        GL.UseProgram(_baseShaderProgram);
        
        int uniformLoc = GL.GetUniformLocation(_baseShaderProgram, "objectId");
        GL.Uniform1ui(uniformLoc, (uint)Id);
        _mesh.Draw();
        GL.UseProgram(0);
    }

    public void Draw()
    {
        if(_shaderProgram == 0)
            return;

        GL.UseProgram(_shaderProgram);
        _mesh.Draw();
        GL.UseProgram(0);
    }


}