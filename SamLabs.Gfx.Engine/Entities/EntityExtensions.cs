namespace SamLabs.Gfx.Engine.Entities;

public static class EntityExtensions
{
    public static bool None(this Entity entity) { return entity.Id == -1; }
    
}