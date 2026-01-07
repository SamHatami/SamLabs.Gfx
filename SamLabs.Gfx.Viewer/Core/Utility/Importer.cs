using Assimp;
using OpenTK.Mathematics;
using SamLabs.Gfx.Geometry.Mesh;
using SamLabs.Gfx.Viewer.ECS.Components;
using Face = SamLabs.Gfx.Geometry.Mesh.Face;

namespace SamLabs.Gfx.Viewer.Core.Utility;

public static class ModelLoader
{
    public static async Task<MeshDataComponent> LoadObj(string filePath)
    {
        var importer = new AssimpContext();
        var steps = PostProcessSteps.FlipUVs | // Align texture V-axis with OpenGL
                    PostProcessSteps.GenerateNormals |
                    PostProcessSteps.JoinIdenticalVertices;

        Scene scene;
        try
        {
            scene = await Task.Run(() => importer.ImportFile(filePath, steps));
            if (scene == null || scene.SceneFlags.HasFlag(SceneFlags.Incomplete) || scene.RootNode == null)
            {
                throw new Exception($"Error loading model from {filePath}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        //we are combining all meshes into one, hence "combined", if we want to support multiple mesh, we need to return an array of meshdatacomponents
        var combinedVertices = new List<Vertex>();
        var combinedIndices = new List<int>(); //The EBO for GL_TRIANGLES
        var combinedFaces = new List<Face>();
        var vertexOffset = 0;
        var faceId = 0;
        foreach (var mesh in scene.Meshes)
        {
            //Vertices
            for (var i = 0; i < mesh.VertexCount; i++)
            {
                var aPos = mesh.Vertices[i];
                var position = new Vector3(aPos.X, aPos.Y, aPos.Z);

                var normal = Vector3.Zero;
                if (mesh.HasNormals)
                {
                    var aNorm = mesh.Normals[i];
                    normal = new Vector3(aNorm.X, aNorm.Y, aNorm.Z);
                }

                var uv = Vector2.Zero;
                if (mesh.HasTextureCoords(0))
                {
                    var aUv = mesh.TextureCoordinateChannels[0][i];
                    uv = new Vector2(aUv.X, aUv.Y);
                }

                combinedVertices.Add(new Vertex(position, normal, uv));
            }


            //Faces
            for (int i = 0; i < mesh.FaceCount; i++)
            {
                var surface = mesh.Faces[i];
                var localSurfaceIndices = surface.Indices.ToArray();

                var globalIndices = new int[localSurfaceIndices.Length];
                for (int j = 0; j < localSurfaceIndices.Length; j++)
                {
                    globalIndices[j] = localSurfaceIndices[j] + vertexOffset;
                }

                var triangulatedIndices = MeshUtils.TriangulateFace(globalIndices);
                combinedIndices.AddRange(triangulatedIndices);
                combinedFaces.Add(new Face()
                {
                    Id = faceId++,
                    VertexIndices = globalIndices,
                    RenderIndices = triangulatedIndices,
                    Normal = MeshUtils.CalculateFaceNormal(combinedVertices, globalIndices),
                    CenterPoint = MeshUtils.CalculateCenter(combinedVertices, globalIndices),
                });
            }

            vertexOffset += mesh.VertexCount; //Since we are combining multiple meshes into one
        }

        var edges = MeshUtils.GenerateEdges(combinedFaces.ToArray());
        return new MeshDataComponent
        {
            Name = Path.GetFileNameWithoutExtension(filePath),
            Vertices = combinedVertices.ToArray(),
            Faces = combinedFaces.ToArray(),
            Edges = edges.ToArray(),
            TriangleIndices = combinedIndices.ToArray(),
            EdgeIndices = edges.SelectMany(e => new[] { e.V2, e.V1 }).ToArray()
        };
    }
}