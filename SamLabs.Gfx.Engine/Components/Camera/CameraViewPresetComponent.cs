namespace SamLabs.Gfx.Engine.Components.Camera;

public struct CameraViewPresetComponent:IComponent
{
    public ViewPreset Preset { get; set; }
}

public enum ViewPreset
{
    FreeLook,
    Front,
    Back,
    Left,
    Right,
    Top,
    Bottom,
}