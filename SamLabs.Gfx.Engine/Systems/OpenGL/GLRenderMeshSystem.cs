using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Manipulators;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.OpenGL;

public class GLRenderMeshSystem : RenderSystem
{
    private readonly EntityRegistry _entityRegistry;
    public override int SystemPosition => SystemOrders.MainRender;

    public GLRenderMeshSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry) : base(entityRegistry, componentRegistry)
    {
        _entityRegistry = entityRegistry;
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        //Get all glmeshes and render them, in the future we will allow to hide meshes aswell and  have a better render component
        var meshEntities = _entityRegistry.Query.With<GlMeshDataComponent>().Without<ManipulatorChildComponent>().Get();

        if (meshEntities.IsEmpty()) return;

        var pickingEntity = ComponentRegistry.GetEntityIdsForComponentType<PickingDataComponent>();
        var pickingData = ComponentRegistry.GetComponent<PickingDataComponent>(pickingEntity[0]);

        foreach (var meshEntity in meshEntities)
        {
            var transform = ComponentRegistry.GetComponent<TransformComponent>(meshEntity);
            var modelMatrix = transform.WorldMatrix;
            var mesh = ComponentRegistry.GetComponent<GlMeshDataComponent>(meshEntity);

            //Manipulators are rendered in the ManipulatorRenderSystem
            if (mesh.IsManipulator) continue;

            var materials = ComponentRegistry.GetComponent<MaterialComponent>(meshEntity);

            var isSelected = pickingData.SelectedEntityIds.Contains(meshEntity);
            var isHovered = !isSelected && pickingData.HoveredEntityId == meshEntity ? 1 : 0;
            var isSelectedInt = isSelected ? 1 : 0;

            RenderMesh(mesh, materials, modelMatrix, isHovered, isSelectedInt);
        }
    }

    private void RenderMesh(GlMeshDataComponent mesh, MaterialComponent materialComponent,
        Matrix4 modelMatrix, int isHovered, int isSelected)
    {
        //TODO: Move this to a new ShaderUniformUploadSystem where the materials handles are necessary uniforms are set for specific shaders
        using var shader = new ShaderProgram(materialComponent.Shader).Use();
        shader.SetMatrix4(UniformNames.uModel, ref modelMatrix).SetInt(UniformNames.uIsHovered, ref isHovered)
            .SetInt(UniformNames.uIsSelected, ref isSelected);
        materialComponent.Shader.UniformLocations.TryGetValue(UniformNames.uIsHovered, out var materialLocation);

        if (mesh.IsGrid)
        {
            MeshRenderer.Draw(mesh);
        }
        else
        {
            using var renderContext = MeshRenderer.Begin(mesh).Faces(); //.Edges().Vertices();
        }
    }
}