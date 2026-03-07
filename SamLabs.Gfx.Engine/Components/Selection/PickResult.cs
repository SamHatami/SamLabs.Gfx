namespace SamLabs.Gfx.Engine.Components.Selection;

public readonly record struct PickResult
{
    public static readonly PickResult Empty = new(-1, -1, SelectionType.None);

    public PickResult(int entityId, int subElementId, SelectionType type)
    {
        EntityId = entityId;
        SubElementId = subElementId;
        Type = type;
    }

    public int EntityId { get; init; }
    public int SubElementId { get; init; }
    public SelectionType Type { get; init; }
}
