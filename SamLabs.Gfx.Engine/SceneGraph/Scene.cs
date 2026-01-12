using SamLabs.Gfx.Engine.Rendering.Abstractions;

namespace SamLabs.Gfx.Engine.SceneGraph;

public class Scene
{
    private List<IRenderable> _renderables = [];
    public IGrid Grid { get; internal set; }
    public ICamera Camera { get; internal set; }

    public List<IRenderable> GetRenderables() => _renderables;

    public void AddRenderable(IRenderable renderable) => _renderables.Add(renderable);

    public void RemoveRenderable(int id)
    { 
        _renderables.RemoveAll(x => x.Id == id);
    }

    public void Update()
    {
        //Update the scene, requests all the sceneobjectentities from the componentmanager, build up the hierarchy from the parent components
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