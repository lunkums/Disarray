namespace Disarray.Engine.Controllers;

public interface IController
{
    void Update();
    bool IsActionUp(string action);
    bool IsActionDown(string action);
    bool IsActionPressed(string action);
    bool IsActionDefined(string action);
}
