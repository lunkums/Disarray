﻿using DefaultEcs;
using Disarray.Engine;
using Disarray.Gameplay.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime;

namespace Disarray;

/// <summary>
/// Runs the game and provides access to its subsystems.
/// </summary>
public class Main : Game
{
    public Main() : base()
    {
        // The graphics device manager is necessary for the creation of some components; wait to serialize the game
        // until after its creation, which can be trigger by invoking "ApplyChanges()" (totally not a hack)
        Graphics = new(this);
        Graphics.ApplyChanges();

        Data.Initialize(this);

        Assets = new();
        Camera = new();
        Input = new();
        Physics = new();
        Renderer = new();
        Screen = new();
        World = new();
        Level = LevelLoader.Create();

        // Stole this from Monocle (Celeste) engine
        GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
    }

    // Engine components and global data
    public GraphicsDeviceManager Graphics { get; }
    public Assets Assets { get; init; }
    public Camera Camera { get; init; }
    public Input Input { get; init; }
    public Physics Physics { get; init; }
    public Renderer Renderer { get; init; }
    public Screen Screen { get; init; }
    public World World { get; init; }
    public ILevel Level { get; set; }

    // Model view projection matrices
    public Matrix Model => Matrix.Identity;
    public Matrix View => Camera.GetViewMatrix();
    public Matrix Projection => Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width,
        GraphicsDevice.Viewport.Height, 0, 0, -1);

    protected override void Initialize()
    {
        // Apply the user's settings
        Data.ApplyUserSettings(this);

        // Initialize the engine after applying settings
        Assets.Initialize(this);
        Camera.Initialize(this);
        Input.Initialize(this);
        Level.Initialize(this);
        Physics.Initialize(this);
        Renderer.Initialize(this);
        Screen.Initialize(this);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Renderer.LoadContent();
        Level.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        Input.Update(IsActive);
        Physics.Update(gameTime);
        Level.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        Renderer.Draw();
        base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
        // Level should implement IDisposable if you want to manually clean up its resources
        (Level as IDisposable)?.Dispose();
        base.Dispose(disposing);
    }
}
