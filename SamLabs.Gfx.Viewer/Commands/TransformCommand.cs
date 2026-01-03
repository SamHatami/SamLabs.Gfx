using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.Commands;

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
        ComponentManager.SetComponentToEntity(_postChangeTransform, _entityId);
    }

    public override void Undo()
    {
        ComponentManager.SetComponentToEntity(_preChangeTransform, _entityId);
    }
}