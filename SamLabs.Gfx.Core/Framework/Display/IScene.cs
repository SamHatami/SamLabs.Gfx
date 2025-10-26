using System.Drawing;

namespace SamLabs.Gfx.Core.Framework.Display;

public interface IScene
{
    IGrid Grid { get; }
    ICamera Camera { get; }
    List<IRenderable> GetRenderables();
    void AddRenderable(IRenderable renderable);
    void RemoveRenderable(IRenderable renderable);
    void SetGrid(int size, float spacing, Color color, bool showAxis = true, bool showGrid = true);
}