using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Gizmos;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

public class TransformSystem:UpdateSystem
{
    public TransformSystem() : base()
    {
    }

    public override void Update(FrameInput frameInput)
    {
        var activeGizmo = GetEntities.With<ActiveGizmoComponent>();
        if(activeGizmo.IsEmpty) return;
        
        var selectedEntities = GetEntities.With<SelectedComponent>().AndWith<TransformComponent>();

        //TODO: How to attach the gizmo to the selected entity?
        
       //which axis or plane is selected as the transfom origin?
       //is the user currently trying to transform?
        foreach (var entity in selectedEntities)
        {
            
        }
    }
}