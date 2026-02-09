﻿﻿using SamLabs.Gfx.Engine.Components;
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
        
        // Copy the plane data from the construction plane to the sketch entity
        if (_componentRegistry.HasComponent<PlaneDataComponent>(_constructionPlaneEntityId))
        {
            var planeData = _componentRegistry.GetComponent<PlaneDataComponent>(_constructionPlaneEntityId);
            _componentRegistry.SetComponentToEntity(planeData, _sketchEntityId);
        }
        
        // Hide the construction plane during sketch mode
        if (_componentRegistry.HasComponent<VisibilityComponent>(_constructionPlaneEntityId))
        {
            ref var visibility = ref _componentRegistry.GetComponent<VisibilityComponent>(_constructionPlaneEntityId);
            visibility.IsVisible = false;
        }
        
        // Set the reference to the construction plane
        ref var sketchComponent = ref _componentRegistry.GetComponent<SketchComponent>(_sketchEntityId);
        sketchComponent.ConstructionPlaneEntityId = _constructionPlaneEntityId;
    }

    public override void Undo()
    {
        // Show the plane again
        if (_componentRegistry.HasComponent<VisibilityComponent>(_constructionPlaneEntityId))
        {
            ref var visibility = ref _componentRegistry.GetComponent<VisibilityComponent>(_constructionPlaneEntityId);
            visibility.IsVisible = true;
        }
        
        _componentRegistry.RemoveEntity(_sketchEntityId);
    }
}
