using Microsoft.Xna.Framework;

namespace Disarray.Engine.Components;

internal struct BlendedTransform
{
    public Vector2 Position;
    public Vector2 Scale;
    public float Rotation;

    public BlendedTransform(in Transform transform)
    {
        Position = transform.Position;
        Scale = transform.Scale;
        Rotation = transform.Rotation;
    }
}
