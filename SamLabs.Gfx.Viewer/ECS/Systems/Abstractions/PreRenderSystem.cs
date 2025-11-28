using SamLabs.Gfx.Viewer.ECS.Interfaces;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.Rendering.Abstractions;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;

public abstract class PreRenderSystem : ISystem
{
    private readonly ComponentManager _componentManager;

    protected PreRenderSystem(ComponentManager componentManager)
    {
        _componentManager = componentManager;
    }
    
    public void Initialize(IRenderer renderer) { } // No initialization required (yet)

    public abstract void Update();
}