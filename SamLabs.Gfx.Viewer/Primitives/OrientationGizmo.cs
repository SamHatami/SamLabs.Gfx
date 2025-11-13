using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Viewer.Geometry;

namespace SamLabs.Gfx.Viewer.Primitives;

public class OrientationGizmo 
{
    public int RenderQuadWidth { get; } = 10;
    public int RenderQuadHeight { get; } = 10;


    public OrientationGizmo(int renderQuadWidth, int renderQuadHeight)
    {
        RenderQuadWidth = renderQuadWidth;
        RenderQuadHeight = renderQuadHeight;
    }
    

}