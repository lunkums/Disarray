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

    public Color ClearColor { get; set; } = Color.CornflowerBlue;
    public SpriteSortMode SpriteSortMode { get; set; }
    public BlendState BlendState { get; set; }
    public SamplerState SamplerState { get; set; }
    public DepthStencilState DepthStencilState { get; set; }
    public RasterizerState RasterizerState { get; set; }
    public Effect Effect { get; set; }
    public VirtualViewport VirtualViewport { get; set; }

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
        VirtualViewport.LoadContent(main);
    }

    public void Draw()
    {
        GraphicsDevice graphicsDevice = main.GraphicsDevice;

        // Set the render target
        graphicsDevice.SetRenderTarget(VirtualViewport.RenderTarget);
        graphicsDevice.Clear(ClearColor);

        // Draw the sprites
        spriteBatch.Begin(SpriteSortMode, BlendState, SamplerState, graphicsDevice.DepthStencilState, RasterizerState,
            Effect);
        spriteSystems.Update(spriteBatch);
        spriteBatch.End();

        // Drop the render target before drawing it
        graphicsDevice.SetRenderTarget(null);
        graphicsDevice.Clear(VirtualViewport.ClearColor);

        // Draw the render target
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState, SamplerState, DepthStencilState, RasterizerState);
        spriteBatch.Draw(VirtualViewport.RenderTarget, VirtualViewport.Destination, VirtualViewport.DrawColor);
        spriteBatch.End();
    }

    private void DrawLevel(SpriteBatch spriteBatch)
    {
        main.Level.Draw(spriteBatch);
    }
}
