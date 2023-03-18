using Microsoft.Xna.Framework;
using DefaultEcs.System;
using Disarray.Engine.Systems;
using Microsoft.Xna.Framework.Graphics;

namespace Disarray.Engine;

/// <summary>
/// Handles the game's rendering systems by drawing the scene to a virtual viewport, then drawing the virtual viewport
/// to the screen.
/// </summary>
public class Renderer : ISubsystem
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
            new MovingSpriteRenderer(main.World),
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

    /// <summary>
    /// Draw the scene to the virtual viewport then draw the virtual viewport to the screen.
    /// </summary>
    public void Draw()
    {
        GraphicsDevice graphicsDevice = main.GraphicsDevice;

        // Set the render target
        graphicsDevice.SetRenderTarget(VirtualViewport.RenderTarget);
        graphicsDevice.Clear(ClearColor);

        // Draw the sprites
        spriteBatch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect,
            main.View);
        spriteSystems.Update(spriteBatch);
        spriteBatch.End();

        // Drop the render target before drawing it
        graphicsDevice.SetRenderTarget(null);
        graphicsDevice.Clear(VirtualViewport.ClearColor);

        // Draw the render target
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
        spriteBatch.Draw(VirtualViewport.RenderTarget, VirtualViewport.Destination, VirtualViewport.DrawColor);
        spriteBatch.End();
    }


    /// <summary>
    /// Draw the level to the given sprite batch.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch to draw to (where "Begin" has already been called).</param>
    private void DrawLevel(SpriteBatch spriteBatch)
    {
        main.Level.Draw(spriteBatch);
    }
}
