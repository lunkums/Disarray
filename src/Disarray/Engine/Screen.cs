using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Disarray.Engine;

public class Screen
{
    private GraphicsDeviceManager graphicsDeviceManager;
    private GameWindow gameWindow;

    private int previousWindowWidth;
    private int previousWindowHeight;

    public event EventHandler<EventArgs> ResolutionChanged;

    public bool IsFullScreen => graphicsDeviceManager.IsFullScreen;
    public bool IsBorderless => gameWindow.IsBorderlessEXT;

    public void Initialize(Main main)
    {
        graphicsDeviceManager = main.Graphics;

        gameWindow = main.Window;
        previousWindowWidth = gameWindow.ClientBounds.Width;
        previousWindowHeight = gameWindow.ClientBounds.Height;
        gameWindow.ClientSizeChanged += GameWindow_ClientSizeChanged;
    }

    /// <summary>
    /// Set the resolution of the window to the given values.
    /// </summary>
    /// <param name="width">The new width of the window.</param>
    /// <param name="height">The new height of the window.</param>
    public void SetResolution(int width, int height)
    {
        graphicsDeviceManager.PreferredBackBufferWidth = width;
        graphicsDeviceManager.PreferredBackBufferHeight = height;
        graphicsDeviceManager.ApplyChanges();
        ResolutionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ToggleFullscreen()
    {
        int targetWidth;
        int targetHeight;

        if (IsFullScreen)
        {
            targetWidth = previousWindowWidth;
            targetHeight = previousWindowHeight;
        }
        else
        {
            previousWindowWidth = gameWindow.ClientBounds.Width;
            previousWindowHeight = gameWindow.ClientBounds.Height;
            targetWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            targetHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }

        graphicsDeviceManager.IsFullScreen = !graphicsDeviceManager.IsFullScreen;
        SetResolution(targetWidth, targetHeight);
    }

    public void ToggleBorderless()
    {
        gameWindow.IsBorderlessEXT = !IsBorderless;
        graphicsDeviceManager.ApplyChanges();
    }

    private void GameWindow_ClientSizeChanged(object? sender, EventArgs e)
    {
        SetResolution(gameWindow.ClientBounds.Width, gameWindow.ClientBounds.Height);
    }
}
