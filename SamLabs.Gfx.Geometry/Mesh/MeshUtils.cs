using OpenTK.Mathematics;

namespace SamLabs.Gfx.Geometry.Mesh;

public static class MeshUtils
{
    
    //Newells method for computing the plane equation
    //one implementation from
    //https://every-algorithm.github.io/2024/03/06/newells_algorithm.html#:~:text=Newell%E2%80%99s%20algorithm%20is%20a%20straightforward%20method%20for%20computing,correspond%20to%20the%20components%20of%20the%20desired%20normal.
    public static Vector3 CalculateFaceNormal(List<Vertex> meshVertices, int[] faceIndices)
    {
        // 1. If it's a simple triangle, the cross product is faster and exact.
        if (faceIndices.Length == 3)
        {
            var p0 = meshVertices[faceIndices[0]].Position;
            var p1 = meshVertices[faceIndices[1]].Position;
            var p2 = meshVertices[faceIndices[2]].Position;
            return Vector3.Normalize(Vector3.Cross(p1 - p0, p2 - p0));
        }

        // 2. For Quads and N-Gons, use Newell's Method.
        // This handles "bent" faces by averaging the normal across all edges.
        float x = 0;
        float y = 0;
        float z = 0;

        for (var i = 0; i < faceIndices.Length; i++)
        {
            var current = meshVertices[faceIndices[i]].Position;
            var next = meshVertices[faceIndices[(i + 1) % faceIndices.Length]].Position;

            x += (current.Y - next.Y) * (current.Z + next.Z);
            y += (current.Z - next.Z) * (current.X + next.X);
            z += (current.X - next.X) * (current.Y + next.Y);
        }

        return Vector3.Normalize(new Vector3(x, y, z));
    }
    
    public static Vector3 CalculateCenter(List<Vertex> allVertices, int[] faceIndices)
    {
        var sum = Vector3.Zero;
        foreach (var idx in faceIndices)
        {
            sum += allVertices[idx].Position;
        }
        return sum / faceIndices.Length;
    }

    public static Edge[] GenerateEdges(Face[] faces)
    {
        var uniqueEdges = new HashSet<(int, int)>();
        var edgeList = new List<Edge>();
        var edgeId = 0;

        foreach (var face in faces)
        {
            var indices = face.VertexIndices;
            for (var i = 0; i < indices.Length; i++)
            {
                var a = indices[i];
                var b = indices[(i + 1) % indices.Length];

                // Sort to ensure uniqueness (Edge 0-1 == Edge 1-0)
                var key = a < b ? (a, b) : (b, a);

                if (uniqueEdges.Add(key))
                    edgeList.Add(new Edge(edgeId++, key.Item1, key.Item2));
            }
        }
        return edgeList.ToArray();
    }
    
    public static int[] TriangulateFace(int[] polygonIndices)
    {
        // 1. If it's already a triangle, just return it.
        if (polygonIndices.Length == 3) return polygonIndices;

        // 2. If it's a Quad (4) or N-Gon (5+), split it into triangles.
        // We use a "Triangle Fan" approach anchored at Index 0.
        // N-Gon with V vertices = (V-2) Triangles.
    
        var triangleIndices = new List<int>();
    
        for (int i = 0; i < polygonIndices.Length - 2; i++)
        {
            // Triangle is: Anchor, Current, Next
            triangleIndices.Add(polygonIndices[0]);
            triangleIndices.Add(polygonIndices[i + 1]);
            triangleIndices.Add(polygonIndices[i + 2]);
        }

        return triangleIndices.ToArray();
    }
}