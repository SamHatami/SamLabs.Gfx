using OpenTK.Graphics.OpenGL;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Flags;
using SamLabs.Gfx.Engine.Components.Sketch.Geometry;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Rendering.Engine.Primitives;
using SamLabs.Gfx.Engine.Systems.Abstractions;
using System.Runtime.InteropServices;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Core.Utility;

namespace SamLabs.Gfx.Engine.Systems.OpenGL;

[RenderPassAttributes.RenderOrder(SystemOrders.Init)]
public class GLInitializeLineDataSystem : RenderSystem
{
    public override int SystemPosition => SystemOrders.Init;
    private readonly EntityRegistry _entityRegistry;
    
    // Shared quad geometry (used by all line instances)
    private static int _sharedQuadVbo = 0;
    private static int _sharedQuadEbo = 0;
    private static bool _sharedQuadInitialized = false;

    public GLInitializeLineDataSystem(EntityRegistry entityRegistry, IComponentRegistry componentRegistry) 
        : base(entityRegistry, componentRegistry)
    {
        _entityRegistry = entityRegistry;
    }

    public override void Update(FrameInput frameInput, RenderContext renderContext)
    {
        // Initialize shared quad if needed
        if (!_sharedQuadInitialized)
        {
            InitializeSharedQuad();
            _sharedQuadInitialized = true;
        }

        // Query entities that need GPU line data initialized
        var lineEntities = _entityRegistry.Query
            .With<LineSegmentComponent>()
            .With<CreateGlLineDataFlag>()
            .Get();

        if (lineEntities.IsEmpty())
            return;

        foreach (var lineEntity in lineEntities)
        {
            ref var lineSegment = ref ComponentRegistry.GetComponent<LineSegmentComponent>(lineEntity);
            
            // Get or create style component
            if (!ComponentRegistry.HasComponent<SketchLineStyleComponent>(lineEntity))
            {
                ComponentRegistry.SetComponentToEntity(SketchLineStyleComponent.Default, lineEntity);
            }
            
            ref var lineStyle = ref ComponentRegistry.GetComponent<SketchLineStyleComponent>(lineEntity);

            // Create GPU data
            var glLineData = new GlLineDataComponent();
            CreateGlLineData(ref glLineData, ref lineSegment, ref lineStyle, lineEntity);

            // Add GPU component
            ComponentRegistry.SetComponentToEntity(glLineData, lineEntity);
            
            // Remove flag
            ComponentRegistry.RemoveComponentFromEntity<CreateGlLineDataFlag>(lineEntity);
        }
    }

    private void InitializeSharedQuad()
    {
        // Create unit quad vertices (vertex shader will expand to screen-space line)
        float[] quadVertices = new float[]
        {
            -0.5f, -0.5f, 0f,  // Bottom-left
             0.5f, -0.5f, 0f,  // Bottom-right
             0.5f,  0.5f, 0f,  // Top-right
            -0.5f,  0.5f, 0f   // Top-left
        };

        uint[] indices = new uint[] { 0, 1, 2, 2, 3, 0 };

        // Create VBO for quad
        _sharedQuadVbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _sharedQuadVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsage.StaticDraw);

        // Create EBO for quad
        _sharedQuadEbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _sharedQuadEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsage.StaticDraw);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    }

    private void CreateGlLineData(ref GlLineDataComponent glLineData, ref LineSegmentComponent lineSegment, 
        ref SketchLineStyleComponent lineStyle, int entityId)
    {
        // Create VAO
        glLineData.Vao = GL.GenVertexArray();
        GL.BindVertexArray(glLineData.Vao);

        // Bind shared quad VBO
        glLineData.QuadVbo = _sharedQuadVbo;
        GL.BindBuffer(BufferTarget.ArrayBuffer, _sharedQuadVbo);
        
        // Set up quad vertex attributes (location 0)
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Bind shared quad EBO
        glLineData.Ebo = _sharedQuadEbo;
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _sharedQuadEbo);

        // Create instance VBO
        glLineData.InstanceVbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, glLineData.InstanceVbo);

        // Build LineInstance from components
        var lineInstance = new LineInstance
        {
            Start = lineSegment.StartPoint,
            End = lineSegment.EndPoint,
            Thickness = lineStyle.Thickness,
            StyleFlags = lineStyle.StyleFlags,
            DashLength = lineStyle.DashLength,
            GapLength = lineStyle.GapLength,
            Color = lineStyle.Color,
            EntityID = entityId
        };

        // Upload instance data
        int instanceSize = Marshal.SizeOf<LineInstance>();
        GL.BufferData(BufferTarget.ArrayBuffer, instanceSize, ref lineInstance, BufferUsage.DynamicDraw);

        // Set up instance attributes (locations 1-8)
        int stride = instanceSize;
        int offset = 0;

        // Start (vec3) - location 1
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, offset);
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribDivisor(1, 1);
        offset += 3 * sizeof(float);

        // End (vec3) - location 2
        GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, offset);
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribDivisor(2, 1);
        offset += 3 * sizeof(float);

        // Thickness (float) - location 3
        GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, stride, offset);
        GL.EnableVertexAttribArray(3);
        GL.VertexAttribDivisor(3, 1);
        offset += sizeof(float);

        // StyleFlags (uint) - location 4
        GL.VertexAttribIPointer(4, 1, VertexAttribIType.UnsignedInt, stride, new IntPtr(offset));
        GL.EnableVertexAttribArray(4);
        GL.VertexAttribDivisor(4, 1);
        offset += sizeof(uint);

        // DashLength (float) - location 5
        GL.VertexAttribPointer(5, 1, VertexAttribPointerType.Float, false, stride, offset);
        GL.EnableVertexAttribArray(5);
        GL.VertexAttribDivisor(5, 1);
        offset += sizeof(float);

        // GapLength (float) - location 6
        GL.VertexAttribPointer(6, 1, VertexAttribPointerType.Float, false, stride, offset);
        GL.EnableVertexAttribArray(6);
        GL.VertexAttribDivisor(6, 1);
        offset += sizeof(float);

        // Color (uint packed) - location 7
        GL.VertexAttribPointer(7, 4, VertexAttribPointerType.UnsignedByte, true, stride, offset);
        GL.EnableVertexAttribArray(7);
        GL.VertexAttribDivisor(7, 1);
        offset += sizeof(uint);

        // EntityID (int) - location 8
        GL.VertexAttribIPointer(8, 1, VertexAttribIType.Int, stride, new IntPtr(offset));
        GL.EnableVertexAttribArray(8);
        GL.VertexAttribDivisor(8, 1);

        glLineData.InstanceCount = 1;

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }
}

