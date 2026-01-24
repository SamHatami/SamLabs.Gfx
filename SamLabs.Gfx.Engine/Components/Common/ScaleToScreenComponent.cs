
using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components.Common;

public struct ScaleToScreenComponent:IComponent
{
    public Vector3 Size { get; set; }
    public bool IsPixelSize { get; set; }
    
    public bool LockX { get; set; }
    public bool LockY { get; set; }
    public bool LockZ { get; set; }
}