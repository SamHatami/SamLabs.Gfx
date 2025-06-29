using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SDL3;

namespace CPURendering;

public class Display
{
    private IntPtr _window;
    private IntPtr _renderer;
    private IntPtr _frameBufferTexture;
    private uint[] _frameBuffer;
    private float[] _depthBuffer;
    private int _pitch;
    private int _windowWidth;
    private int _windowHeight;

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

        for (int y = posY; y < posY + size && y < _windowHeight; y++)
        {
            for (int x = posX; x < posX + size && x < _windowWidth; x++)
            {
                if (x >= 0 && y >= 0)
                {
                    _frameBuffer[(_windowWidth * y) + x] = color;
                }
            }
        }
    }
    public void DrawLine(Vector3 p0, Vector3 p1, uint color)
    {
        int dX = (int)(p1.X - p0.X);
        int dy = (int)(p1.Y - p0.Y);

        int sideLength = Math.Abs(dX) >= Math.Abs(dy) ? Math.Abs(dX) : Math.Abs(dy);

        float xInc = dX / (float)sideLength;
        float yInc = dy / (float)sideLength;

        float x = (int)p0.X;
        float y = (int)p0.X;

        for (int i = 0; i <= sideLength; i++) {
            DrawPixel((int)Math.Round(x), (int)Math.Round(y), color);
            x += xInc;
            y += yInc;
        }
    }
    private void DrawPixel(int x, int y, uint color)
    {
        if (x >= 0 && x < _windowWidth && y >= 0 && y < _windowHeight)
        {
            _frameBuffer[(_windowWidth * y) + x] = color;
        }
    }
    public void DrawGrid(uint color)
    {
        int gridStep = 10;
        for (int y = 0; y < _windowHeight; y++)
        {
            for (int x = 0; x < _windowWidth; x++)
            {
                if (x % gridStep == 0 || y % gridStep == 0)
                    _frameBuffer[(_windowWidth * y) + x] = color;
            }
        }
    }
    public void DrawRect(int startX, int startY, int width, int height, uint color)
    {
        for (int y = startY; y < height + startY && y < _windowHeight; y++)
        {
            for (int x = startX; x < width + startX && x < _windowWidth; x++)
            {
                if (x >= 0 && y >= 0 && x < _windowWidth && y < _windowHeight)
                {
                    _frameBuffer[(_windowWidth * y) + x] = color;
                }
            }
        }
    }
    public void ClearColorBuffer()
    {
        SDL.SetRenderDrawColor(_renderer, 0, 0, 0, 255);
        if(!SDL.RenderClear(_renderer))
        {
            SDL.LogError(SDL.LogCategory.Application, $"Failed to clear color buffer: {SDL.GetError()}");
        }
    }
    public  void RenderColorBuffer()
    {
        var byteSpan = MemoryMarshal.AsBytes(_frameBuffer.AsSpan());
        if(!SDL.UpdateTexture(_frameBufferTexture, IntPtr.Zero, byteSpan, _pitch))
        {
            SDL.LogError(SDL.LogCategory.Application, $"Failed to update texture: {SDL.GetError()}");
        }
        if(!SDL.RenderTexture(_renderer, _frameBufferTexture, IntPtr.Zero, IntPtr.Zero))
        {
            SDL.LogError(SDL.LogCategory.Application, $"Failed to render texture: {SDL.GetError()}");
        }
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