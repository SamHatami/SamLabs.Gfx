using OpenTK.Mathematics;

namespace SamLabs.Gfx.Geometry.Mesh;

public struct Face
{
    public int Id { get; set; }
    public int[] VertexIndices { get; set; }
    public int[] RenderIndices { get; set; } //triangulated face [v1,v2,v3] etc
    public Vector3 Normal { get; set; }
    public Vector3 CenterPoint { get; set; } //default manipulator position

    
    public Face(int id, int[] vertexIndices, Vector3 normal, Vector3 centerPoint)
    {
        Id = id;
        VertexIndices = vertexIndices;
        Normal = normal;
        CenterPoint = centerPoint;
    }

    
}