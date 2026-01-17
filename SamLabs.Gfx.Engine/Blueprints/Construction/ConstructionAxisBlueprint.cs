using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Construction;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Blueprints.Construction;

public class ConstructionAxisBlueprint : EntityBlueprint
{
    private readonly IComponentRegistry _componentRegistry;

    public ConstructionAxisBlueprint(IComponentRegistry componentRegistry)
    {
        _componentRegistry = componentRegistry;
    }

    public override string Name => EntityNames.ConstructionAxis;

    public override void Build(Entity entity, MeshDataComponent meshData = default)
    {
        _componentRegistry.SetComponentToEntity(new ConstructionAxisDataComponent(), entity.Id);
        _componentRegistry.SetComponentToEntity(new CreateConstructionFlag(), entity.Id);
        
        //Send out RequestUserInputEvent, so that the user can start dragging the axis. and await for a callback
        //If the blueprints has some graphics to show, we can use the same entity for the graphics, but we don't
        //add/remove it until the user accepts or rejects the construction.
    }
}
