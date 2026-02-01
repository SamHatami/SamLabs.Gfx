using OpenTK.Mathematics;
using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Camera;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.Utility;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Camera;

public class ScaleToScreenSystem : UpdateSystem
{
    public override int SystemPosition => SystemOrders.PreRenderUpdate;

    public ScaleToScreenSystem(EntityRegistry entityRegistry, CommandManager commandManager, EditorEvents editorEvents,
        IComponentRegistry componentRegistry) : base(entityRegistry, commandManager, editorEvents, componentRegistry)
    {
    }

    public override void Update(FrameInput frameInput)
    {
        var scalableEntities = ComponentRegistry.GetEntityIdsForComponentType<ScaleToScreenComponent>();
        if (scalableEntities.IsEmpty()) return;

        foreach (var entityId in scalableEntities)
        {
            // Skip invalid entities or entities pending removal
            if (entityId < 0 || entityId >= EditorSettings.MaxEntities) continue;
            if (ComponentRegistry.HasComponent<PendingRemovalFlag>(entityId)) continue;
            if (!ComponentRegistry.HasComponent<TransformComponent>(entityId)) continue;
            
            var screenScale = ComponentRegistry.GetComponent<ScaleToScreenComponent>(entityId);
            ref var entityTransform = ref ComponentRegistry.GetComponent<TransformComponent>(entityId);

            var cameraEntities = ComponentRegistry.GetEntityIdsForComponentType<CameraComponent>();
            if (cameraEntities.IsEmpty) continue;

            //one active camera assumption
            var cameraEntity = cameraEntities[0];
            ref var cameraData = ref ComponentRegistry.GetComponent<CameraDataComponent>(cameraEntity);
            ref var cameraTransform = ref ComponentRegistry.GetComponent<TransformComponent>(cameraEntity);

            var toCamera = cameraTransform.Position - entityTransform.Position;
            var forward = Vector3.Normalize(cameraData.Target - cameraTransform.Position);
            var depth = MathF.Abs(Vector3.Dot(toCamera, forward));
            if (depth < 0.1f) depth = 0.1f;

            var frustumHeight = 2.0f * cameraData.OrthographicSize;

            switch (cameraData.ProjectionType)
            {
                case ProjectionType.Orthographic:

                    var viewportHeightOrtho = frameInput.ViewportSize.Y;
                    var worldYOrtho = screenScale.Size.Y / viewportHeightOrtho * frustumHeight;
                    var worldXOrtho = screenScale.Size.X / viewportHeightOrtho * frustumHeight;
                    var worldZOrtho = screenScale.Size.Z / viewportHeightOrtho * frustumHeight;

                    entityTransform.Scale = new Vector3(
                        screenScale.LockX ? entityTransform.Scale.X : worldXOrtho,
                        screenScale.LockY ? entityTransform.Scale.Y : worldYOrtho,
                        screenScale.LockZ ? entityTransform.Scale.Z : worldZOrtho
                    );
                    break;
                case ProjectionType.Perspective:
                {
                    var fovScale = 2.0f * depth * MathF.Tan(cameraData.Fov * 0.5f);
                    var viewportHeight = frameInput.ViewportSize.Y;
                    var viewportWidth = frameInput.ViewportSize.X;

                    var worldY = screenScale.Size.Y / viewportHeight * fovScale;
                    var worldX = screenScale.Size.X / viewportWidth * fovScale * (viewportWidth / viewportHeight);
                    var worldZ = screenScale.Size.Z / viewportHeight * fovScale;

                    var scaleX = screenScale.LockX ? entityTransform.Scale.X : worldX;
                    var scaleY = screenScale.LockY ? entityTransform.Scale.Y : worldY;
                    var scaleZ = screenScale.LockZ ? entityTransform.Scale.Z : worldZ;

                    entityTransform.Scale = new Vector3(scaleX, scaleY, scaleZ);
                    break;
                }
            }


            entityTransform.WorldMatrix = entityTransform.LocalMatrix;
            entityTransform.IsDirty = false;
        }
    }
}