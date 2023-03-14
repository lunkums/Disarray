namespace Disarray.Gameplay.Levels;

/// <summary>
/// Override this class to create a custom default level.
/// </summary>
public static class LevelFactory
{
    // Override me if using a custom level class
    public static readonly LevelJsonConverter<Level> LevelJsonConverter = new();

    public static ILevel Create()
    {
        return new Level();
    }
}
