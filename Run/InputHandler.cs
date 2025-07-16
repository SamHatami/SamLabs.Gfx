using System.Numerics;
using SDL3;

namespace Run;

public static class InputHandler
{
    public static bool HandleInput()
    {
        while (SDL.PollEvent(out var e))
            switch ((SDL.EventType)e.Type)
            {
                case SDL.EventType.Quit:
                    return false;
                case SDL.EventType.KeyDown:
                    if (e.Key.Key == SDL.Keycode.Escape)
                        return false;
                    break;
            }

        return true;
    }
    
}