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
    public static string TranslateGizmo { get; set; } = "TranslateGizmo";
    public static string RotateGizmo { get; set; } = "RotateGizmo";
    public static string ScaleGizmo { get; set; } = "ScaleGizmo";
    public static string Imported { get; set; } = "Imported";
}