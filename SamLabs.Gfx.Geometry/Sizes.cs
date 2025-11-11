using System.Runtime.InteropServices;

namespace SamLabs.Gfx.Geometry;

public static class Sizes
{
    public static int Matrix4Size => 16*4;
    public static int Vector3Size => 3*4;
    public static int Vector4Size => 4*4;
    public static int VertexSize => Marshal.SizeOf<Vertex>();
}