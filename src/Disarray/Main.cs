using DefaultEcs;
using Disarray.Engine;
using Disarray.Gameplay;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Disarray;

public class Main : Game
{
    public Main() : base()
    {
        Graphics = new GraphicsDeviceManager(this);
        Graphics.ApplyChanges();

        Data.InitializeConverters(this);

        World = new();
        Camera = new(GraphicsDevice);
        Renderer = new();
        Physics = new();
        Input = new();
        Level = new();

        Disposed += DisposeAdditional;
    }

    // Engine components and global data
    public GraphicsDeviceManager Graphics { get; init; }
    public World World { get; init; }
    public OrthographicCamera Camera { get; init; }
    public Renderer Renderer { get; init; }
    public Physics Physics { get; init; }
    public Input Input { get; init; }
    public Level Level { get; init; }

    // Model view projection matrices
    public Matrix Model => Matrix.Identity;
    public Matrix View => Camera.GetViewMatrix();
    public Matrix Projection => Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width,
        GraphicsDevice.Viewport.Height, 0, 0, -1);

    protected override void Initialize()
    {
        // Apply the systems and game settings
        Data.ApplyUserSettings(this);

        // Initialize the engine after applying settings
        Level.Initialize(this);
        Renderer.Initialize(this);
        Physics.Initialize(this);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Renderer.LoadContent();
        Level.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Input.Update(IsActive);
        Physics.Update(delta);
        Level.Update(delta);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        Renderer.Draw();
        base.Draw(gameTime);
    }

    /// <summary>
    /// Dispose additional resources.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DisposeAdditional(object? sender, EventArgs e)
    {
        Input.Dispose();

        // Try to dispose the level
        var disposableLevel = Level as IDisposable;
        if (disposableLevel != null)
        {
            disposableLevel.Dispose();
        }
    }
}
