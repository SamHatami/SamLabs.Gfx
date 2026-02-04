using System.Runtime.CompilerServices;

namespace SamLabs.Gfx.Engine;

public static class MiscHelpers
{
    public static string GetProjectSourcePath([CallerFilePath] string path = null)
    {
        return Path.GetDirectoryName(path);
    }
}