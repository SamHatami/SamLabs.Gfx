using CPURendering.Geometry;

namespace CPURendering.Import;

public interface MeshParsingStrategy
{
    Mesh ReadFromFile(string fullPath);
    string Extension { get; }
}