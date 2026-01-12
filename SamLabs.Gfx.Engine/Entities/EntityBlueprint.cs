using SamLabs.Gfx.Engine.Components.Common;

namespace SamLabs.Gfx.Engine.Entities;

public abstract class EntityBlueprint
{
    public abstract string Name { get; }
    public abstract void Build(Entity entity, MeshDataComponent meshData = default);

    public EntityBlueprint()
    {
        
    }
}