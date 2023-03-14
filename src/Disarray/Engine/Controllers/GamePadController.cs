using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Disarray.Engine.Controllers;

public class GamePadController : Controller<GamePadState, Buttons>
{
    public PlayerIndex PlayerIndex { get; set; }
    public GamePadDeadZone GamePadDeadZone { get; set; }

    public bool IsButtonDown(Buttons button)
    {
        return CurrentState.IsButtonDown(button);
    }

    public bool IsButtonPressed(Buttons button)
    {
        return CurrentState.IsButtonDown(button);
    }

    public bool IsButtonUp(Buttons button)
    {
        return CurrentState.IsButtonDown(button);
    }

    public override bool IsActionDown(string action)
    {
        return IsButtonDown(ActionMap[action]);
    }

    public override bool IsActionPressed(string action)
    {
        return IsButtonPressed(ActionMap[action]);
    }

    public override bool IsActionUp(string action)
    {
        return IsButtonUp(ActionMap[action]);
    }

    protected override GamePadState GetState()
    {
        return GamePad.GetState(PlayerIndex, GamePadDeadZone);
    }
}
