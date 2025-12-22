using SamLabs.Gfx.Viewer.ECS.Entities;

namespace SamLabs.Gfx.Viewer.ECS.Core;

public struct Entity(int id)
{
    public int Id { get; } = id;
    public EntityType Type { get; set; }
}

public static class EntityExtensions
{
    public static bool None(this Entity entity) { return entity.Id == -1; }
}