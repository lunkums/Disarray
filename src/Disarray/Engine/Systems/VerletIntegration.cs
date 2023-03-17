using DefaultEcs;
using DefaultEcs.System;
using Disarray.Engine.Components;
using Disarray.Engine.Util;
using Microsoft.Xna.Framework;

namespace Disarray.Engine.Systems;

[With(typeof(Transform), typeof(RigidBody), typeof(BlendedTransform))]
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
        ref BlendedTransform blendedTransform = ref entity.Get<BlendedTransform>();
        ref RigidBody rigidBody = ref entity.Get<RigidBody>();

        // Save the original position
        blendedTransform.Position = transform.Position;

        //// Begin Verlet integration
        //Vector2 newPosition = transform.Position
        //    + rigidBody.Velocity * delta
        //    + rigidBody.Acceleration * (delta * delta * 0.5f);
        //Vector2 newAcceleration;

        //// Apply forces
        //Vector2 dragForce = 0.5f * rigidBody.Drag * (rigidBody.Velocity * rigidBody.Velocity);
        //Vector2 dragAcceleration = dragForce / rigidBody.Mass;
        //newAcceleration = physics.Gravity - dragAcceleration;

        //// Continue Verlet integration
        //Vector2 newVelocity = rigidBody.Velocity
        //    + (rigidBody.Acceleration + newAcceleration) * (delta * 0.5f);

        //// Set the blended transform
        //blendedTransform.Position = (newPosition * physics.Alpha + transform.Position * (1.0f - physics.Alpha)).Round();

        //transform.Position = newPosition;
        //rigidBody.Velocity = newVelocity;
        //rigidBody.Acceleration = newAcceleration;
        rigidBody.Velocity += rigidBody.Acceleration * delta;
        transform.Position += rigidBody.Velocity * delta;

        blendedTransform.Position = Vector2.Lerp(blendedTransform.Position, transform.Position, physics.Alpha);
        blendedTransform.Position = blendedTransform.Position.Round();
    }
}
