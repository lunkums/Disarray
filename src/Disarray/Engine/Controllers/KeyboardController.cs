using Microsoft.Xna.Framework.Input;

namespace Disarray.Engine.Controllers;

public class KeyboardController : Controller<KeyboardState, Keys>
{
    /// <summary>
    /// Return true if the given key is currently being held down.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>Whether the key is being held down.</returns>
    public bool IsKeyDown(Keys key)
    {
        return CurrentState.IsKeyDown(key);
    }

    /// <summary>
    /// Return true if the given key is currently not being held down.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>Whether the key is not being held down.</returns>
    public bool IsKeyUp(Keys key)
    {
        return CurrentState.IsKeyUp(key);
    }

    /// <summary>
    /// Return true if the given key has just been pressed.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>Whether the key has just been pressed.</returns>
    public bool IsKeyPressed(Keys key)
    {
        return CurrentState.IsKeyDown(key) && PreviousState.IsKeyUp(key);
    }

    public override bool IsActionDown(string action)
    {
        return IsKeyDown(ActionMap[action]);
    }

    public override bool IsActionPressed(string action)
    {
        return IsKeyPressed(ActionMap[action]);
    }

    public override bool IsActionUp(string action)
    {
        return IsKeyUp(ActionMap[action]);
    }

    protected override KeyboardState GetState()
    {
        return Keyboard.GetState();
    }
}
