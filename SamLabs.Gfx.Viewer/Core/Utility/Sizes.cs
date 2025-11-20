using System.Runtime.InteropServices;
using SamLabs.Gfx.Geometry;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.Core.Utility;

public static class Sizes
{
    public static int FMatrix4 => 16*4;
    public static int FVector3 => 3*4;
    public static int FVector4 => 4*4;
    public static int VertexSize => Marshal.SizeOf<VertexComponent>();
    public static int Float => 4;
    public static int Int => 4;
    public static int Byte => 1;
    public static int Short => 2;
    public static int Bool => 1;
    
    public static int Pixel => 4;
    
}