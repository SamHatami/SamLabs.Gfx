using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components;

namespace SamLabs.Gfx.Engine.Entities;

public abstract class EntityBlueprint
{
    protected readonly IComponentRegistry _componentRegistry;

    public EntityBlueprint(IComponentRegistry componentRegistry)
    {
        _componentRegistry = componentRegistry;
    }
    public abstract string Name { get; }
    public abstract void Build(Entity entity, MeshDataComponent meshData = default);

    public EntityBlueprint()
    {
        
    }
}