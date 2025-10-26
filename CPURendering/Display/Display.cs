using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SDL3;

namespace CPURendering.Display;

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
    private int _windowHeightB;
    private int _windowWidthB;
    private int _ssaa;

    public void InitializeWindow(int width, int height, string title = "Sam Labs - CPU Rendering", int samplingSize = 2, bool sampling = false)
    {
        if (!SDL.Init(SDL.InitFlags.Video))
        {
            SDL.LogError(SDL.LogCategory.System, $"SDL could not initialize: {SDL.GetError()}");
            return;
        }

        TTF.Init();

        SDL.SetHint("RenderDriver", "Software");
        if (!SDL.CreateWindowAndRenderer(title, width, height, SDL.WindowFlags.Borderless, out _window, out _renderer))
            SDL.LogError(SDL.LogCategory.Application, $"Failed to create window and renderer: {0} {SDL.GetError()}");


        _windowWidth = width;
        _windowHeight = height;
        
        _pitch = _windowWidth * sizeof(uint);
        
        _ssaa = sampling ? samplingSize : 1;
        _windowWidthB = width * _ssaa;
        _windowHeightB = height * _ssaa;
        
        int bufferSize = width * height;

        _frameBuffer = new uint[bufferSize];
        _depthBuffer = new float[bufferSize];

        int finalBufferSize = _windowWidth * _windowHeight; 
        _frameBuffer = new uint[finalBufferSize];
        _depthBuffer = new float[finalBufferSize];


        _backBuffer = sampling ? new uint[_windowWidthB * _windowHeightB] : new uint[finalBufferSize]; 

        _frameBufferTexture =
            SDL.CreateTexture(_renderer, SDL.PixelFormat.RGBA8888, SDL.TextureAccess.Streaming, width, height);

        PrintFrameBufferSize();
    }

    public void DrawPoint(int posX, int posY, int size, uint color)
    {
        if (posX >= _windowWidthB || posX < 0)
            return;
        if (posY >= _windowHeightB || posY < 0)
            return;
        
        size *= _ssaa;
        for (var y = posY; y < posY + size && y < _windowHeightB; y++)
        for (var x = posX; x < posX + size && x < _windowWidthB; x++)
            if (x >= 0 && y >= 0)
                _backBuffer[_windowWidthB * y + x] = color;
    }

    public void DrawFilledCircle(int posX, int posY, int radius, uint color)
    {
        //TBD
    }


    public void DrawLine(Vector2 p0, Vector2 p1, uint color)
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
            DrawPoint((int)Math.Round(x), (int)Math.Round(y),1,color);
            x += xInc;
            y += yInc;
        }
    }

    public void DrawPixel(int x, int y, uint color)
    {
        
        if (x >= 0 && x < _windowWidthB && y >= 0 && y < _windowHeightB) _backBuffer[_windowWidthB * y + x] = color;
    }

    public void DrawGrid(uint color)
    {
        var gridStep = 10;
        for (var y = 0; y < _windowHeightB; y++)
        for (var x = 0; x < _windowWidthB; x++)
            if (x % gridStep == 0 || y % gridStep == 0)
                _backBuffer[_windowWidthB * y + x] = color;
    }

    public void DrawRect(int startX, int startY, int width, int height, uint color)
    {
        for (var y = startY; y < height + startY && y < _windowHeightB; y++)
        for (var x = startX; x < width + startX && x < _windowWidthB; x++)
            if (x >= 0 && y >= 0 && x < _windowWidthB && y < _windowHeightB)
                _backBuffer[_windowWidthB * y + x] = color;
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
        //Downsize _backbuffer after sampling
        if (_ssaa > 1)
            SuperSample(_frameBuffer, _backBuffer);
        else
            (_frameBuffer, _backBuffer) = (_backBuffer, _frameBuffer);

        Array.Clear(_backBuffer, 0, _backBuffer.Length);

        var byteSpan = MemoryMarshal.AsBytes(_frameBuffer.AsSpan());
        if (!SDL.UpdateTexture(_frameBufferTexture, IntPtr.Zero, byteSpan, _pitch))
            SDL.LogError(SDL.LogCategory.Application, $"Failed to update texture: {SDL.GetError()}");
        if (!SDL.RenderTexture(_renderer, _frameBufferTexture, IntPtr.Zero, IntPtr.Zero))
            SDL.LogError(SDL.LogCategory.Application, $"Failed to render texture: {SDL.GetError()}");
    }

    private void SuperSample(uint[] frameBuffer, uint[] backBuffer)
    {
        for (var y = 0; y < _windowHeight; y++)
        {
            for (var x = 0; x < _windowWidth; x++)
            {
                int hx = x * _ssaa;
                int hy = y * _ssaa;
                // Kom ihåg indexberäkningen: index = y * bredd + x
                uint c1 = _backBuffer[hy * _windowWidthB + hx];
                uint c2 = _backBuffer[hy * _windowWidthB + (hx + 1)];
                uint c3 = _backBuffer[(hy + 1) * _windowWidthB + hx];
                uint c4 = _backBuffer[(hy + 1) * _windowWidthB + (hx + 1)];

                _frameBuffer[y * _windowWidth + x] = AverageColor(new[] { c1, c2, c3, c4 });
            }
        }
    }


    public uint AverageColor(uint[] colors)
    {
        int r = 0;
        int g = 0;
        int b = 0;
        for (var i = 0; i < colors.Length; i++)
        {
            r += (int)colors[i] >> 24;
            g += (int)colors[i] >> 16;
            b += (int)colors[i] >> 8;
        }

        r /= colors.Length;
        g /= colors.Length;
        b /= colors.Length;
        return (uint)(r << 24 | g << 16 | b << 8 | 0xFF);
    }

    public void Render()
    {
        ClearColorBuffer();
        RenderColorBuffer();
        SDL.RenderPresent(_renderer);
    }

    public void DisplayInformation() // The entire writing text will probably be the next big thing
    {
        //https://gist.github.com/stavrossk/5004111
        //https://dev.to/deusinmachina/sdl-tutorial-in-c-part-2-displaying-text-o55
        var font = TTF.OpenFont("Sans.ttf", 16);
        var color = new SDL.Color
        {
            R = 255,
            B = 255,
            G = 255
        };
        var textSurface = TTF.RenderTextSolid(font, "Hello World", UIntPtr.Zero, color);
        var textTexture = SDL.CreateTextureFromSurface(_renderer, textSurface);
        SDL.Rect rect = new SDL.Rect()
        {
            X = 10,
            Y = 10,
            W = 100,
            H = 100
        };
    }

    private void PrintFrameBufferSize()
    {
        var structSize = Unsafe.SizeOf<byte>();
        var arraySize = _frameBuffer.Length * structSize;
        var kbSize = arraySize / 1024.0;
        var mbSize = kbSize / 1024.0;
        Console.WriteLine($"Frame buffer array size: {arraySize} bytes ({kbSize:F1} KB / {mbSize:F1} MB)");
    }

    public void DrawBoundingBox(BoundingBox2D boundingBox, uint color = 0xFFFFFFFF)
    {
        DrawLine(new Vector2(boundingBox.MinX, boundingBox.MinY), new Vector2(boundingBox.MaxX, boundingBox.MinY),
            color);
        DrawLine(new Vector2(boundingBox.MaxX, boundingBox.MinY), new Vector2(boundingBox.MaxX, boundingBox.MaxY),
            color);
        DrawLine(new Vector2(boundingBox.MaxX, boundingBox.MaxY), new Vector2(boundingBox.MinX, boundingBox.MaxY),
            color);
        DrawLine(new Vector2(boundingBox.MinX, boundingBox.MaxY), new Vector2(boundingBox.MinX, boundingBox.MinY),
            color);
    }
}