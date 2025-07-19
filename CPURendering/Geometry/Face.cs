using System.Numerics;

namespace CPURendering.Geometry;

public struct Face //this is a triangular face for now
{
    public int[] VertIndices;
    public int[] VertTextureIndices;
    public int[] VertNormalIndices;
    public Vector3 Normal;

    public Face(int[] vertIndices, int[] vertTextureIndices, int[] vertNormalIndices, Vector3 normal)
    {
        VertIndices = vertIndices;
        VertTextureIndices = vertTextureIndices;
        VertNormalIndices = vertNormalIndices;
        
        Normal = normal;
    }
}