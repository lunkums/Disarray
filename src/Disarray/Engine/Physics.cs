using DefaultEcs.System;
using Disarray.Engine.Systems;
using Microsoft.Xna.Framework;

namespace Disarray.Engine;

/// <summary>
/// Handle the physics of the game including the integration of bodies and the detection and resolution of collisions.
/// </summary>
public class Physics : ISubsystem
{
    private ISystem<float> physicsSystems;

    public Vector2 Gravity { get; set; }

    public void Initialize(Main game)
    {
        physicsSystems = new SequentialSystem<float>(
            new VerletIntegration(game.World, this)
            );
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        physicsSystems.Update(delta);
    }
}
