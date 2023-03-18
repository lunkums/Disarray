using DefaultEcs;
using DefaultEcs.System;
using Disarray.Engine;
using Disarray.Engine.Components;
using Disarray.Engine.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Disarray.Gameplay.Levels;

public class Level : ILevel
{
    private World world;
    private Input input;
    private Camera camera;
    private Assets assets;

    private ISystem<float> updateSystems;
    private ISystem<SpriteBatch> drawSystems;

    private InfiniteTilemapRenderer tilemapRenderer;

    // Add additional properties here and serialize their values in data/game_settings.json
    public float TilemapLayerDepth { get; set; }
    public string TilemapDirectory { get; set; }
    public string Tilemap { get; set; }

    public void Initialize(Main main)
    {
        world = main.World;
        input = main.Input;
        camera = main.Camera;
        assets = main.Assets;

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

        player.Set<RigidBody>(new());
        player.Set<Sprite>(new()
        {
            Color = Color.White,
            LayerDepth = 0.1f,
            Origin = Vector2.Zero,
            SourceRectangle = new Rectangle(0, 16, 16, 16),
            SpriteEffects = SpriteEffects.None,
            Texture = assets.Load<Texture2D>("textures/player")
        });
        player.Set<Player>(new()
        {
            Speed = 60
        });

        tilemapRenderer.LoadContent(TilemapDirectory, Tilemap);
        tilemapRenderer.LayerDepth = TilemapLayerDepth;
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Add your update logic here
        updateSystems.Update(delta);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Add your draw logic here
        drawSystems.Update(spriteBatch);
    }
}
