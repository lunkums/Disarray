using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Disarray.Gameplay.Levels;

/// <summary>
/// Defines what the first level of the game is and how subsequent levels will be loaded.
/// </summary>
public static class LevelLoader
{
    /// To implement a custom level, change every instance "Level" in this class with the type of your
    /// own level class. Change the implementation of "ShouldIgnoreMember(...)" to define which properties are
    /// serialized when the game saves.

    /// <summary>
    /// The JSON converter that determines how a level is loaded. Override if using a custom level class.
    /// </summary>
    public static readonly LevelJsonConverter<Level> LevelJsonConverter = new();

    public static ILevel Create()
    {
        return new Level();
    }

    /// <summary>
    /// Returns whether the given member should be ignored when saving the game.
    /// </summary>
    /// <param name="member"></param>
    /// <param name="memberSerialization"></param>
    /// <param name="property">The property whose delcaring type is <see cref="ILevel"/>.</param>
    public static bool ShouldIgnoreMember(MemberInfo member, MemberSerialization memberSerialization, JsonProperty property)
    {
        return true;
    }
}
