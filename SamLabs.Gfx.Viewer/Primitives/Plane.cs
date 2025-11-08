using OpenTK.Graphics.OpenGLES2;
using OpenTK.Mathematics;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Geometry;
using SamLabs.Gfx.Viewer.Framework;
using SamLabs.Gfx.Viewer.Geometry;

namespace SamLabs.Gfx.Viewer.Primitives;

public class Plane: IRenderable
{
    private readonly GlMesh _mesh;
    private int _shaderProgram;
    /// <summary>
    /// Creates a plane on the XZ plane (Y=0) centered at origin
    /// </summary>
    /// <param name="width">Width along X axis</param>
    /// <param name="depth">Depth along Z axis</param>
    /// <param name="subdivisions">Number of subdivisions per side (min 1)</param>
    public Plane(float width = 10f, float depth = 10f, int subdivisions = 1)
    {
        subdivisions = Math.Max(1, subdivisions);
        
        var (vertices, indices) = GeneratePlaneData(width, depth, subdivisions);
        _mesh = new GlMesh(vertices, indices);
    }
    
    private (Vertex[] vertices, int[] indices) GeneratePlaneData(float width, float depth, int subdivisions)
    {
        var verticesPerSide = subdivisions + 1;
        var totalVertices = verticesPerSide * verticesPerSide;
        
        // 8 floats per vertex: position(3) + normal(3) + texcoord(2)
        var vertices = new Vertex[totalVertices];
        
        var halfWidth = width * 0.5f;
        var halfDepth = depth * 0.5f;
        
        var vertexIndex = 0;
        for (var z = 0; z < verticesPerSide; z++)
        {
            for (var x = 0; x < verticesPerSide; x++)
            {
                var xPos = -halfWidth + (x / (float)subdivisions) * width;
                var zPos = -halfDepth + (z / (float)subdivisions) * depth;
                var u = x / (float)subdivisions;
                var v = z / (float)subdivisions;
                
                var vertexPosition = new Vector3(xPos, 0f, zPos);
                var normal = Vector3.UnitY;
                var texCoord = new Vector2(u, v);
                
                vertices[vertexIndex++] = new Vertex(vertexPosition, normal, texCoord);
            }
        }
        
        // Generate indices (two triangles per quad)
        var numQuads = subdivisions * subdivisions;
        var indices = new int[numQuads * 6];
        
        var indexArrayIndex = 0;
        for (var z = 0; z < subdivisions; z++)
        {
            for (var x = 0; x < subdivisions; x++)
            {
                var topLeft = z * verticesPerSide + x;
                var topRight = topLeft + 1;
                var bottomLeft = (z + 1) * verticesPerSide + x;
                var bottomRight = bottomLeft + 1;
                
                // First triangle (counter-clockwise when viewed from above)
                indices[indexArrayIndex++] = topLeft;
                indices[indexArrayIndex++] = bottomLeft;
                indices[indexArrayIndex++] = topRight;
                
                // Second triangle
                indices[indexArrayIndex++] = topRight;
                indices[indexArrayIndex++] = bottomLeft;
                indices[indexArrayIndex++] = bottomRight;
            }
        }
        
        return (vertices, indices);
    }
    
    public void Draw()
    {
        _shaderProgram = ShaderManager.GetShaderProgram("flat");
        GL.UseProgram(_shaderProgram);
       _mesh.Draw();
       GL.UseProgram(0);
    }

    public void Draw(Matrix4 viewMatrix, Matrix4 projectionMatrix)
    {
        Draw();       
    }

    public void Dispose()
    {
        GL.DeleteProgram(_shaderProgram);
        _mesh.Dispose();
    }
}