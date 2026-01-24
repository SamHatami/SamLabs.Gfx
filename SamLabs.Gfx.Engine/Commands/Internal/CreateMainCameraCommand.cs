using SamLabs.Gfx.Engine.Entities;

namespace SamLabs.Gfx.Engine.Commands.Internal;

public class CreateMainCameraCommand : InternalCommand
{
    private readonly CommandManager _commandManager;
    private readonly EntityFactory _entityFactory;
    private int _cameraId;

    public CreateMainCameraCommand(CommandManager commandManager, EntityFactory entityFactory)
    {
        _commandManager = commandManager;
        _entityFactory = entityFactory;
    }

    public override void Execute()
    {
        Task.Run(async () =>
        {
            var cameraEntity = _entityFactory.CreateFromBlueprint(EntityNames.MainCamera);
            if (cameraEntity.HasValue)
                _cameraId = cameraEntity.Value.Id;
        });
    }

    public override void Undo() => _commandManager.EnqueueCommand();
}
