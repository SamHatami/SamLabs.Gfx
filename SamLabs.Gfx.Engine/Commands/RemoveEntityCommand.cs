using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Commands;

public class RemoveEntityCommand : Command
{
    private readonly EntityRegistry _entityRegistry;
    private readonly int _entityId;

    public RemoveEntityCommand(EntityRegistry entityRegistry, int entityId)
    {
        _entityRegistry = entityRegistry;
        _entityId = entityId;
    }

    public override void Execute()
    {
        _entityRegistry.Remove(_entityId);
    }

    public override void Undo()
    {
        // Redoing creation is complex if we don't store components. 
        // For now, let's keep it simple as per existing patterns.
    }
}
