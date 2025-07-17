using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using CPURendering.Geometry;

namespace CPURendering;

public struct MeshReader
{
    public static Mesh? ReadFromFile(string fullPath)
    {
        try
        {
            var extension = Path.GetExtension(fullPath); // Implement meshreader strategies, register strategies at startup
            // if(extension == ".obj")
            //     return new ObjParseStrategy(fullPath);
            
            return ReadObj(fullPath);    
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        return null;
    }

    private static Mesh? ReadObj(string fullPath)
    {
            var lines = File.ReadAllLines(fullPath);
            
            if(lines.Length < 3)
                return null;

            var mesh = new Mesh();
            var meshVerts = new List<Vector3>();
            var meshVertNormals = new List<Vector3>();
            var meshTextureCoordinates = new List<TextureCoordinate>();
            var meshFaces = new List<Face>();
            
            foreach (var line in lines)
            {
                var lineType = line.Split(" ").First();
                switch (lineType)
                {
                    case "o":
                        mesh.Name = line.Substring(1);
                        break;
                    case "v": //Vertex positions
                        ExtractVertex(line, meshVerts);
                        break;
                    case "vn": //Vertex normals
                        ExtractVertexNormal(line, meshVertNormals);
                        break;
                    case "vt": //texture coordinates
                        ExtractTextureCoordinate(line, meshTextureCoordinates);
                        break;
                    case "f": //face
                        ExtractFace(line, meshFaces, meshVerts);
                        break;
                    case "g":
                        //extract smoothing groups
                    break;
                }
            }

            mesh.Vertices = meshVerts.ToArray();
            mesh.Faces = meshFaces.ToArray();
            mesh.TextureCoordinates  = meshTextureCoordinates.ToArray();
            return mesh;
    }

    private static void ExtractFace(string line, List<Face> faces, List<Vector3> meshVerts)
    {
       
        //v/t/n v/t/n v/t/n ....
        var matches = Regex.Matches(line, @"\d+/\d+/\d+");
        
        if(matches.Count > 4)  //only support triangulated mesh for now
            return;
        
        var vertIndices = new List<int>();
        var vertTextureIndices = new List<int>();
        var vertNormalIndices = new List<int>();
        
        for(var i = 0; i < matches.Count; i++)
        {
            var vertexData = matches[i].Value.Split('/');
            vertIndices.Add(int.Parse(vertexData[0])-1);
            vertTextureIndices.Add(int.Parse(vertexData[1])-1);
            vertNormalIndices.Add(int.Parse(vertexData[2])-1);
        }
        
        
        faces.Add(new Face(vertIndices.ToArray(), vertTextureIndices.ToArray(), vertNormalIndices.ToArray()));
    }

    private static void ExtractTextureCoordinate(string line, List<TextureCoordinate> meshTextureCoordinates)
    {
        var textCoord = line.Split(" ");
        var vertexTextureCoord = new TextureCoordinate(
            float.Parse(textCoord[1], CultureInfo.InvariantCulture),
            float.Parse(textCoord[2], CultureInfo.InvariantCulture) //flip ? -1
            );
        meshTextureCoordinates.Add(vertexTextureCoord);
    }

    private static void ExtractVertexNormal(string line, List<Vector3> meshVertNormals)
    {
        var vertPositionsN = line.Split(" ");
        var vertexNormal = new Vector3(
            float.Parse(vertPositionsN[1], CultureInfo.InvariantCulture), 
            float.Parse(vertPositionsN[2], CultureInfo.InvariantCulture), 
            float.Parse(vertPositionsN[3], CultureInfo.InvariantCulture));
        meshVertNormals.Add(vertexNormal);
    }

    private static void ExtractVertex(string line, List<Vector3> meshVerts)
    {
        var vertPositions = line.Split(" ");
        var vertex = new Vector3(
            float.Parse(vertPositions[1], CultureInfo.InvariantCulture), 
            float.Parse(vertPositions[2], CultureInfo.InvariantCulture), 
            float.Parse(vertPositions[3], CultureInfo.InvariantCulture));
        meshVerts.Add(vertex);
    }
}