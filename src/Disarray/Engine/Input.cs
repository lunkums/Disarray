using Disarray.Engine.Controllers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Disarray.Engine;

/// <summary>
/// Handles input and provides an API for developers to specify custom actions. Actions are input events (pressed, up,
/// down) with a special meaning related to gameplay.
/// </summary>
public sealed class Input
{
    private readonly List<IController> controllers;

    private Main main;
    private bool gameInFocus;

    public Input()
    {
        MouseController = new();
        KeyboardController = new();
        GamePadController = new();

        controllers = new List<IController>()
        {
            KeyboardController, MouseController
        };
    }

    public MouseController MouseController { get; init; }
    public Point MousePoint => MouseController.Point;
    public Vector2 MousePosition => MouseController.Position;

    public KeyboardController KeyboardController { get; init; }

    public GamePadController GamePadController { get; init; }
    public bool GamePadSupported { get; init; }
    public GamePadDeadZone GamePadDeadZone { get; init; }

    public void Initialize(Main main)
    {
        this.main = main;

        if (GamePadSupported)
        {
            GamePadController.Initialize(PlayerIndex.One, GamePadDeadZone);
            controllers.Add(GamePadController);
        }
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
            && (!GamePadSupported || GamePadController.IsActionDefined(action));
    }

    private void HandleSignals()
    {
        if ((KeyboardController.IsKeyDown(Keys.LeftAlt) || KeyboardController.IsKeyDown(Keys.RightAlt))
            && KeyboardController.IsKeyPressed(Keys.Enter))
        {
            main.Screen.ToggleFullscreen();
        }
    }
}
