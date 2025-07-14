using System.Numerics;

namespace CPURendering.Geometry;

public struct Face
{
    public int[] VertIndices;
    public int[] VertTextureIndices;
    public int[] VertNormalIndices;
    public Vector3 Normal;

    public Face(int[] vertIndices, int[] vertTextureIndices, int[] vertNormalIndices)
    {
        VertIndices = vertIndices;
        VertTextureIndices = vertTextureIndices;
        VertNormalIndices = vertNormalIndices;
        
        //Face Normal
    }
}