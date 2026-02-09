using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Components.Sketch;
using SamLabs.Gfx.Engine.Components.Sketch.Geometry;
using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Commands;

public class AddLineSegmentToSketchCommand : Command
{
    private readonly EntityRegistry _entityRegistry;
    private readonly IComponentRegistry _componentRegistry;
    private readonly int _sketchEntityId;
    private readonly LineSegmentComponent _lineSegment;
    private int _lineSegmentEntityId = -1;

    public AddLineSegmentToSketchCommand(
        EntityRegistry entityRegistry,
        IComponentRegistry componentRegistry,
        int sketchEntityId,
        LineSegmentComponent lineSegment)
    {
        _entityRegistry = entityRegistry;
        _componentRegistry = componentRegistry;
        _sketchEntityId = sketchEntityId;
        _lineSegment = lineSegment;
    }

    public override void Execute()
    {
        // Create entity for the line segment
        var lineEntity = _entityRegistry.CreateEntity();
        _lineSegmentEntityId = lineEntity.Id;

        // Add line segment component
        _componentRegistry.SetComponentToEntity(_lineSegment, _lineSegmentEntityId);
        
        // Add flag to trigger GPU data initialization
        _componentRegistry.SetComponentToEntity(new CreateGlLineDataFlag(), _lineSegmentEntityId);

        // Add to sketch's line segment list
        ref var sketch = ref _componentRegistry.GetComponent<SketchComponent>(_sketchEntityId);
        sketch.LineSegmentEntityIds.Add(_lineSegmentEntityId);
    }

    public override void Undo()
    {
        if (_lineSegmentEntityId == -1)
            return;

        // Remove from sketch's line segment list
        ref var sketch = ref _componentRegistry.GetComponent<SketchComponent>(_sketchEntityId);
        sketch.LineSegmentEntityIds.Remove(_lineSegmentEntityId);

        // Remove entity
        _componentRegistry.RemoveEntity(_lineSegmentEntityId);
        _lineSegmentEntityId = -1;
    }
}

