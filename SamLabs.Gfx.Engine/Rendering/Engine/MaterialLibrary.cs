using System.Reflection;
using Microsoft.Extensions.Logging;
using SamLabs.Gfx.Engine.Components.Common;
using SamLabs.Gfx.Engine.Rendering.Utility;

namespace SamLabs.Gfx.Engine.Rendering.Engine;

public class MaterialLibrary
{
    private readonly ILogger<MaterialLibrary> _logger;
    private readonly ShaderService _shaderService;
    Dictionary<string,MaterialComponent> _materials = new Dictionary<string, MaterialComponent>();

    public MaterialLibrary(ILogger<MaterialLibrary> logger, ShaderService shaderService)
    {
        _logger = logger;
        _shaderService = shaderService;
    }

    public void InitializeLibrary()
    {
        if(!_shaderService.Started) 
        {
            _logger.LogError("Shader service not initialized");
            return;
        }

        //we'll use resource later on
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var shaderFolder = Path.Combine(assemblyPath, "Rendering\\Shaders");
        
        foreach (var shaderProgram in _shaderService.ShadersProgram)
        {
            if(shaderProgram.Key == "pickingShader") // not a material shader
                 continue;

            var shaderPath = Path.Combine(shaderFolder,shaderProgram.Value.ShaderName+".vert");
            var shaderSource = ShaderUtility.LoadFromTextureSource(shaderPath);
            var uniformsValues = ShaderUtility.ExtractAndCreateUniformValueDictionary(shaderSource);
            
            var material = new MaterialComponent
            {
                Name =  shaderProgram.Key + "_Mat",
                Shader = shaderProgram.Value,
                PickingShader = _shaderService.GetShader("pickingShader")!, //All materials use the same picking shader for now
                UniformValues = uniformsValues
            };
            
            _materials.Add(material.Name, material);
        }
        
    }

    public void ReloadMaterial() => InitializeLibrary();


    public MaterialComponent GetDefaultMaterialForShader(string shaderName)
    {
        var key = shaderName + "_Mat";
        return _materials.TryGetValue(key, out var mat) ? mat : new MaterialComponent();
    }
}