using DefaultEcs;
using DefaultEcs.System;
using Disarray.Engine;
using Disarray.Engine.Components;
using Microsoft.Xna.Framework;
using System.Diagnostics;

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

        Vector2 direction = Vector2.Zero;
        int speed = 250;

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

        rigidBody.Velocity = direction * speed;
    }

    private void CheckExit()
    {
        if (Input.IsActionDown("GameExit"))
        {
            Main.Exit();
        }
    }
}
