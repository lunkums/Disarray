
using Microsoft.Xna.Framework;

namespace Disarray.Engine.Components;

public struct Transform
{
    public Vector2 Position;
    public Vector2 Scale;
    public float Rotation;

    public Transform()
    {
        Position = Vector2.Zero;
        Rotation = 0;
        Scale = Vector2.One;
    }
}
