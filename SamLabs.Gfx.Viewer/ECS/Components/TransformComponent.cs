using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components;

public struct TransformComponent : IDataComponent
{
    public TransformComponent()
    {
        LocalPosition = Vector3.Zero;
        LocalScale = Vector3.One;
        LocalRotation = Quaternion.Identity;
        Position = Vector3.Zero;
        Scale = Vector3.One;
        Rotation = Quaternion.Identity;
    }

    public Vector3 LocalPosition { get; set; }
    public Vector3 LocalScale { get; set; } 
    public Quaternion LocalRotation { get; set; } 
    
    public Vector3 Position { get; set; }
    public Vector3 Scale { get; set; } 
    public Quaternion Rotation { get; set; }

    public Vector3 Y => Vector3.Transform(Vector3.UnitY, Rotation);
    public Vector3 X => Vector3.Transform(Vector3.UnitX, Rotation);
    public Vector3 Z => Vector3.Transform(Vector3.UnitZ, Rotation);
    public Matrix4 WorldMatrixInverse() => Matrix4.Invert(WorldMatrix());
    
    public Matrix4 WorldMatrix() =>

        Matrix4.CreateScale(Scale) *
        Matrix4.CreateFromQuaternion(Rotation) *
        Matrix4.CreateTranslation(Position);


    public int ParentId { get; set; }
}