using SamLabs.Gfx.Viewer.Geometry;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.SceneGraph;

namespace SamLabs.Gfx.Viewer.Commands;

public class ImportObjAsyncCommand : Command
{
    private readonly CommandManager _commandManager;
    private readonly string _filePath;
    private readonly Scene _scene;
    private readonly ObjectImporter _importer;
    private int _importedId;

    public ImportObjAsyncCommand(CommandManager commandManager,string filePath, Scene scene, ObjectImporter importer)
    {
        _commandManager = commandManager;
        _filePath = filePath;
        _scene = scene;
        _importer = importer;
    }


    public override void Execute()
    {
        Task.Run(async () =>
        {
            try
            {
                var imported = await _importer.Import(@"");
                _commandManager.EnqueueCommand(new AddImportedFileCommand(_scene, imported));
                _importedId = imported.Id;
            }
            catch (Exception e)
            {
            }
        });
        
    }

    public override void Undo() => _commandManager.EnqueueCommand( new RemoveRenderableCommand(_scene,_importedId)); 

}