﻿namespace SamLabs.Gfx.Engine.Components.Sketch;

public struct SketchComponent : IComponent
{
    public int ConstructionPlaneEntityId { get; set; }
    
    /// <summary>
    /// Entity IDs of line segments belonging to this sketch
    /// </summary>
    public List<int> LineSegmentEntityIds { get; set; }
    
    /// <summary>
    /// Entity IDs of arc segments belonging to this sketch
    /// </summary>
    public List<int> ArcSegmentEntityIds { get; set; }
    
    public SketchComponent()
    {
        LineSegmentEntityIds = new List<int>();
        ArcSegmentEntityIds = new List<int>();
    }
}