﻿using SamLabs.Gfx.Engine.Commands;
using SamLabs.Gfx.Engine.Components;
using SamLabs.Gfx.Engine.Components.Sketch;
using SamLabs.Gfx.Engine.Components.Sketch.Geometry;
using SamLabs.Gfx.Engine.Core;
using SamLabs.Gfx.Engine.Entities;
using SamLabs.Gfx.Engine.IO;
using SamLabs.Gfx.Engine.Rendering.Engine;
using SamLabs.Gfx.Engine.Rendering.Engine.Primitives;
using SamLabs.Gfx.Engine.Systems.Abstractions;

namespace SamLabs.Gfx.Engine.Systems.Drawing;

public class CurveTesselationSystem: UpdateSystem
{
    //Needs global tesselation settings, such as max segment length, max angle between segments, etc.
    //Move to GPU if possible, or at least use multi-threading to speed up the process. I need to be able to debug this
    //keeping in CPU for now.
    List<LineInstance> _lineInstancesCache = new List<LineInstance>();
    
    public CurveTesselationSystem(EntityRegistry entityRegistry, CommandManager commandManager,
        EditorEvents editorEvents, IComponentRegistry componentRegistry) : base(entityRegistry, commandManager,
        editorEvents, componentRegistry)
    {
    }

    public override void Update(FrameInput frameInput)
    {
        
    }

    public void EvaluateArc(ArcSegmentComponent arcSegment)
    {
        
    }
}