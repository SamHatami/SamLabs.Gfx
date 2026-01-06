using SamLabs.Gfx.Viewer.Commands;
using SamLabs.Gfx.Viewer.Core;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

public class SetProjectionTypeSystem:UpdateSystem
{
    public override int SystemPosition { get; } = SystemOrders.PreRenderUpdate;

    public SetProjectionTypeSystem(EntityManager entityManager, CommandManager commandManager, EditorEvents editorEvents) : base(entityManager, commandManager, editorEvents)
    {
    }
    
    public override void Update(FrameInput frameInput)
    {
        var cameraEntityId = GetEntityIds.With<CameraComponent>().First();
        
        if(cameraEntityId != -1) return;
        
        var cameraData = ComponentManager.GetComponent<CameraDataComponent>(cameraEntityId);
        
        //set projection type etc
    }
}