using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Transform;

namespace SamLabs.Gfx.Engine.Commands;

public class TransformCommand:Command
{
    private readonly int _entityId;
    private readonly TransformComponent _preChangeTransform;
    private readonly TransformComponent _postChangeTransform;
    private readonly IComponentRegistry _componentRegistry;

    public TransformCommand(int entityId, TransformComponent preChangeTransform, TransformComponent postChangeTransform, IComponentRegistry componentRegistry)
    {
        _entityId = entityId;
        _preChangeTransform = preChangeTransform;
        _postChangeTransform = postChangeTransform;
        _componentRegistry = componentRegistry;
    }
    public override void Execute()
    {
        _componentRegistry.SetComponentToEntity(_postChangeTransform, _entityId);
    }

    public override void Undo()
    {
        _componentRegistry.SetComponentToEntity(_preChangeTransform, _entityId);
    }
}