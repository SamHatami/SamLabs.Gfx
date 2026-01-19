namespace SamLabs.Gfx.Engine.Tools;

public enum ToolCategory
{
    Transform,
    Sketch,
    Modifier,
    Selection,
    View,
    Measurement
}

public enum ToolState
{
    Inactive,
    Active,
    InputCapture,
    AwaitingConfirm,
    NumericInputMode
}

public enum ToolUIType
{
    StaticPanel,
    FollowCursor,
    ModalDialog,
    None
}

