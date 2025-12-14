using System.Reflection;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Viewer.Rendering.Shaders;

namespace SamLabs.Gfx.Viewer.Rendering.Engine;

public class ShaderService : IDisposable
{
    private readonly ILogger<ShaderService> _logger;
    private static Dictionary<string, GLShader> _shadersProgram = new();
    private static Dictionary<string, int> _uniformLocations = new();

    public ShaderService(ILogger<ShaderService> logger)
    {
        _logger = logger;
    }

    public void RegisterShaders()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var shaderFolder = Path.Combine(assemblyPath, "Rendering\\Shaders");
        var vertPaths = Directory.GetFiles(shaderFolder, "*.vert", SearchOption.AllDirectories);

        foreach (var vertPath in vertPaths)
        {
            var fragPath = vertPath.Replace(".vert", ".frag");

            if (!File.Exists(fragPath))
                continue;

            try
            {
                var vertShader = Path.GetFileNameWithoutExtension(vertPath);
                var program = CreateAndRegisterShader(vertPath, fragPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
   

            //Maybe expand into its own shader record later on
        }


        Console.WriteLine($"Registered {vertPaths.Length} shaders");
    }

    public GLShader? GetShader(string name)
    {
        var shader = _shadersProgram?.GetValueOrDefault(name, null);
        return shader ?? null;
    }

    private int CreateAndRegisterShader(string vertPath, string fragPath)
    {
        var vertShader = Path.GetFileNameWithoutExtension(vertPath);
        int programLocation = 0;
        if (!_shadersProgram.TryGetValue(vertShader, out var shader))
        {
            programLocation = CreateShaderProgram(vertPath, fragPath);
            var modelmatrixLocation = GL.GetUniformLocation(programLocation, "uModel");
            shader = new GLShader(vertShader, programLocation, modelmatrixLocation);
            _shadersProgram.Add(vertShader, shader);
        }

        return programLocation;
    }

    public string[] GetShaderNames()
    {
        return _shadersProgram.Keys.ToArray();
    }

    public GLShader[] GetShaderPrograms()
    {
        return _shadersProgram.Values.ToArray();
    }

    private int CreateShaderProgram(string vertPath, string fragPath)
    {
        var vert = LoadTextResource(vertPath);
        var frag = LoadTextResource(fragPath);

        var v = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(v, vert);
        GL.CompileShader(v);

        GL.GetShaderi(v, ShaderParameterName.CompileStatus, out var ok);
        if (ok == 0)
        {
            GL.GetShaderInfoLog(v, out var info);
            throw new Exception($"Vertex shader compile error: {info}");
        }

        var f = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(f, frag);
        GL.CompileShader(f);
        GL.GetShaderi(f, ShaderParameterName.CompileStatus, out ok);
        if (ok == 0)
        {
            GL.GetShaderInfoLog(f, out var info);
            throw new Exception($"Fragment shader compile error: {info}");
        }

        var program = GL.CreateProgram();
        GL.AttachShader(program, v);
        GL.AttachShader(program, f);
        GL.LinkProgram(program);

        GL.GetProgrami(program, ProgramProperty.LinkStatus, out ok);
        if (ok == 0)
        {
            GL.GetProgramInfoLog(program, out var info);
            throw new Exception($"Program link error: {info}");
        }

        GL.DeleteShader(v);
        GL.DeleteShader(f);
        return program;
    }

    private string LoadTextResource(string path)
    {
        var baseDir = AppContext.BaseDirectory;
        var full = Path.Combine(baseDir, path.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(full)) full = Path.Combine(Environment.CurrentDirectory, path);
        return File.ReadAllText(full);
    }

    public void WatchForChanges(string path)
    {
        var watcher = new FileSystemWatcher(path, "*.vert;*.frag");
        watcher.Changed += (s, e) => ReloadShader(e.FullPath); //the renderer should know about this
        watcher.EnableRaisingEvents = true;
    }

    private void ReloadShader(string eFullPath)
    {
    }

    public void Dispose()
    {
        foreach (var shader in _shadersProgram.Values) GL.DeleteProgram(shader.ProgramId);
    }
}