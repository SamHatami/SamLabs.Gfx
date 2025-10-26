using System.Drawing;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Viewer.Primitives;

namespace SamLabs.Gfx.Viewer;

public class Scene : IScene
{
    private List<IRenderable> _renderables = [];

    public IGrid Grid { get; internal set; }
    public ICamera Camera { get; internal set; }

    public List<IRenderable> GetRenderables() => _renderables;

    public void AddRenderable(IRenderable renderable) => _renderables.Add(renderable);

    public void RemoveRenderable(IRenderable renderable) => _renderables.Remove(renderable);
    
    public void Clear() => _renderables.Clear();
    public void Draw() => _renderables.ForEach(x => x.Draw());
    public void SetGrid(int size, float spacing, Color color, bool showAxis = true, bool showGrid = true) =>
        Grid = new Grid();
}