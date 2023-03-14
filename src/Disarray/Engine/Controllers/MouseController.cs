using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Disarray.Engine.Controllers;

public class MouseController : Controller<MouseState, MouseController.MouseButton>
{
    public enum MouseButton
    {
        LeftMouseButton,
        MiddleMouseButton,
        RightMouseButton,
        SideMouseButton1,
        SideMouseButton2
    }

    public Point Point => new(CurrentState.X, CurrentState.Y);
    public Vector2 Position => new(CurrentState.X, CurrentState.Y);

    public bool IsButtonUp(MouseButton button)
    {
        return GetButtonState(CurrentState, button) == ButtonState.Released;
    }

    public bool IsButtonDown(MouseButton button)
    {
        return GetButtonState(CurrentState, button) == ButtonState.Pressed;
    }

    public bool IsButtonPressed(MouseButton button)
    {
        return GetButtonState(CurrentState, button) == ButtonState.Pressed
            && GetButtonState(PreviousState, button) == ButtonState.Released;
    }

    public override bool IsActionUp(string action)
    {
        return IsButtonUp(ActionMap[action]);
    }

    public override bool IsActionDown(string action)
    {
        return IsButtonDown(ActionMap[action]);
    }

    public override bool IsActionPressed(string action)
    {
        return IsButtonPressed(ActionMap[action]);
    }

    protected override MouseState GetState()
    {
        return Mouse.GetState();
    }

    private static ButtonState GetButtonState(MouseState mouseState, MouseButton mouseButton)
    {
        switch (mouseButton)
        {
            case MouseButton.LeftMouseButton:
                return mouseState.LeftButton;
            case MouseButton.MiddleMouseButton:
                return mouseState.MiddleButton;
            case MouseButton.RightMouseButton:
                return mouseState.RightButton;
            case MouseButton.SideMouseButton1:
                return mouseState.XButton1;
            case MouseButton.SideMouseButton2:
                return mouseState.XButton2;
            default:
                Debug.Fail("Undefined mouse button");
                return ButtonState.Released;
        }
    }
}
