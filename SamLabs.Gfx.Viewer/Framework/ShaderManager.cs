using System.Reflection;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL;

namespace SamLabs.Gfx.Viewer.Framework;

public class ShaderManager: IDisposable
{
    private readonly ILogger<ShaderManager> _logger;
    private Dictionary<string, int> _shadersProgram = new();
    
    public ShaderManager(ILogger<ShaderManager> logger)
    {
        _logger = logger;
    }
    
    public void RegisterShaders()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var shaderFolder = Path.Combine(assemblyPath, "Shaders");
        var vertPaths = Directory.GetFiles(shaderFolder, "*.vert", SearchOption.AllDirectories);
        
        foreach (var vertPath in vertPaths)
        {
            var fragPath = vertPath.Replace(".vert", ".frag");
            
            if(!File.Exists(fragPath))
                continue;

            var vertShader = Path.GetFileNameWithoutExtension(vertPath);
            var program = GetShaderProgram(vertPath, fragPath);
            _shadersProgram[vertShader] = program; 
            
            //Maybe expand into its own shader record later on
        }
        
        Console.WriteLine($"Registered {vertPaths.Length} shaders");
    }

    public int GetShaderProgramPosition(string name) => _shadersProgram.TryGetValue(name, out var program) ? program : -1;

    public int GetShaderProgram(string vertPath, string fragPath)
    {
        var vertShader = Path.GetFileNameWithoutExtension(vertPath);
        if (!_shadersProgram.TryGetValue(vertShader, out var program))
        {
            program = CreateShaderProgram(vertPath, fragPath);
            _shadersProgram.Add(vertShader, program);
        }
        return program;
    }
    
    private int CreateShaderProgram(string vertPath, string fragPath)
        {
            var vert = LoadTextResource(vertPath);
            var frag = LoadTextResource(fragPath);
    
            var v = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(v, vert);
            GL.CompileShader(v);
            
            GL.GetShaderi(v, ShaderParameterName.CompileStatus, out int ok);
            if (ok == 0)
            {
                GL.GetShaderInfoLog(v, out var info);
                throw new Exception($"Vertex shader compile error: {info}");
            }
    
            int f = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(f, frag);
            GL.CompileShader(f);
            GL.GetShaderi(f, ShaderParameterName.CompileStatus, out ok);
            if (ok == 0)
            {
                GL.GetShaderInfoLog(f, out var info);
                throw new Exception($"Fragment shader compile error: {info}");
            }
    
            int program = GL.CreateProgram();
            GL.AttachShader(program, v);
            GL.AttachShader(program, f);
            GL.LinkProgram(program);
    
            GL.GetProgrami(program, ProgramProperty.LinkStatus , out ok);
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
        if (!File.Exists(full))
        {
            full = Path.Combine(Environment.CurrentDirectory, path);
        }
        return File.ReadAllText(full);
    }
    
    public void WatchForChanges(string path) {
        var watcher = new FileSystemWatcher(path, "*.glsl");
        watcher.Changed += (s, e) => ReloadShader(e.FullPath);
        watcher.EnableRaisingEvents = true;
    }

    private void ReloadShader(string eFullPath)
    {
        
    }

    public void Dispose()
    {
        foreach (var shader in _shadersProgram.Values)
        {
            GL.DeleteProgram(shader);
        }
    }
}

