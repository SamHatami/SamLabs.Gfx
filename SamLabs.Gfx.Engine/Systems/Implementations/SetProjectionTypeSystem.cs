using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Implementations;

public class SetProjectionTypeSystem:UpdateSystem
{
    public override int SystemPosition { get; } = SystemOrders.PreRenderUpdate;

    public SetProjectionTypeSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents) : base(entityRegistry, commandManager, editorEvents)
    {
    }
    
    public override void Update(FrameInput frameInput)
    {
        var cameraEntityId = GetEntityIds.With<CameraComponent>().First();
        
        if(cameraEntityId != -1) return;
        
        var cameraData = ComponentRegistry.GetComponent<CameraDataComponent>(cameraEntityId);
        
        //set projection type etc
    }
}