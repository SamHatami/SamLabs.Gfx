﻿namespace SamLabs.Gfx.Engine.Core.Utility;

public static class CollectionExtensions
{
    public static int First(this ReadOnlySpan<int> entities) => entities.IsEmpty ? -1 : entities[0];

    public static bool IsEmpty(this ReadOnlySpan<int> entities) => entities.IsEmpty;

    public static bool IsEmpty(this int[]? entities) => entities == null || entities.Length == 0;

    public static int First(this int[]? entities) => (entities == null || entities.Length == 0) ? -1 : entities[0];
}