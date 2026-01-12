namespace SamLabs.Gfx.Engine.Core.Utility;

public static class EnumTypes
{
    public enum ProjectionType
    {
        Perspective,
        Orthographic,
    }
    
    public enum ToolType
    {
        None,
        Translate,
        Rotate,
        Scale,
        Boolean
    }
}