
using Microsoft.Xna.Framework;

namespace Disarray.Engine.Components;

public struct RigidBody
{
    public Vector2 Velocity;
    public Vector2 Acceleration;
    public float Drag;
    public float Mass;

    /// <summary>
    /// The minimum mass of a rigid body. Useful for avoiding a division by zero.
    /// </summary>
    private const float MinimumMass = 0.00001f;

    public RigidBody()
    {
        Velocity = Vector2.Zero;
        Acceleration = Vector2.Zero;
        Drag = 0;
        Mass = MinimumMass;
    }
}
