using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components.Sketch;

public struct LineComponent:IComponent
{
    public Vector3 P1, P2;
    
    public float Lenght => (P2 - P1).Length;
}