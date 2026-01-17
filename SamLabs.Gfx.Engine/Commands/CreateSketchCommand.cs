using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Construction;
using SamLabs.Gfx.Engine.Components.Sketch;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Commands;

public class CreateSketchCommand : Command
{
    private readonly EntityFactory _entityFactory;
    private readonly IComponentRegistry _componentRegistry;
    private readonly int _constructionPlaneEntityId;
    private int _sketchEntityId = -1;

    public CreateSketchCommand(EntityFactory entityFactory, IComponentRegistry componentRegistry, int constructionPlaneEntityId)
    {
        _entityFactory = entityFactory;
        _componentRegistry = componentRegistry;
        _constructionPlaneEntityId = constructionPlaneEntityId;
    }

    public override void Execute()
    {
        if (_sketchEntityId == -1)
        {
            var sketchEntity = _entityFactory.CreateFromBlueprint(EntityNames.Sketch);
            if (sketchEntity.HasValue)
                _sketchEntityId = sketchEntity.Value.Id;
        }
        
        _componentRegistry.SetComponentToEntity(new CreateSketchRequest 
        { 
            ConstructionPlaneEntityId = _constructionPlaneEntityId 
        }, _sketchEntityId);
    }

    public override void Undo()
    {
        // Unlink plane
        if (_componentRegistry.HasComponent<PlaneDataComponent>(_constructionPlaneEntityId))
        {
            ref var planeData = ref _componentRegistry.GetComponent<PlaneDataComponent>(_constructionPlaneEntityId);
        }
        
        _componentRegistry.RemoveEntity(_sketchEntityId);
    }
}
