﻿using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Grid;
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
    private HashSet<int> _cachedSelectedIds = new();
    private int[] _lastSelectedEntityIds = Array.Empty<int>();

    public GLRenderMeshSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry) : base(entityRegistry, componentRegistry)
    {
        _entityRegistry = entityRegistry;
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        //Get all glmeshes and render them, in the future we will allow to hide meshes aswell and  have a better render component
        var meshEntities = _entityRegistry.Query.With<GlMeshDataComponent>().Without<ManipulatorChildComponent>().Get();

        if (meshEntities.IsEmpty()) return;
        
        var queryTime = stopwatch.Elapsed.TotalMilliseconds;

        var pickingEntity = ComponentRegistry.GetEntityIdsForComponentType<PickingDataComponent>();
        var pickingData = ComponentRegistry.GetComponent<PickingDataComponent>(pickingEntity[0]);
        
        var pickingLookupTime = stopwatch.Elapsed.TotalMilliseconds - queryTime;

        // Only rebuild selection cache if it changed
        if (!pickingData.SelectedEntityIds.SequenceEqual(_lastSelectedEntityIds))
        {
            _cachedSelectedIds.Clear();
            foreach (var id in pickingData.SelectedEntityIds)
                _cachedSelectedIds.Add(id);
            _lastSelectedEntityIds = pickingData.SelectedEntityIds.ToArray();
        }
        
        var selectionCacheTime = stopwatch.Elapsed.TotalMilliseconds - queryTime - pickingLookupTime;

        var hoveredId = pickingData.HoveredEntityId;

        // Group entities by shader, then by VAO to minimize state changes
        var shaderBatches = new Dictionary<GLShader, Dictionary<int, List<(int entity, GlMeshDataComponent mesh, TransformComponent transform, MaterialComponent material, int isSelected, int isHovered)>>>();

        foreach (var meshEntity in meshEntities)
        {
            var mesh = ComponentRegistry.GetComponent<GlMeshDataComponent>(meshEntity);

            if(mesh.IsGrid) continue;
            if (mesh.IsManipulator) continue;

            var transform = ComponentRegistry.GetComponent<TransformComponent>(meshEntity);
            var materials = ComponentRegistry.GetComponent<MaterialComponent>(meshEntity);

            var isSelected = _cachedSelectedIds.Contains(meshEntity) ? 1 : 0;
            var isHovered = (!_cachedSelectedIds.Contains(meshEntity) && hoveredId == meshEntity) ? 1 : 0;

            if (!shaderBatches.ContainsKey(materials.Shader))
                shaderBatches[materials.Shader] = new Dictionary<int, List<(int, GlMeshDataComponent, TransformComponent, MaterialComponent, int, int)>>();
            
            if (!shaderBatches[materials.Shader].ContainsKey(mesh.Vao))
                shaderBatches[materials.Shader][mesh.Vao] = new List<(int, GlMeshDataComponent, TransformComponent, MaterialComponent, int, int)>();

            shaderBatches[materials.Shader][mesh.Vao].Add((meshEntity, mesh, transform, materials, isSelected, isHovered));
        }
        
        var batchingTime = stopwatch.Elapsed.TotalMilliseconds - queryTime - pickingLookupTime - selectionCacheTime;

        // Render batched by shader, then by VAO
        var renderStartTime = stopwatch.Elapsed.TotalMilliseconds;
        var uniformsTime = 0.0;
        var drawsTime = 0.0;
        var shaderSwitchTime = 0.0;
        var vaoSwitchTime = 0.0;
        
        foreach (var (shader, vaoBatches) in shaderBatches)
        {
            var switchStart = stopwatch.Elapsed.TotalMilliseconds;
            var shaderProgram = new ShaderProgram(shader).Use();
            shaderSwitchTime += stopwatch.Elapsed.TotalMilliseconds - switchStart;
            
            foreach (var (vao, batch) in vaoBatches)
            {
                var vaoStart = stopwatch.Elapsed.TotalMilliseconds;
                GL.BindVertexArray(vao); //TODO: Share VAO for meshes with same layout, currently every mesh has its own VAO which is bad
                vaoSwitchTime += stopwatch.Elapsed.TotalMilliseconds - vaoStart;
                
                foreach (var (entity, mesh, transform, materials, isSelected, isHovered) in batch)
                {
                    var uniformStart = stopwatch.Elapsed.TotalMilliseconds;
                    var modelMatrix = transform.WorldMatrix;
                    var selected = isSelected;
                    var hovered = isHovered;
                    shaderProgram
                        .SetMatrix4(UniformNames.uModel, ref modelMatrix)
                        .SetInt(UniformNames.uIsHovered, ref hovered)
                        .SetInt(UniformNames.uIsSelected, ref selected);
                    uniformsTime += stopwatch.Elapsed.TotalMilliseconds - uniformStart;
                        
                    var drawStart = stopwatch.Elapsed.TotalMilliseconds;
                    if (mesh.Ebo > 0)
                        GL.DrawElements(mesh.PrimitiveType, mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
                    else
                        GL.DrawArrays(mesh.PrimitiveType, 0, mesh.VertexCount);
                    drawsTime += stopwatch.Elapsed.TotalMilliseconds - drawStart;
                }
                
                // var uniqueVaos = shaderBatches.Values.SelectMany(dict => dict.Keys).Distinct().Count();
                // Console.WriteLine($"[DEBUG] Unique VAOs: {uniqueVaos}");
            }
            
            GL.BindVertexArray(0);
            shaderProgram.Dispose();
        }
        
        var renderTime = stopwatch.Elapsed.TotalMilliseconds - renderStartTime;
        
        // Render grid separately
        var gridStartTime = stopwatch.Elapsed.TotalMilliseconds;
        var gridMeshEntity = _entityRegistry.Query.With<GlMeshDataComponent>().With<GridComponent>().First();
        if (gridMeshEntity != -1)
        {
            var gridTransform = ComponentRegistry.GetComponent<TransformComponent>(gridMeshEntity);
            var gridModelMatrix = gridTransform.WorldMatrix;
            var gridMesh = ComponentRegistry.GetComponent<GlMeshDataComponent>(gridMeshEntity);
            var gridMaterial = ComponentRegistry.GetComponent<MaterialComponent>(gridMeshEntity);
            
            RenderGridMesh(gridMesh, gridMaterial, gridModelMatrix);
        }
        
        // var gridTime = stopwatch.Elapsed.TotalMilliseconds - gridStartTime;
        // var totalTime = stopwatch.Elapsed.TotalMilliseconds;
        //
        // if (totalTime > 1.0)
        // {
        //     Console.WriteLine($"[GLRenderMeshSystem] Total: {totalTime:F2}ms | " +
        //         $"Query: {queryTime:F2}ms | " +
        //         $"PickingLookup: {pickingLookupTime:F2}ms | " +
        //         $"SelectionCache: {selectionCacheTime:F2}ms | " +
        //         $"Batching: {batchingTime:F2}ms | " +
        //         $"Render: {renderTime:F2}ms (Uniforms: {uniformsTime:F2}ms | Draws: {drawsTime:F2}ms | ShaderSwitch: {shaderSwitchTime:F2}ms | VAOSwitch: {vaoSwitchTime:F2}ms) | " +
        //         $"Grid: {gridTime:F2}ms");
        // }
    }

    private void RenderGridMesh(GlMeshDataComponent mesh, MaterialComponent materialComponent, Matrix4 modelMatrix)
    {
        using var shader = new ShaderProgram(materialComponent.Shader).Use();
        
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.DepthMask(false);
        materialComponent.UniformValues.TryGetValue(UniformNames.uGridSize, out var gridSize);
        materialComponent.UniformValues.TryGetValue(UniformNames.uMajorLineFrequency, out var majorGridLines);
        materialComponent.UniformValues.TryGetValue(UniformNames.uGridSpacing, out var gridSpacing);

        shader.SetMatrix4(UniformNames.uModel, ref modelMatrix)
            .SetFloat(UniformNames.uGridSize, (float)gridSize)
            .SetFloat(UniformNames.uMajorLineFrequency, (float)majorGridLines)
            .SetFloat(UniformNames.uGridSpacing, (float)gridSpacing);

        MeshRenderer.Draw(mesh);
        
        GL.Disable(EnableCap.Blend);
        GL.DepthMask(true);
    }
        
        
}