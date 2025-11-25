using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;

namespace SamLabs.Gfx.Viewer.ECS.Systems;

public class MeshUploadGlDataSystem: GPUResourceSystem
{
    private readonly ComponentManager _componentManager;

    public MeshUploadGlDataSystem(ComponentManager componentManager) : base(componentManager)
    {
        _componentManager = componentManager;
    }

    public override void Update()
    {
        var glMeshDataEntities = _componentManager.GetEntityIdsFor<MeshGlDataComponent>();
        
        if(glMeshDataEntities.Length == 0) return;
        
        //TBD Implement GL upload, same as in base mesh class
    }
}