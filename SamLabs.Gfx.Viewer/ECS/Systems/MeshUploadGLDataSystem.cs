using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Components.Flags;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;

namespace SamLabs.Gfx.Viewer.ECS.Systems;

public class MeshUploadGlDataSystem : PreRenderSystem
{
    private readonly ComponentManager _componentManager;

    public MeshUploadGlDataSystem(ComponentManager componentManager) : base(componentManager)
    {
        _componentManager = componentManager;
    }

    public override void Update()
    {
        var glMeshDataEntities = _componentManager.GetEntityIdsFor<MeshGlDataChangedComponent>();
        if (glMeshDataEntities.Length == 0) return;
        
        var glMeshData = new MeshGlDataComponent[glMeshDataEntities.Length];
        for (var i = 0; i < glMeshDataEntities.Length; i++)
        {
            glMeshData[i] = _componentManager.GetComponent<MeshGlDataComponent>(glMeshDataEntities[i]);
            
            UpdateGlMeshData(glMeshData[i]);
            
        }



        //TBD Implement GL upload, same as in base mesh class
    }

    private void UpdateGlMeshData(MeshGlDataComponent glMeshData)
    {
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
    }
}