using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Geometry.Mesh;

namespace SamLabs.Gfx.Engine.Core.Utility;

public static class MeshCreator
{
    public static MeshDataComponent CreatePlane(float size)
    {
        return CreatePlane(size, size);
    }
    public static MeshDataComponent CreatePlane(float width = 1.0f, float height = 1.0f, string name = "Plane")
    {
        var hw = width / 2.0f;
        var hh = height / 2.0f;
        var normal = Vector3.UnitZ;
        var centerPoint = new Vector3(0, 0, 0);

        var vertices = new Vertex[]
        {
            new(new Vector3(-hw, -hh, 0), normal, new Vector2(0.0f, 0.0f)), // Bottom Left
            new(new Vector3(hw, -hh, 0), normal, new Vector2(1.0f, 0.0f)), // Top left
            new(new Vector3(hw, hh, 0), normal, new Vector2(1.0f, 1.0f)), // Top right
            new(new Vector3(-hw, hh, 0), normal, new Vector2(0.0f, 1.0f)) // Bottom right
        };

        var edges = new Edge[]
        {
            new Edge(0, 1, 1), // Bottom
            new Edge(1, 2, 2), // Right
            new Edge(2, 3, 3), // Top
            new Edge(3, 0, 4) // Left
        };

        var face = new Face()
        {
            Id = 0,
            Normal = normal,
            CenterPoint = centerPoint,
            VertexIndices = [0, 1, 2, 3],
            RenderIndices = [0, 1, 2, 2, 3, 0]
        };

        var triangleIndices = new int[]
        {
            0, 1, 2,
            0, 2, 3
        };

        var edgeIndices = new int[]
        {
            0, 1, // Bottom
            1, 2, // Right
            2, 3, // Top
            3, 0 // Left
        };

        return new MeshDataComponent
        {
            Vertices = vertices,
            Edges = edges,
            Faces = [face],
            TriangleIndices = triangleIndices,
            EdgeIndices = edgeIndices,
            Name = name
        };
    }
}