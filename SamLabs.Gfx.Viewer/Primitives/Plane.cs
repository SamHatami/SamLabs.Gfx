using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.Geometry;

namespace SamLabs.Gfx.Viewer.Primitives;

public class Plane
{
    private readonly GlMesh _mesh;
    
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
    
    private (float[] vertices, int[] indices) GeneratePlaneData(float width, float depth, int subdivisions)
    {
        var verticesPerSide = subdivisions + 1;
        var totalVertices = verticesPerSide * verticesPerSide;
        
        // 8 floats per vertex: position(3) + normal(3) + texcoord(2)
        var vertices = new float[totalVertices * 8];
        
        var halfWidth = width * 0.5f;
        var halfDepth = depth * 0.5f;
        
        int vertexIndex = 0;
        for (int z = 0; z < verticesPerSide; z++)
        {
            for (int x = 0; x < verticesPerSide; x++)
            {
                float xPos = -halfWidth + (x / (float)subdivisions) * width;
                float zPos = -halfDepth + (z / (float)subdivisions) * depth;
                float u = x / (float)subdivisions;
                float v = z / (float)subdivisions;
                
                // Position
                vertices[vertexIndex++] = xPos;
                vertices[vertexIndex++] = 0f;
                vertices[vertexIndex++] = zPos;
                
                // Normal (pointing up)
                vertices[vertexIndex++] = 0f;
                vertices[vertexIndex++] = 1f;
                vertices[vertexIndex++] = 0f;
                
                // TexCoord
                vertices[vertexIndex++] = u;
                vertices[vertexIndex++] = v;
            }
        }
        
        // Generate indices (two triangles per quad)
        var numQuads = subdivisions * subdivisions;
        var indices = new int[numQuads * 6];
        
        int indexArrayIndex = 0;
        for (int z = 0; z < subdivisions; z++)
        {
            for (int x = 0; x < subdivisions; x++)
            {
                int topLeft = z * verticesPerSide + x;
                int topRight = topLeft + 1;
                int bottomLeft = (z + 1) * verticesPerSide + x;
                int bottomRight = bottomLeft + 1;
                
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
       _mesh.Draw();
    }
    
    public void Dispose()
    {
        _mesh.Dispose();
    }
}