namespace SamLabs.Gfx.Viewer.ECS.Entities;

public static class EntityNames
{
    public const string Cube = "Cube";
    public const string Sphere = "Sphere";
    public const string Cylinder = "Cylinder";
    public const string Plane = "Plane";
    public const string Triangle = "Triangle";
    public const string Quad = "Quad";

    public static string MainCamera = "MainCamera";
    public static string MainGrid = "MainGrid";
    public static string TranslateManipulator { get; set; } = "TranslateManipulator";
    public static string RotateManipulator { get; set; } = "RotateManipulator";
    public static string ScaleManipulator { get; set; } = "ScaleManipulator";
    public static string Imported { get; set; } = "Imported";
}