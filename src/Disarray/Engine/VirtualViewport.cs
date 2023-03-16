using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Disarray.Engine;

public class VirtualViewport
{
    private GraphicsDevice graphicsDevice;
    private GameWindow gameWindow;

    // The render target should always be created at least once
    private bool shouldCreateRenderTarget = true;

    /// <summary>
    /// The method by which to resize the viewport when the resolution changes.
    /// </summary>
    public enum ViewportResizeMethod
    {
        Window = 0,
        Stretch = 1,
        PreserveAspectRatio = 2
    }

    public RenderTarget2D RenderTarget { get; private set; }
    public Rectangle Destination { get; private set; }
    public Matrix Scale { get; private set; }
    public ViewportResizeMethod ResizeMethod { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int HorizontalBleed { get; set; }
    public int VerticalBleed { get; set; }
    public Color ClearColor { get; set; }
    public Color DrawColor { get; set; }

    public void LoadContent(Main main)
    {
        graphicsDevice = main.GraphicsDevice;
        gameWindow = main.Window;

        Resize();
        main.Screen.ResolutionChanged += OnResolutionChanged;
    }
    
    /// <summary>
    /// Create the render target.
    /// </summary>
    private void CreateRenderTarget()
    {
        // Only create the render target
        switch (ResizeMethod)
        {
            case ViewportResizeMethod.Window:
                CreateRenderTarget(gameWindow.ClientBounds.Width, gameWindow.ClientBounds.Height);
                shouldCreateRenderTarget = true;
                break;
            case ViewportResizeMethod.Stretch:
                if (!shouldCreateRenderTarget) return;
                CreateRenderTarget(Width, Height);
                shouldCreateRenderTarget = false;
                break;
            case ViewportResizeMethod.PreserveAspectRatio:
                if (!shouldCreateRenderTarget) return;
                CreateRenderTarget(Width, Height);
                shouldCreateRenderTarget = false;
                break;
        }
    }

    /// <summary>
    /// Create the render target with the given width and height.
    /// </summary>
    /// <param name="targetWidth">The width of the new render target.</param>
    /// <param name="targetHeight">The height of the new render target.</param>
    private void CreateRenderTarget(int targetWidth, int targetHeight)
    {
        // Dispose the old render target if there is one
        RenderTarget?.Dispose();

        PresentationParameters pParams = graphicsDevice.PresentationParameters;
        RenderTarget = new RenderTarget2D(graphicsDevice, targetWidth, targetHeight, false, pParams.BackBufferFormat,
            pParams.DepthStencilFormat);
    }

    /// <summary>
    /// Given the bounds of the game window, calculate the destination rectangle of the render target (which should be
    /// centered about the game window and boxed out to maintain its virtual aspect ratio.
    /// </summary>
    private void Resize()
    {
        switch (ResizeMethod)
        {
            case ViewportResizeMethod.Window:
                ResizeToWindow();
                break;
            case ViewportResizeMethod.Stretch:
                ResizeWithStretch();
                break;
            case ViewportResizeMethod.PreserveAspectRatio:
                ResizeToPreserveAspectRatio();
                break;
            default:
                throw new NotImplementedException();
        }

        CreateRenderTarget();
    }

    /// <summary>
    /// Resize the destination rectangle to fit the window.
    /// </summary>
    private void ResizeToWindow()
    {
        Rectangle clientBounds = gameWindow.ClientBounds;
        Destination = new(0, 0, clientBounds.Width, clientBounds.Height);
        Scale = Matrix.CreateScale(1, 1, 1);
    }

    /// <summary>
    /// Resize the destination rectangle to fit the window.
    /// </summary>
    private void ResizeWithStretch()
    {
        Rectangle clientBounds = gameWindow.ClientBounds;
        Destination = new(0, 0, clientBounds.Width, clientBounds.Height);
        Scale = Matrix.CreateScale((float)clientBounds.Width / Width, (float)clientBounds.Height / Height, 1);
    }

    /// <summary>
    /// Resize the destination rectangle to preserve the aspect ratio.
    /// </summary>
    private void ResizeToPreserveAspectRatio()
    {
        var clientBounds = gameWindow.ClientBounds;

        var worldScaleX = (float)clientBounds.Width / Width;
        var worldScaleY = (float)clientBounds.Height / Height;

        var safeScaleX = (float)clientBounds.Width / (Width - HorizontalBleed);
        var safeScaleY = (float)clientBounds.Height / (Height - VerticalBleed);

        var worldScale = MathHelper.Max(worldScaleX, worldScaleY);
        var safeScale = MathHelper.Min(safeScaleX, safeScaleY);
        var scale = MathHelper.Min(worldScale, safeScale);

        var realWidth = (int)(scale * Width + 0.5f);
        var realHeight = (int)(scale * Height + 0.5f);

        var x = clientBounds.Width / 2 - realWidth / 2;
        var y = clientBounds.Height / 2 - realHeight / 2;

        Destination = new(x, y, realWidth, realHeight);
        Scale = Matrix.CreateScale(scale, scale, 1);
    }

    /// <summary>
    /// Resize the render texture if the resolution changes.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event args.</param>
    private void OnResolutionChanged(object? sender, EventArgs e)
    {
        Resize();
    }
}
