using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace Disarray.Engine.Util;

public static class MathHelperExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Round(this Vector2 value)
    {
        var x = (float)Math.Round(value.X);
        var y = (float)Math.Round(value.Y);
        return new Vector2(x, y);
    }
}
