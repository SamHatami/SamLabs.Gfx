using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components.Manipulators;

public struct DragComponent:IComponent
{
    public Vector3 Origin;
    public Vector3 Direction;
}