using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Rendering.Engine.Primitives;

public struct LineInstance
{
    public Vector3 Start;
    public Vector3 End;
    public float Thickness;
    public uint StyleFlags;
    public float DashLength;
    public float GapLength;
    public uint Color;
    public int EntityID;
}

/* 
// Bit flags
const uint STYLE_DASHED = 1 << 0;  // Bit 0
const uint STYLE_DOTTED = 1 << 1;  // Bit 1
const uint STYLE_HIDDEN = 1 << 2;  // Bit 2
// ... room for 29 more flags

// Set flags
instance.StyleFlags = STYLE_DASHED;
instance.StyleFlags |= STYLE_HIDDEN;  // Combine flags

// Shader reads
bool isDashed = (styleFlags & 1u) != 0u;
*/