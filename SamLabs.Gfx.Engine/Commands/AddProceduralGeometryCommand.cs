using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Commands;

public class AddProceduralGeometryCommand : Command
{
    private readonly CommandManager _commandManager;
    private readonly EntityFactory _entityFactory;
    private readonly string _blueprintName;
    private int _geometryId;

    public AddProceduralGeometryCommand(CommandManager commandManager, EntityFactory entityFactory, string blueprintName)
    {
        _commandManager = commandManager;
        _entityFactory = entityFactory;
        _blueprintName = blueprintName;
    }

    public override void Execute()
    {
        var entity = _entityFactory.CreateFromBlueprint(_blueprintName);
        if (entity.HasValue)
            _geometryId = entity.Value.Id;
    }

    public override void Undo() => _commandManager.EnqueueCommand();
}
