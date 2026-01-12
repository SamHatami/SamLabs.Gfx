using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.SceneGraph;

namespace SamLabs.Gfx.Engine.Commands;

public class ImportObjAsyncCommand : Command
{
    private readonly string _path;
    private readonly CommandManager _commandManager;
    private readonly Scene _scene;
    private readonly EntityFactory _entityFactory;
    private int _importedId;

    public ImportObjAsyncCommand(string path ,CommandManager commandManager, Scene scene, EntityFactory entityFactory)
    {
        _path = path;
        _commandManager = commandManager;

        _scene = scene;
        _entityFactory = entityFactory;
    }


    public override void Execute()
    {
        var importedMesh = new MeshDataComponent();
        Task.Run(async () =>
        {
            try
            {
                importedMesh = await ModelLoader.LoadObj(_path);
                
                var newModelEntity = _entityFactory.CreateFromImport(EntityNames.Imported, importedMesh);
                if (newModelEntity.HasValue)
                    _importedId = newModelEntity.Value.Id;
            }
            catch (Exception e)
            {
            }
        });
        

        
    }

    public override void Undo() => _commandManager.EnqueueCommand( new RemoveRenderableCommand(_scene,_importedId)); 

}