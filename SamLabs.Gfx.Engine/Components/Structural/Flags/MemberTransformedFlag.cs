﻿using OpenTK.Mathematics;

namespace SamLabs.Gfx.Engine.Components.Structural.Flags;

/// <summary>
/// Flag component. Set when a frame member is translated or rotated as a rigid body.
/// Stores the delta movement to apply to connected nodes and the originating member id.
/// </summary>
public struct MemberTransformedFlag : IComponent
{
    public Vector3 Delta;
    public int OriginatingMemberId;
}
