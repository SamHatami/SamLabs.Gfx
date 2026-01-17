
using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components.Construction;

//This component is used to hold the reference plane id and offset of a construction plane, incase the reference plane is destroyed
//The actual planedata is stored in a PlaneDataComponent
public struct ConstructionOffsetPlaneComponent:IComponent
{
    public int ReferencePlaneId { get; set; }
    public Vector3 ReferenceOrigin { get; set; } 
    public float Offset { get; set; }
}