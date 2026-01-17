using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Construction;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Blueprints.Construction;

public class ConstructionPlaneBlueprint : EntityBlueprint
{
    private readonly IComponentRegistry _componentRegistry;

    public ConstructionPlaneBlueprint(IComponentRegistry componentRegistry, EditorEvents editorEvents)
    {
        _componentRegistry = componentRegistry;
    }

    public override string Name => EntityNames.ConstructionPlane;

    public override void Build(Entity entity, MeshDataComponent meshData = default)
    {
        //Do we need to get some parameters to begin with? 
        
        _componentRegistry.SetComponentToEntity(new ConstructionPlaneDataComponent(), entity.Id);
        _componentRegistry.SetComponentToEntity(new CreateConstructionFlag(), entity.Id);
        
        //Create plane mesh data
    }
}
