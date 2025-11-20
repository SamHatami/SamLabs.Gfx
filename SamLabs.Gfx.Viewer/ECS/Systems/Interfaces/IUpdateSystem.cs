using SamLabs.Gfx.Viewer.IO;

namespace SamLabs.Gfx.Viewer.ECS.Systems.Interfaces;

public interface IUpdateSystem
{
    void Update(in FrameInput frameInput);
}