﻿namespace SamLabs.Gfx.Engine.Components.Structural.Flags;

/// <summary>
/// Flag component. Set when a frame node is translated.
/// Stores the originating member id if the move was triggered by a member transform (-1 if moved directly).
/// </summary>
public struct NodeMovedFlag : IComponent
{
    public int OriginatingMemberId; // -1 if moved directly
}
