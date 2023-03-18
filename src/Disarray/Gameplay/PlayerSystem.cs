using DefaultEcs;
using DefaultEcs.System;
using Disarray.Engine;
using Disarray.Engine.Components;
using Disarray.Engine.Util;
using Microsoft.Xna.Framework;

namespace Disarray.Gameplay;

[With(typeof(Player), typeof(Transform), typeof(RigidBody))]
public class PlayerSystem : AEntitySetSystem<float>
{
    public PlayerSystem(Main main) : base(main.World)
    {
        Main = main;
        Input = main.Input;
    }

    private Main Main { get; }
    private Input Input { get; }

    protected override void Update(float delta, in Entity entity)
    {
        Move(entity);
        CheckExit();
    }

    private void Move(in Entity entity)
    {
        ref RigidBody rigidBody = ref entity.Get<RigidBody>();
        ref Player player = ref entity.Get<Player>();

        Vector2 direction = Vector2.Zero;

        if (Input.IsActionDown("MoveUp"))
        {
            direction -= Vector2.UnitY;
        }
        if (Input.IsActionDown("MoveDown"))
        {
            direction += Vector2.UnitY;
        }
        if (Input.IsActionDown("MoveLeft"))
        {
            direction -= Vector2.UnitX;
        }
        if (Input.IsActionDown("MoveRight"))
        {
            direction += Vector2.UnitX;
        }

        rigidBody.Velocity = direction * player.Speed;
    }

    private void CheckExit()
    {
        if (Input.IsActionDown("GameExit"))
        {
            Main.Exit();
        }
    }
}
