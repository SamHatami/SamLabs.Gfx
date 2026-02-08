namespace SamLabs.Gfx.Engine.Rendering.Utility;

public static class BitFlags
{
    const uint StyleNone = 0;
    const uint StyleDashed = 1 << 0;
    const uint StyleDotted = 1 << 1;
    const uint StyleDashDot = 1 << 2;
    const uint StyleHidden = 1 << 3;
}