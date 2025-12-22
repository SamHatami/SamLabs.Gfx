using Assimp;
using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Managers;


public static class ModelLoader
{
    public static async Task<MeshDataComponent> LoadObj(string filePath)
    {
        var importer = new AssimpContext();
        var steps = PostProcessSteps.Triangulate |    
                    // Ensure all faces are triangles
                    PostProcessSteps.FlipUVs |               // Align texture V-axis with OpenGL
                       // Create normals if missing
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




        var combinedVertices = new List<Vertex>();
        var combinedIndices = new List<int>();
        int vertexOffset = 0;

        foreach (var mesh in scene.Meshes)
        {
            for (int i = 0; i < mesh.VertexCount; i++)
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

            var meshIndices = mesh.GetIndices().ToArray();
            for (int i = 0; i < meshIndices.Count(); i++)
            {
                combinedIndices.Add(meshIndices[i] + vertexOffset);
            }

            vertexOffset += mesh.VertexCount;
        }

        return new MeshDataComponent
        {
            Vertices = combinedVertices.ToArray(),
            Indices = combinedIndices.ToArray(),
            Name = Path.GetFileNameWithoutExtension(filePath)
        };
    }
}