using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Entities;

public abstract class EntityBlueprint
{
    public abstract string Name { get; }
    public abstract void Build(Entity entity);

    public EntityBlueprint(ComponentManager componentManager)
    {
        
    }
}