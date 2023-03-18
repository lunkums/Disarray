using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace Disarray.Engine.Util;

public static class MathHelperExtensions
{
    public const float Epsilon = 0.000001f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Round(this Vector2 value)
    {
        var x = (float)Math.Round(value.X);
        var y = (float)Math.Round(value.Y);
        return new Vector2(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Floor(this Vector2 value)
    {
        var x = (float)Math.Floor(value.X);
        var y = (float)Math.Floor(value.Y);
        return new Vector2(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Ceiling(this Vector2 value)
    {
        var x = (float)Math.Ceiling(value.X);
        var y = (float)Math.Ceiling(value.Y);
        return new Vector2(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Normalized(this Vector2 value)
    {
        float length = value.Length();
        if (MathF.Abs(length) < Epsilon) return value;
        return new(value.X / length, value.Y / length);
    }
}
