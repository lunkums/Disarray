using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Disarray.Engine.Controllers;

public class GamePadController : Controller<GamePadState, Buttons>
{
    private PlayerIndex playerIndex;
    private GamePadDeadZone deadZone;

    public void Initialize(PlayerIndex playerIndex, GamePadDeadZone deadZone)
    {
        this.playerIndex = playerIndex;
        this.deadZone = deadZone;
    }

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
        return GamePad.GetState(playerIndex, deadZone);
    }
}
