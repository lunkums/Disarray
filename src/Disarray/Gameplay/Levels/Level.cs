using DefaultEcs;
using DefaultEcs.System;
using Disarray.Engine;
using Disarray.Engine.Components;
using Disarray.Engine.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Disarray.Gameplay.Levels;

public class Level : ILevel
{
    private World world;
    private Input input;
    private Camera camera;

    private ISystem<float> updateSystems;
    private ISystem<SpriteBatch> drawSystems;

    private TilemapRenderer tilemapRenderer;

    // Add additional properties here and serialize their values in data/game_settings.json
    public float TilemapLayerDepth { get; set; }
    public string TilemapDirectory { get; set; }
    public string Tilemap { get; set; }

    public void Initialize(Main main)
    {
        world = main.World;
        input = main.Input;
        camera = main.Camera;

        tilemapRenderer = new(main);

        // Add your custom systems here (and define new components wherever you want)
        updateSystems = new SequentialSystem<float>(
            new PlayerSystem(main),
            new ActionSystem<float>(tilemapRenderer.Update)
            );
        drawSystems = new SequentialSystem<SpriteBatch>(
            new ActionSystem<SpriteBatch>(tilemapRenderer.Draw)
            );
    }

    public void LoadContent()
    {
        // Create your entities here
        Entity player = world.CreateEntity();

        player.Set<Transform>(new());
        player.Set<RigidBody>(new());
        //player.Set<Sprite>(new()
        //{
        //    Color = Color.White,
        //    LayerDepth = 0f,
        //    Origin = Vector2.Zero,
        //    SourceRectangle = null,
        //    SpriteEffects = SpriteEffects.None,
        //    Texture = main.Content.Load<Texture2D>("textures/player")
        //});
        player.Set<Player>(new());

        tilemapRenderer.LoadContent(TilemapDirectory, Tilemap);
        tilemapRenderer.LayerDepth = TilemapLayerDepth;
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 direction = Vector2.Zero;
        int speed = 250;

        if (input.IsActionDown("MoveUp"))
        {
            direction -= Vector2.UnitY;
        }
        if (input.IsActionDown("MoveDown"))
        {
            direction += Vector2.UnitY;
        }
        if (input.IsActionDown("MoveLeft"))
        {
            direction -= Vector2.UnitX;
        }
        if (input.IsActionDown("MoveRight"))
        {
            direction += Vector2.UnitX;
        }
        if (input.IsActionDown("ScaleUp"))
        {
            camera.Zoom += 0.1f;
        }
        if (input.IsActionDown("ScaleDown"))
        {
            camera.Zoom -= 0.1f;
        }
        if (input.IsActionDown("RotateUp"))
        {
            camera.Rotation += 0.1f;
        }
        if (input.IsActionDown("RotateDown"))
        {
            camera.Rotation -= 0.1f;
        }

        camera.Position += direction * speed * delta;

        // Add your update logic here
        updateSystems.Update(delta);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Add your draw logic here
        drawSystems.Update(spriteBatch);
    }
}
