using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Disarray.Engine;

public class Screen
{
    private GraphicsDeviceManager graphicsDeviceManager;
    private GameWindow gameWindow;

    private int previousWindowWidth;
    private int previousWindowHeight;

    private Point resolution;
    private bool isFullScreen;
    private bool isBorderless;

    private bool customResolution;
    private bool customFullScreenStatus;
    private bool customBorderlessStatus;

    public event EventHandler<EventArgs> ResolutionChanged;

    public bool IsFullScreen
    {
        get => isFullScreen;
        set
        {
            customFullScreenStatus = true;
            isFullScreen = value;
        }
    }
    public bool IsBorderless
    {
        get => isBorderless;
        set
        {
            customBorderlessStatus = true;
            isBorderless = value;
        }
    }
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
        ApplyChanges();

        gameWindow.ClientSizeChanged += GameWindow_ClientSizeChanged;
    }

    public void SetResolution(int width, int height)
    {
        resolution = new(width, height);
        ApplyChanges();
    }

    public void ToggleFullscreen()
    {
        IsFullScreen = !IsFullScreen;
        ApplyChanges();
    }

    public void ToggleBorderless()
    {
        IsBorderless = !IsBorderless;
        ApplyChanges();
    }

    /*
     * Manage state
     */

    public void ApplyChanges()
    {
        ApplyFullScreen();
        ApplyBorderless();
        ApplyResolution();
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

    private void GameWindow_ClientSizeChanged(object? sender, EventArgs e)
    {
        resolution = new(gameWindow.ClientBounds.Width, gameWindow.ClientBounds.Height);
        ApplyChanges();
    }
}
