<<<<<<< Updated upstream
﻿using SamLabs.Gfx.Viewer.Core.Utility;
using SamLabs.Gfx.Viewer.ECS.Components;
=======
﻿using SamLabs.Gfx.Viewer.ECS.Components;
>>>>>>> Stashed changes
using SamLabs.Gfx.Viewer.ECS.Entities;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.Commands;

public class ImportObjAsyncCommand : Command
{
    private readonly string _path;
    private readonly CommandManager _commandManager;
    private readonly Scene _scene;
    private readonly EntityCreator _entityCreator;
    private int _importedId;

    public ImportObjAsyncCommand(string path ,CommandManager commandManager, Scene scene, EntityCreator entityCreator)
    {
        _path = path;
        _commandManager = commandManager;

        _scene = scene;
        _entityCreator = entityCreator;
    }


    public override void Execute()
    {
        var importedMesh = new MeshDataComponent();
        Task.Run(async () =>
        {
            try
            {
                importedMesh = await ModelLoader.LoadObj(_path);
                
                var newModelEntity = _entityCreator.CreateFromImport(EntityNames.Imported, importedMesh);
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