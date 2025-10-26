using SDL3;

//https://github.com/edwardgushchin/SDL3-CS/blob/master/SDL3-CS.Examples/FPS%20Counter/FPSCounter.cs
namespace CPURendering.Display;

public class FpsCounter
{
    private ulong _lastTime = SDL.GetPerformanceCounter();
    private int _frameCount;
    private double _fps;

    public void Update()
    {
        _frameCount++;
        var currentTime = SDL.GetPerformanceCounter();
        var elapsedTime = (currentTime - _lastTime) / (double)SDL.GetPerformanceFrequency();

        if (!(elapsedTime >= 0.1)) return;
        
        _fps = _frameCount / elapsedTime;
        _frameCount = 0;
        _lastTime = currentTime;
    }
    
    public double FPS => _fps;
}