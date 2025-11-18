using System.Collections.Concurrent;
using SamLabs.Gfx.Viewer.Interfaces;

namespace SamLabs.Gfx.Viewer.Scenes;

public class Scene
{
    private List<IRenderable> _renderables = [];
    public IGrid Grid { get; internal set; }
    public ICamera Camera { get; internal set; }

    public List<IRenderable> GetRenderables() => _renderables;
    public ConcurrentQueue<Action> Actions { get; set; } = new();
    public CameraController CameraController { get; set; }

    public void AddRenderable(IRenderable renderable) => _renderables.Add(renderable);

    public void RemoveRenderable(int id)
    { 
        _renderables.RemoveAll(x => x.Id == id);
    }


    public void Clear()
    {
        _renderables.Clear();
    }

    public void Draw()
    {
        _renderables.ForEach(x => x.Draw());
    }
}