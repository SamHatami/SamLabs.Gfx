using OpenTK.Mathematics;
using SamLabs.Gfx.Viewer.ECS.Managers;

namespace SamLabs.Gfx.Viewer.ECS.Components;

public struct TransformComponent:IDataComponent
{
    public Vector3 Position { get; set; }
    public Vector3 Scale { get; set; }
    public Vector3 Rotation { get; set; }
    public Matrix4 WorldMatrix { get; set; }
    
    public int ParentId { get; set; }
}