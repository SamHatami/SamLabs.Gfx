using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Commands;

public class AddMemberElementCommand : ICommand
{
    private readonly CommandManager _commandManager;
    private readonly EntityFactory _entityFactory;
    private int _memberId;

    public AddMemberElementCommand(CommandManager commandManager, EntityFactory entityFactory)
    {
        _commandManager = commandManager;
        _entityFactory = entityFactory;
    }

    public void Execute()
    {
        var memberEntity = _entityFactory.CreateFromBlueprint(EntityNames.MemberElement);
        if (memberEntity.HasValue)
            _memberId = memberEntity.Value.Id;
    }

    public void Undo()
    {
        _commandManager.EnqueueCommand();
    }

    public void Redo()
    {
    }

    public bool Internal { get; set; }
}
