using DefaultEcs;
using Disarray.Engine;
using Disarray.Gameplay.Levels;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Disarray;

public class Main : Game
{
    public Main() : base()
    {
        // The graphics device manager is necessary for the creation of some components; wait to serialize the game
        // until after its creation, which can be trigger by invoking "ApplyChanges()" (totally not a hack)
        Graphics = new(this);
        Graphics.ApplyChanges();

        Data.InitializeConverters(this);

        Assets = new();
        Camera = new(GraphicsDevice);
        Input = new();
        Physics = new();
        Renderer = new();
        World = new();
        Level = LevelFactory.Create();
    }

    public event Action ResolutionChanged;

    // Engine components and global data
    public Assets Assets { get; init; }
    public OrthographicCamera Camera { get; init; }
    public GraphicsDeviceManager Graphics { get; init; }
    public Input Input { get; init; }
    public Physics Physics { get; init; }
    public Renderer Renderer { get; init; }
    public World World { get; init; }
    public ILevel Level { get; set; }

    // Model view projection matrices
    public Matrix Model => Matrix.Identity;
    public Matrix View => Camera.GetViewMatrix(Vector2.Zero);
    public Matrix Projection => Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width,
        GraphicsDevice.Viewport.Height, 0, 0, -1);

    /// <summary>
    /// Set the resolution of the window to the given values.
    /// </summary>
    /// <param name="width">The new width of the window.</param>
    /// <param name="height">The new height of the window.</param>
    public void SetResolution(int width, int height)
    {
        Graphics.PreferredBackBufferWidth = width;
        Graphics.PreferredBackBufferHeight = height;
        Graphics.ApplyChanges();
        ResolutionChanged?.Invoke();
    }

    protected override void Initialize()
    {
        // Apply the user's settings
        Data.ApplyUserSettings(this);

        // Initialize the engine after applying settings
        Assets.Initialize(this);
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
        Input.Dispose();

        // Level should implement IDisposable if you want to manually clean up its resources
        (Level as IDisposable)?.Dispose();

        base.Dispose(disposing);
    }
}
