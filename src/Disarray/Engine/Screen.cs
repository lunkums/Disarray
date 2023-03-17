using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Disarray.Engine;

/// <summary>
/// Manages the game screen. Acts as a wrapper for the <see cref="GameWindow"/> and <see cref="GraphicsDeviceManager"/>.
/// </summary>
public class Screen : ISubsystem
{
    private Game game;
    private GraphicsDeviceManager graphicsDeviceManager;
    private GameWindow gameWindow;

    private int previousWindowWidth;
    private int previousWindowHeight;

    private Point resolution;
    private int targetFrameRate;
    private bool isFullScreen;
    private bool isBorderless;
    private bool vsyncEnabled;

    private bool customResolution;
    private bool customTargetFrameRate;
    private bool customFullScreenStatus;
    private bool customBorderlessStatus;
    private bool customVsyncStatus;

    public event EventHandler<EventArgs> ResolutionChanged;

    public int TargetFrameRate
    {
        get => targetFrameRate;
        set
        {
            customTargetFrameRate = true;
            targetFrameRate = value;
        }
    }

    /// <summary>
    /// Set, but don't apply, the full screen status of the screen.
    /// </summary>
    public bool IsFullScreen
    {
        get => isFullScreen;
        set
        {
            customFullScreenStatus = true;
            isFullScreen = value;
        }
    }

    /// <summary>
    /// Set, but don't apply, the borderless status of the window.
    /// </summary>
    public bool IsBorderless
    {
        get => isBorderless;
        set
        {
            customBorderlessStatus = true;
            isBorderless = value;
        }
    }

    public bool VsyncEnabled
    {
        get => vsyncEnabled;
        set
        {
            customVsyncStatus = true;
            vsyncEnabled = value;
        }
    }

    /// <summary>
    /// Set, but don't apply, the resolution of the screen.
    /// </summary>
    public Point Resolution
    {
        get => resolution;
        set
        {
            customResolution = true;
            resolution = value;
        }
    }

    public void Initialize(Main main)
    {
        game = main;
        graphicsDeviceManager = main.Graphics;
        gameWindow = main.Window;

        previousWindowHeight = graphicsDeviceManager.PreferredBackBufferHeight;
        previousWindowWidth = graphicsDeviceManager.PreferredBackBufferWidth;

        if (!customBorderlessStatus)
        {
            isBorderless = gameWindow.IsBorderlessEXT;
        }
        if (!customFullScreenStatus)
        {
            isFullScreen = graphicsDeviceManager.IsFullScreen;
        }
        if (!customResolution)
        {
            resolution = new(gameWindow.ClientBounds.Width, gameWindow.ClientBounds.Height);
        }
        if (!customVsyncStatus)
        {
            vsyncEnabled = graphicsDeviceManager.SynchronizeWithVerticalRetrace;
        }
        if (!customTargetFrameRate)
        {
            targetFrameRate = (int)Math.Round(1 / game.TargetElapsedTime.TotalSeconds);
        }
        ApplyChanges();

        gameWindow.ClientSizeChanged += GameWindow_ClientSizeChanged;
    }


    /// <summary>
    /// Set the resolution to the given width and height and apply the change.
    /// </summary>
    /// <param name="width">The new width.</param>
    /// <param name="height">The new height.</param>
    public void SetResolution(int width, int height)
    {
        resolution = new(width, height);
        ApplyChanges();
    }

    /// <summary>
    /// Set the full screen status to the opposite of its current value and apply the change.
    /// </summary>
    public void ToggleFullscreen()
    {
        IsFullScreen = !IsFullScreen;
        ApplyChanges();
    }

    /// <summary>
    /// Set the borderless status to the opposite of its current value and apply the change.
    /// </summary>
    public void ToggleBorderless()
    {
        IsBorderless = !IsBorderless;
        ApplyChanges();
    }

    /*
     * Manage state
     */

    /// <summary>
    /// Apply any recent changes to the full screen status, borderless status, and resolution.
    /// </summary>
    public void ApplyChanges()
    {
        ApplyFullScreen();
        ApplyBorderless();
        ApplyResolution();
        ApplyVsync();
        ApplyTargetFrameRate();
        graphicsDeviceManager.ApplyChanges();
        ResolutionChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ApplyFullScreen()
    {
        if (graphicsDeviceManager.IsFullScreen == isFullScreen) return;

        int targetWidth;
        int targetHeight;

        if (isFullScreen)
        {
            previousWindowWidth = gameWindow.ClientBounds.Width;
            previousWindowHeight = gameWindow.ClientBounds.Height;
            targetWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            targetHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }
        else
        {
            targetWidth = previousWindowWidth;
            targetHeight = previousWindowHeight;
        }

        graphicsDeviceManager.IsFullScreen = isFullScreen;
        Resolution = new(targetWidth, targetHeight);
    }

    private void ApplyBorderless()
    {
        gameWindow.IsBorderlessEXT = isBorderless;
    }

    private void ApplyResolution()
    {
        graphicsDeviceManager.PreferredBackBufferWidth = resolution.X;
        graphicsDeviceManager.PreferredBackBufferHeight = resolution.Y;
    }

    private void ApplyVsync()
    {
        graphicsDeviceManager.SynchronizeWithVerticalRetrace = vsyncEnabled;
    }

    private void ApplyTargetFrameRate()
    {
        game.TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / targetFrameRate);
    }

    private void GameWindow_ClientSizeChanged(object? sender, EventArgs e)
    {
        resolution = new(gameWindow.ClientBounds.Width, gameWindow.ClientBounds.Height);
        ApplyChanges();
    }
}
