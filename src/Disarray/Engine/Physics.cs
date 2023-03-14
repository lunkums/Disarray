using DefaultEcs.System;
using Disarray.Engine.Systems;
using Microsoft.Xna.Framework;

namespace Disarray.Engine;

public class Physics
{
    private ISystem<float> physicsSystems;

    public Vector2 Gravity { get; set; }

    public void Initialize(Main game)
    {
        physicsSystems = new SequentialSystem<float>(
            new VerletSystem(game.World, this)
            );
    }

    public void Update(float delta)
    {
        physicsSystems.Update(delta);
    }
}
