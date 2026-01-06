using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Managers;
using SamLabs.Gfx.Viewer.ECS.Systems.Abstractions;
using SamLabs.Gfx.Viewer.IO;
using SamLabs.Gfx.Viewer.Rendering;
using SamLabs.Gfx.Viewer.Rendering.Engine;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Implementations;

public class GLRenderMeshSystem : RenderSystem
{
    public override int SystemPosition => SystemOrders.MainRender;

    public GLRenderMeshSystem(EntityManager entityManager) : base(entityManager)
    {
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        //Get all glmeshes and render them, in the future we will allow to hide meshes aswell
        var meshEntities = ComponentManager.GetEntityIdsForComponentType<GlMeshDataComponent>();
        if (meshEntities.Length == 0) return;
        
        var pickingEntity = ComponentManager.GetEntityIdsForComponentType<PickingDataComponent>();
        var pickingData = ComponentManager.GetComponent<PickingDataComponent>(pickingEntity[0]);

        foreach (var meshEntity in meshEntities)
        {
            var transform = ComponentManager.GetComponent<TransformComponent>(meshEntity);
            var modelMatrix = transform.WorldMatrix;
            var mesh = ComponentManager.GetComponent<GlMeshDataComponent>(meshEntity);
            
            //Manipulators are rendered in the ManipulatorRenderSystem
            if (mesh.IsManipulator) continue;
            
            var materials = ComponentManager.GetComponent<MaterialComponent>(meshEntity);
            
            var isSelected = pickingData.SelectedEntityIds.Contains(meshEntity);
            var isHovered = (!isSelected && pickingData.HoveredEntityId == meshEntity) ? 1 : 0;
            var isSelectedInt = isSelected ? 1 : 0;
            
            RenderMesh(mesh, materials, modelMatrix, isHovered, isSelectedInt);
        }
    }

    private void RenderMesh(GlMeshDataComponent mesh, MaterialComponent materialComponent,
        Matrix4 modelMatrix, int isHovered, int isSelected)
    {
        using var shader = new ShaderProgram(materialComponent.Shader).Use();
        shader.SetMatrix4(UniformNames.uModel,ref modelMatrix).
            SetInt(UniformNames.uIsHovered, ref isHovered)
            .SetInt(UniformNames.uIsSelected, ref isSelected);
        materialComponent.Shader.UniformLocations.TryGetValue(UniformNames.uIsHovered, out var materialLocation);
        MeshRenderer.Draw(mesh);
    }
}