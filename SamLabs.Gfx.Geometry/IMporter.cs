using Assimp;

namespace SamLabs.Gfx.Geometry;

public class IMporter
{
    public void Import()
    {
        using (var importer = new AssimpContext())
        {
            // 2. Read the file, optionally applying post-processing flags
            Scene scene = importer.ImportFile(
                "path/to/your/model.obj", 
                PostProcessSteps.Triangulate | 
                PostProcessSteps.GenerateSmoothNormals | 
                PostProcessSteps.JoinIdenticalVertices
            );

            // 3. Process the data
            if (scene != null && scene.HasMeshes)
            {
                
                // Iterate through scene.Meshes, scene.Materials, etc.,
                // and upload the vertex data to your GPU buffers.
            }
        }  
    }
}