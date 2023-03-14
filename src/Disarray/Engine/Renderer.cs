using Microsoft.Xna.Framework;
using DefaultEcs.System;
using Disarray.Engine.Systems;
using Microsoft.Xna.Framework.Graphics;

namespace Disarray.Engine;

public class Renderer
{
    private Main main;
    private ISystem<SpriteBatch> spriteSystems;
    private SpriteBatch spriteBatch;

    public Color ClearColor { get; set; } = Color.White;
    public SpriteSortMode SpriteSortMode { get; set; }
    public BlendState BlendState { get; set; }
    public SamplerState SamplerState { get; set; }
    public DepthStencilState DepthStencilState { get; set; }
    public RasterizerState RasterizerState { get; set; }
    public Effect Effect { get; set; }

    public void Initialize(Main main)
    {
        this.main = main;

        spriteSystems = new SequentialSystem<SpriteBatch>(
            new SpriteRenderer(main.World),
            new ActionSystem<SpriteBatch>(DrawLevel)
            );
    }

    /// <summary>
    /// Load the content of the renderer, specifically the sprite batch.
    /// </summary>
    public void LoadContent()
    {
        spriteBatch = new(main.GraphicsDevice);
    }

    public void Draw()
    {
        main.GraphicsDevice.Clear(ClearColor);

        spriteBatch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, main.View);
        spriteSystems.Update(spriteBatch);
        spriteBatch.End();
    }

    private void DrawLevel(SpriteBatch spriteBatch)
    {
        main.Level.Draw(spriteBatch);
    }
}
