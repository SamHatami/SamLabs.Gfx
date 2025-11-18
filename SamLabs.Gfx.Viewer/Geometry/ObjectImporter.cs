using SamLabs.Gfx.Viewer.Interfaces;

namespace SamLabs.Gfx.Viewer.Geometry;

public class ObjectImporter
{
    public async Task<IRenderable?> Import(string filePath)
    {
        var extension = Path.GetExtension(filePath);

        switch (extension)
        {
            case ".obj":
                await ImportObj(filePath);
                break;
            case ".ply":
                await ImportPly(filePath);
                break;
            case ".stl":
                await ImportStl(filePath);
                break;
        }
        
        return null;
    }

    public async Task ImportObj(string filePath)
    {
        await Task.Delay(1000);
    }

    public async Task ImportPly(string filePath)
    {
    }

    public async Task ImportStl(string filePath)
    {
    }
}