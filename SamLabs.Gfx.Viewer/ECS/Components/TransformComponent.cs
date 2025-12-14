using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components;

public struct TransformComponent : IDataComponent
{

    public Vector3 Position { get; set; }
    public Vector3 Scale { get; set; }
    public Quaternion Rotation { get; set; }

    public Vector3 Y => Vector3.Transform(Vector3.UnitY, Rotation);

    public Vector3 X => Vector3.Transform(Vector3.UnitX, Rotation);

    public Vector3 Z => Vector3.Transform(Vector3.UnitZ, Rotation);

    public Matrix4 WorldMatrix() =>
        Matrix4.Identity * 
        Matrix4.CreateTranslation(Position) * 
        Matrix4.CreateFromQuaternion(Rotation) *
        Matrix4.CreateScale(Scale);

    public int ParentId { get; set; }
}