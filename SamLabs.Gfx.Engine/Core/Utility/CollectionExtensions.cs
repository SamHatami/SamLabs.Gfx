namespace SamLabs.Gfx.Engine.Core.Utility;

public static class CollectionExtensions
{
    public static int First(this ReadOnlySpan<int> entities) => entities.IsEmpty ? -1 : entities[0];
    
}