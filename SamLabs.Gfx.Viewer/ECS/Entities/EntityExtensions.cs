using SamLabs.Gfx.Viewer.Commands;
using SamLabs.Gfx.Viewer.ECS.Components;
using SamLabs.Gfx.Viewer.ECS.Core;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Entities;

public static class EntityExtensions
{
    public static TransformComponent Transform(this Entity entity) =>
        ComponentManager.GetComponent<TransformComponent>(entity.Id);
}