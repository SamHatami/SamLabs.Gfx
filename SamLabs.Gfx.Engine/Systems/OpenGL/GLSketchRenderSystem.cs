﻿using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Selection;
using SamLabs.Gfx.Engine.Components.Sketch.Geometry;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Abstractions;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.OpenGL;

[RenderPassAttributes.RenderOrder(SystemOrders.SketchRender)]
public class GLSketchRenderSystem : RenderSystem
{
    private readonly EntityRegistry _entityRegistry;
    private GLShader? _lineShader;
    private bool _isPickingPass;
    private HashSet<int> _cachedSelectedIds = new();
    private int[] _lastSelectedEntityIds = Array.Empty<int>();

    public override int SystemPosition => SystemOrders.SketchRender;

    public GLSketchRenderSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry) 
        : base(entityRegistry, componentRegistry)
    {
        _entityRegistry = entityRegistry;
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        // Get shader
        if (_lineShader == null)
        {
            _lineShader = Renderer.GetShader("instanced_line");
            if (_lineShader == null)
            {
                Console.WriteLine("[GLSketchRenderSystem] Shader 'instanced_line' not found!");
                return;
            }
            Console.WriteLine("[GLSketchRenderSystem] Shader 'instanced_line' loaded successfully");
        }

        // Query all line entities with GPU data
        var lineEntities = _entityRegistry.Query
            .With<LineSegmentComponent>()
            .With<GlLineDataComponent>()
            .With<SketchLineStyleComponent>()
            .Get();

        Console.WriteLine($"[GLSketchRenderSystem] Found {lineEntities.Length} line entities to render");

        if (lineEntities.IsEmpty())
            return;

        // Get selection data for hover/selection highlighting
        var pickingEntity = ComponentRegistry.GetEntityIdsForComponentType<PickingDataComponent>();
        if (pickingEntity.Length == 0)
            return;
            
        var pickingData = ComponentRegistry.GetComponent<PickingDataComponent>(pickingEntity[0]);

        // Update selection cache if changed
        if (!pickingData.SelectedEntityIds.SequenceEqual(_lastSelectedEntityIds))
        {
            _cachedSelectedIds.Clear();
            foreach (var id in pickingData.SelectedEntityIds)
                _cachedSelectedIds.Add(id);
            _lastSelectedEntityIds = pickingData.SelectedEntityIds.ToArray();
        }

        var hoveredId = pickingData.HoveredEntityId;

        // Batch lines by shader (all use same shader for now)
        var shaderProgram = new ShaderProgram(_lineShader).Use();
        Console.WriteLine("[GLSketchRenderSystem] Shader program activated");

        // Set uniforms
        var identityMatrix = Matrix4.Identity;
        shaderProgram.SetMatrix4(UniformNames.uModel, ref identityMatrix);

        var isPickingPass = _isPickingPass ? 1 : 0;
        shaderProgram.SetInt(UniformNames.uIsPickingPass, ref isPickingPass);

        var viewportSize = frameInput.ViewportSize;
        shaderProgram.SetVector2(UniformNames.uViewportSize, viewportSize);

        // Enable blending for smooth lines
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        // Render each line entity
        foreach (var lineEntity in lineEntities)
        {
            ref var glLineData = ref ComponentRegistry.GetComponent<GlLineDataComponent>(lineEntity);

            Console.WriteLine($"[GLSketchRenderSystem] Rendering line entity {lineEntity}, VAO: {glLineData.Vao}, InstanceCount: {glLineData.InstanceCount}");

            // Bind VAO and render
            GL.BindVertexArray(glLineData.Vao);
            GL.DrawElementsInstanced(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero, glLineData.InstanceCount);
            GL.BindVertexArray(0);
        }

        GL.Disable(EnableCap.Blend);
        shaderProgram.Dispose();
        Console.WriteLine("[GLSketchRenderSystem] Rendering complete");
    }

    public void SetPickingPass(bool isPickingPass)
    {
        _isPickingPass = isPickingPass;
    }
}

