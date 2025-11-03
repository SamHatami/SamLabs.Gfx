using OpenTK.Mathematics;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Viewer.Geometry;

namespace SamLabs.Gfx.Viewer.Primitives;

public class Box : IRenderable
{
    private GlMesh _mesh;
    
    public Box(int size = 1)
    {
        SetupMesh(size);
    }

    private void SetupMesh(int size)
    {
        
    }

    public void Draw()
    {
        
    }

    public void Draw(Matrix4 viewMatrix, Matrix4 projectionMatrix)
    {
    }
}