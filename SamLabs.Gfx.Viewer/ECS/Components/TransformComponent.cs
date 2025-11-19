using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Interfaces;

namespace SamLabs.Gfx.Viewer.ECS.Components;

public struct TransformComponent:IDataComponent
{
    public Vector3 Position { get; set; }
    public Vector3 Scale { get; set; }
    public Quaternion Rotation { get; set; }
    public Matrix4 WorldMatrix { get; set; }
}