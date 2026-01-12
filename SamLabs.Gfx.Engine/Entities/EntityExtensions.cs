using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Common;

namespace SamLabs.Gfx.Engine.Entities;

public static class EntityExtensions
{
    public static TransformComponent Transform(this Entity entity) =>
        ComponentRegistry.GetComponent<TransformComponent>(entity.Id);
    
    public static bool None(this Entity entity) { return entity.Id == -1; }
    
}