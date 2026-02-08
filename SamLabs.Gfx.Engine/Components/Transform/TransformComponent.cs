using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components.Transform;

public struct TransformComponent : IComponent
{
    public Matrix4 LocalMatrix;
    
    public Matrix4 WorldMatrix;

    public bool IsDirty;

    public TransformComponent()
    {
        LocalMatrix = Matrix4.Identity;
        WorldMatrix = Matrix4.Identity;
        IsDirty = true;
    }

    public Vector3 Position
    {
        readonly get => LocalMatrix.ExtractTranslation();
        set
        {
            LocalMatrix.Row3 = new Vector4(value, 1.0f);
            IsDirty = true;
        }
    }

    public Quaternion Rotation
    {
        readonly get => LocalMatrix.ExtractRotation();
        set
        {
            var scale = LocalMatrix.ExtractScale();
            var pos = LocalMatrix.ExtractTranslation();
            
            LocalMatrix = Matrix4.CreateScale(scale) 
                        * Matrix4.CreateFromQuaternion(value) 
                        * Matrix4.CreateTranslation(pos);
            IsDirty = true;
        }
    }

    public Vector3 Scale
    {
        readonly get => LocalMatrix.ExtractScale();
        set
        {
            var rot = LocalMatrix.ExtractRotation();
            var pos = LocalMatrix.ExtractTranslation();
            
            LocalMatrix = Matrix4.CreateScale(value) 
                        * Matrix4.CreateFromQuaternion(rot) 
                        * Matrix4.CreateTranslation(pos);
            IsDirty = true;
        }
    }
    public int ParentId { get; set; }
}
