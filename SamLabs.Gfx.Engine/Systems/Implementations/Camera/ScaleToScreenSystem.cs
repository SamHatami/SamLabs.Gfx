﻿using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Implementations.Camera;

public class ScaleToScreenSystem:UpdateSystem
{
    override public int SystemPosition => SystemOrders.PreRenderUpdate;
    
    public ScaleToScreenSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents, IComponentRegistry componentRegistry) : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
    }

    public override void Update(FrameInput frameInput)
    {
        var scalableEntities = ComponentRegistry.GetEntityIdsForComponentType<ScaleToScreenComponent>();
        if (scalableEntities.IsEmpty) return;

        foreach (var entityId in scalableEntities)
        {
            var screenScale = ComponentRegistry.GetComponent<ScaleToScreenComponent>(entityId);
            ref var entityTransform = ref ComponentRegistry.GetComponent<TransformComponent>(entityId);

            var cameraEntities = ComponentRegistry.GetEntityIdsForComponentType<CameraComponent>();
            if (cameraEntities.IsEmpty) continue;
            
            //one active camera assumption
            var cameraEntity = cameraEntities[0];
            ref var cameraData = ref ComponentRegistry.GetComponent<CameraDataComponent>(cameraEntity);
            ref var cameraTransform = ref ComponentRegistry.GetComponent<TransformComponent>(cameraEntity);

            var toCamera = cameraTransform.Position - entityTransform.Position;
            var distance = toCamera.Length;
            var frustumHeightAtUnitDistance = 2f * MathF.Tan(cameraData.Fov / 2f);

            switch (cameraData.ProjectionType)
            {
                case ProjectionType.Orthographic:
                    // entityTransform.Scale = new Vector3(screenScale.Size*cameraData.OrthographicSize);
                    break;
                case ProjectionType.Perspective:
                {
                    var fovScale = 2.0f * distance * MathF.Tan(cameraData.Fov * 0.5f);

                    var scaleX = screenScale.LockX ? entityTransform.Scale.X : screenScale.Size.X * fovScale;
                    var scaleY = screenScale.LockY ? entityTransform.Scale.Y : screenScale.Size.Y * fovScale;
                    var scaleZ = screenScale.LockZ ? entityTransform.Scale.Z : screenScale.Size.Z * fovScale;

                    entityTransform.Scale = new Vector3(scaleX, scaleY, scaleZ);
                    break;
                }
            }
            
            
            entityTransform.WorldMatrix = entityTransform.LocalMatrix;
            entityTransform.IsDirty = false;
        }
    }
}