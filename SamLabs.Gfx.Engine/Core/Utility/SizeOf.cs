using System.Runtime.InteropServices;
using SamLabs.Gfx.Geometry.Mesh;

namespace SamLabs.Gfx.Engine.Core.Utility;

public static class SizeOf
{
    public static int FMatrix4 => 16*4;
    public static int FVector3 => 3*4;
    public static int FVector4 => 4*4;
    public static int Vertex => Marshal.SizeOf<Vertex>();
    public static int Float => 4;
    public static int Int => 4;
    public static int Byte => 1;
    public static int Short => 2;
    public static int Bool => 1;
    
    public static int Pixel => 4;
    
}