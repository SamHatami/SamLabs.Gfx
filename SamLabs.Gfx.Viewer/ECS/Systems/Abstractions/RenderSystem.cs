using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;
using SamLabs.Gfx.Viewer.Rendering.Passes;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;

public abstract class RenderSystem
{
    protected readonly ComponentManager _componentManager;
    protected IRenderer _renderer;

    public RenderSystem(ComponentManager componentManager)
    {
        _componentManager = componentManager;
    }
    public void Initialize(IRenderer renderer)
    {
        _renderer = renderer;
    }
    
    public abstract void Update(RenderContext renderContext);
}