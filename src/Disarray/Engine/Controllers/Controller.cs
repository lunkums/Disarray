namespace Disarray.Engine.Controllers;

public abstract class Controller<T, V> : IController
{
    public T CurrentState { get; private set; }
    public T PreviousState { get; private set; }
    public Dictionary<string, V> ActionMap { get; set; } = new();

    public void Update()
    {
        PreviousState = CurrentState;
        CurrentState = GetState();
    }

    public bool IsActionDefined(string action)
    {
        return ActionMap.ContainsKey(action);
    }

    public abstract bool IsActionDown(string action);
    public abstract bool IsActionPressed(string action);
    public abstract bool IsActionUp(string action);
    protected abstract T GetState();
}
