using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;

namespace SamLabs.Gfx.Engine.Commands;

public class TransformCommand:Command
{
    private readonly int _entityId;
    private readonly TransformComponent _preChangeTransform;
    private readonly TransformComponent _postChangeTransform;

    public TransformCommand(int entityId, TransformComponent preChangeTransform, TransformComponent postChangeTransform)
    {
        _entityId = entityId;
        _preChangeTransform = preChangeTransform;
        _postChangeTransform = postChangeTransform;
    }
    public override void Execute()
    {
        ComponentRegistry.SetComponentToEntity(_postChangeTransform, _entityId);
    }

    public override void Undo()
    {
        ComponentRegistry.SetComponentToEntity(_preChangeTransform, _entityId);
    }
}