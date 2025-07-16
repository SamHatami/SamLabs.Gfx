using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SDL3;

namespace CPURendering;

public class Display
{
    private float[] _depthBuffer;
    private uint[] _backBuffer;
    private uint[] _frameBuffer;
    private IntPtr _frameBufferTexture;
    private int _pitch;
    private IntPtr _renderer;
    private IntPtr _window;
    private int _windowHeight;
    private int _windowWidth;

    public void InitializeWindow(int width, int height, string title = "Sam Labs - CPU Rendering")
    {
        if (!SDL.Init(SDL.InitFlags.Video))
        {
            SDL.LogError(SDL.LogCategory.System, $"SDL could not initialize: {SDL.GetError()}");
            return;
        }

        _windowWidth = width;
        _windowHeight = height;
        SDL.SetHint("RenderDriver", "Software");
        if (!SDL.CreateWindowAndRenderer(title, width, height, SDL.WindowFlags.Borderless, out _window, out _renderer))
            SDL.LogError(SDL.LogCategory.Application, $"Failed to create window and renderer: {0} {SDL.GetError()}");

        _pitch = _windowWidth * sizeof(uint);
        _backBuffer = new uint[width * height];
        _frameBuffer = new uint[width * height];
        _depthBuffer = new float[width * height];
        _frameBufferTexture =
            SDL.CreateTexture(_renderer, SDL.PixelFormat.RGBA8888, SDL.TextureAccess.Streaming, width, height);

        PrintFrameBufferSize();
    }

    public void DrawPoint(int posX, int posY, int size, uint color)
    {
        if (posX >= _windowWidth || posX < 0)
            return;
        if (posY >= _windowHeight || posY < 0)
            return;

        for (var y = posY; y < posY + size && y < _windowHeight; y++)
        for (var x = posX; x < posX + size && x < _windowWidth; x++)
            if (x >= 0 && y >= 0)
                _backBuffer[_windowWidth * y + x] = color;
    }

    public void DrawLine(Vector3 p0, Vector3 p1, uint color)
    {
        var dX = (int)(p1.X - p0.X);
        var dy = (int)(p1.Y - p0.Y);

        var sideLength = Math.Abs(dX) >= Math.Abs(dy) ? Math.Abs(dX) : Math.Abs(dy);

        var xInc = dX / (float)sideLength;
        var yInc = dy / (float)sideLength;

        float x = (int)p0.X;
        float y = (int)p0.Y;

        for (var i = 0; i <= sideLength; i++)
        {
            DrawPixel((int)Math.Round(x), (int)Math.Round(y), color);
            x += xInc;
            y += yInc;
        }
    }

    private void DrawPixel(int x, int y, uint color)
    {
        if (x >= 0 && x < _windowWidth && y >= 0 && y < _windowHeight) _backBuffer[_windowWidth * y + x] = color;
    }

    public void DrawGrid(uint color)
    {
        var gridStep = 10;
        for (var y = 0; y < _windowHeight; y++)
        for (var x = 0; x < _windowWidth; x++)
            if (x % gridStep == 0 || y % gridStep == 0)
                _backBuffer[_windowWidth * y + x] = color;
    }

    public void DrawRect(int startX, int startY, int width, int height, uint color)
    {
        for (var y = startY; y < height + startY && y < _windowHeight; y++)
        for (var x = startX; x < width + startX && x < _windowWidth; x++)
            if (x >= 0 && y >= 0 && x < _windowWidth && y < _windowHeight)
                _backBuffer[_windowWidth * y + x] = color;
    }

    public void ClearColorBuffer()
    {
        Array.Clear(_frameBuffer, 0, _frameBuffer.Length);
        SDL.SetRenderDrawColor(_renderer, 0, 0, 0, 255);
        if (!SDL.RenderClear(_renderer))
            SDL.LogError(SDL.LogCategory.Application, $"Failed to clear color buffer: {SDL.GetError()}");
    }

    public void RenderColorBuffer()
    {
        (_frameBuffer, _backBuffer) = (_backBuffer, _frameBuffer);
        
        Array.Clear(_backBuffer, 0, _backBuffer.Length);
        
        var byteSpan = MemoryMarshal.AsBytes(_frameBuffer.AsSpan());
        if (!SDL.UpdateTexture(_frameBufferTexture, IntPtr.Zero, byteSpan, _pitch))
            SDL.LogError(SDL.LogCategory.Application, $"Failed to update texture: {SDL.GetError()}");
        if (!SDL.RenderTexture(_renderer, _frameBufferTexture, IntPtr.Zero, IntPtr.Zero))
            SDL.LogError(SDL.LogCategory.Application, $"Failed to render texture: {SDL.GetError()}");
    }

    public void Render()
    {
        ClearColorBuffer();
        RenderColorBuffer();
        SDL.RenderPresent(_renderer);
    }

    private void PrintFrameBufferSize()
    {
        var structSize = Unsafe.SizeOf<byte>();
        var arraySize = _frameBuffer.Length * structSize;
        var kbSize = arraySize / 1024.0;
        var mbSize = kbSize / 1024.0;
        Console.WriteLine($"Frame buffer array size: {arraySize} bytes ({kbSize:F1} KB / {mbSize:F1} MB)");
    }
}