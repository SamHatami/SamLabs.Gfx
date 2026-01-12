namespace SamLabs.Gfx.Engine.Entities;

public struct Entity(int id)
{
    public int Id { get; } = id;
    public EntityType Type { get; set; } //TODO: REplace this with type-component
}
