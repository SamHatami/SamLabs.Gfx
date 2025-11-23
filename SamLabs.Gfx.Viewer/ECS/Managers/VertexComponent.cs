using OpenTK.Mathematics;

namespace SamLabs.Gfx.Viewer.ECS.Managers;

public struct VertexComponent:IDataComponent
{
    public Vector3 Position { get; }
    public Vector3 Normal { get; }
    public Vector2 TextureCoordinate { get; }

    public VertexComponent(Vector3 position, Vector3 normal = default, Vector2 textureCoordinate = default)
    {
        Position = position;
        Normal = normal;
        TextureCoordinate = textureCoordinate;
    }
}