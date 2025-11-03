using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace SamLabs.Gfx.Geometry;

public struct Vertex
{
    public static readonly int SizeOf = Marshal.SizeOf<Vertex>();
    public Vector3 Position { get; }
    public Vector3 Normal { get; }
    public Vector2 TextureCoordinate { get; }

    public Vertex(Vector3 position, Vector3 normal = default, Vector2 textureCoordinate = default)
    {
        Position = position;
        Normal = normal;
        TextureCoordinate = textureCoordinate;
    }
    
    
}