using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Construction;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Blueprints.Construction;

public class ConstructionPointBlueprint : EntityBlueprint
{
    private readonly IComponentRegistry _componentRegistry;

    public ConstructionPointBlueprint(IComponentRegistry componentRegistry)
    {
        _componentRegistry = componentRegistry;
    }

    public override string Name => EntityNames.ConstructionPoint;

    public override void Build(Entity entity, MeshDataComponent meshData = default)
    {
        _componentRegistry.SetComponentToEntity(new ConstructionPointDataComponent(), entity.Id);
        _componentRegistry.SetComponentToEntity(new CreateConstructionFlag(), entity.Id);
    }
}
