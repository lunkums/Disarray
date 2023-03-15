using Disarray.Engine.Controllers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Disarray.Engine;

public sealed class Input : IDisposable
{
    private static int controllerCount = 0;

    private readonly IEnumerable<IController> controllers;

    private Main main;
    private bool gameInFocus;

    public Input()
    {
        MouseController = new();
        KeyboardController = new();
        GamePadController = new()
        {
            PlayerIndex = PlayerIndexFromInt(controllerCount++)
        };

        controllers = new IController[]
        {
            KeyboardController, MouseController, GamePadController
        };
    }

    public MouseController MouseController { get; private set; }
    public Point MousePoint => MouseController.Point;
    public Vector2 MousePosition => MouseController.Position;

    public KeyboardController KeyboardController { get; private set; }

    public GamePadController GamePadController { get; private set; }
    public bool GamePadSupported { get; private set; }

    public void Dispose()
    {
        controllerCount--;
    }

    public void Initialize(Main main)
    {
        this.main = main;
    }

    /// <summary>
    /// Update the state of the input, including whether the game is in focus.
    /// </summary>
    /// <param name="gameInFocus">Whether the game is in focus.</param>
    public void Update(bool gameInFocus)
    {
        this.gameInFocus = gameInFocus;

        foreach (var controller in controllers)
        {
            controller.Update();
        }

        HandleSignals();
    }

    /*
     * Actions 
     */

    /// <summary>
    /// Return true if the given action is currently not being held down.
    /// </summary>
    /// <param name="action">The action to check.</param>
    /// <returns>Whether the action is not being held down.</returns>
    public bool IsActionUp(string action)
    {
        Debug.Assert(IsActionDefined(action), $"{action} not defined");
        return gameInFocus && controllers.Any((c) => c.IsActionDefined(action) && c.IsActionUp(action));
    }

    /// <summary>
    /// Return true if the given action is currently being held down.
    /// </summary>
    /// <param name="action">The action to check.</param>
    /// <returns>Whether the action is being held down.</returns>
    public bool IsActionDown(string action)
    {
        Debug.Assert(IsActionDefined(action), $"{action} not defined");
        return gameInFocus && controllers.Any((c) => c.IsActionDefined(action) && c.IsActionDown(action));
    }

    /// <summary>
    /// Return true if the given action is currently being held down.
    /// </summary>
    /// <param name="action">The action to check.</param>
    /// <returns>Whether the action is being held down.</returns>
    public bool IsActionPressed(string action)
    {
        Debug.Assert(IsActionDefined(action), $"{action} not defined");
        return gameInFocus && controllers.Any((c) => c.IsActionDefined(action) && c.IsActionPressed(action));
    }

    /*
     * Helper
     */

    /// <summary>
    /// Return true if the given action is defined for all supported input controllers.
    /// </summary>
    /// <param name="action">The action to check.</param>
    /// <returns>Whether the action is defined for all input controllers.</returns>
    private bool IsActionDefined(string action)
    {
        return (KeyboardController.IsActionDefined(action) || MouseController.IsActionDefined(action))
            && (GamePadController.IsActionDefined(action) || !GamePadSupported);
    }

    private void HandleSignals()
    {
        if ((KeyboardController.IsKeyDown(Keys.LeftAlt) || KeyboardController.IsKeyDown(Keys.RightAlt))
            && KeyboardController.IsKeyPressed(Keys.Enter))
        {
            main.Screen.ToggleFullscreen();
        }
    }

    private static PlayerIndex PlayerIndexFromInt(int index)
    {
        switch (index)
        {
            case 0:
                return PlayerIndex.One;
            case 1:
                return PlayerIndex.Two;
            case 2:
                return PlayerIndex.Three;
            case 3:
                return PlayerIndex.Four;
            default:
                throw new Exception("Invalid player index specified");
        }
    }
}
