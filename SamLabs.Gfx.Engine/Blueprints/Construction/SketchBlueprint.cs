using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Sketch;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Blueprints.Construction;

public class SketchBlueprint : EntityBlueprint
{
    private readonly IComponentRegistry _componentRegistry;

    public SketchBlueprint(IComponentRegistry componentRegistry)
    {
        _componentRegistry = componentRegistry;
    }

    public override string Name => EntityNames.Sketch;

    public override void Build(Entity entity, MeshDataComponent meshData = default)
    {
        _componentRegistry.SetComponentToEntity(new SketchComponent(), entity.Id);
        _componentRegistry.SetComponentToEntity(new CreateSketchFlag(), entity.Id);
    }
}
