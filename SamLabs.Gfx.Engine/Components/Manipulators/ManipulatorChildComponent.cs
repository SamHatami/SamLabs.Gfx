using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components.Manipulators;

public enum ManipulatorAxis
{
    None = 0,
    X = 1,
    Y = 2,
    Z = 3,
    XY = 4,
    XZ = 5,
    YZ = 6
}

public static class ManipulatorChildExtension
{
    public static Vector3 ToVector3(this ManipulatorAxis manipulatorAxis)
    {
        switch (manipulatorAxis)
        {
            case ManipulatorAxis.X:
                return new Vector3(1,0,0);
            case ManipulatorAxis.Y:
                return new Vector3(0,1,0);
            case ManipulatorAxis.Z:
                return new Vector3(0,0,1);
        }
        
        return Vector3.Zero;
    }

    public static int ToInt(this ManipulatorAxis manipulatorAxis)
    {
        return manipulatorAxis switch
        {
            ManipulatorAxis.X => 0,
            ManipulatorAxis.Y => 1,
            ManipulatorAxis.Z => 2,
            ManipulatorAxis.XY => 0,
            ManipulatorAxis.XZ => 1,
            ManipulatorAxis.YZ => 2,
            _ => 3
        };
    }
}

public struct ManipulatorChildComponent: IDataComponent
{
    public ManipulatorChildComponent(int parentId, ManipulatorAxis axis = ManipulatorAxis.None)
    {
        ParentId = parentId;
        Axis = axis;
    }

    public int ParentId { get; }
    public ManipulatorAxis Axis { get; }
}