using DefaultEcs;
using DefaultEcs.System;
using Disarray.Engine.Components;
using Microsoft.Xna.Framework;

namespace Disarray.Engine.Systems;

[With(typeof(Transform), typeof(RigidBody))]
public class VerletIntegration : AEntitySetSystem<float>
{
    private readonly Physics physics;

    public VerletIntegration(World world, Physics physics) : base(world)
    {
        this.physics = physics;
    }

    protected override void Update(float delta, in Entity entity)
    {
        ref Transform transform = ref entity.Get<Transform>();
        ref RigidBody rigidBody = ref entity.Get<RigidBody>();

        // Begin Verlet integration
        Vector2 newPosition = transform.Position
            + rigidBody.Velocity * delta
            + rigidBody.Acceleration * (delta * delta * 0.5f);
        Vector2 newAcceleration;

        // Apply forces
        Vector2 dragForce = 0.5f * rigidBody.Drag * (rigidBody.Velocity * rigidBody.Velocity);
        Vector2 dragAcceleration = dragForce / rigidBody.Mass;
        newAcceleration = physics.Gravity - dragAcceleration;

        // Continue Verlet integration
        Vector2 newVelocity = rigidBody.Velocity
            + (rigidBody.Acceleration + newAcceleration) * (delta * 0.5f);

        transform.Position = newPosition;
        rigidBody.Velocity = newVelocity;
        rigidBody.Acceleration = newAcceleration;
    }
}
