using DefaultEcs;
using DefaultEcs.System;
using Disarray.Engine.Components;
using Disarray.Engine.Systems;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Disarray.Engine;

/// <summary>
/// Handle the physics of the game including the integration of bodies and the detection and resolution of collisions.
/// </summary>
public class Physics : ISubsystem
{
    private ISystem<float> physicsSystems;

    // Tracks the time accumulated during rendering that should be taken up during physics processing
    private float accumulator;

    public Vector2 Gravity { get; set; }
    public float FixedDeltaTime { get; init; }
    public float MaxFrameTime { get; init; }
    public float Alpha => accumulator / FixedDeltaTime;

    public void Initialize(Main game)
    {
        physicsSystems = new SequentialSystem<float>(
            new VerletIntegration(game.World, this)
            );

        // Make sure all entities have a transform
        game.World.SubscribeEntityCreated(AddTransform);

        // Make sure all moving entities have a blended transform
        game.World.SubscribeComponentAdded<RigidBody>(OnRigidBodyAdded);
        game.World.SubscribeComponentRemoved<RigidBody>(OnRigidBodyRemoved);
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float frameTime = MathF.Min(delta, MaxFrameTime);

        accumulator += frameTime;

        while (accumulator >= FixedDeltaTime)
        {
            physicsSystems.Update(FixedDeltaTime);
            accumulator -= FixedDeltaTime;
        }
    }

    private void AddTransform(in Entity entity)
    {
        entity.Set<Transform>(new());
    }

    private void OnRigidBodyAdded(in Entity entity, in RigidBody value)
    {
        ref Transform transform = ref entity.Get<Transform>();
        entity.Set<BlendedTransform>(new(in transform));
    }

    private void OnRigidBodyRemoved(in Entity entity, in RigidBody value)
    {
        entity.Remove<BlendedTransform>();
    }
}
