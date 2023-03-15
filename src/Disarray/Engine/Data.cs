using Disarray.Engine.Serialization;
using Disarray.Gameplay.Levels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Disarray.Engine;

/// <summary>
/// Manages settings, file I/O, and serialization for the game.
/// </summary>
public static class Data
{
    /// <summary>
    /// The path to the data directory.
    /// </summary>
    public static readonly string DataDirectoryPath = "data/";

    public static JsonConverter[] GlobalConverters;
    public static JsonSerializerSettings GlobalSerializerSettings;

    private static Main Main;

    static Data()
    {
        GlobalConverters = new JsonConverter[]
        {
            new StringEnumConverter(new DefaultNamingStrategy(), true),
            new BlendStateConverter(),
            new DepthStencilStateConverter(),
            new RasterizerStateConverter(),
            new SamplerStateConverter(),
            new ColorJsonConverter(),
            new Vector2JsonConverter(),
            new PointJsonConverter(),
            LevelFactory.LevelJsonConverter
        };
        GlobalSerializerSettings = new JsonSerializerSettings()
        {
            Converters = GlobalConverters
        };
    }

    /// <summary>
    /// Provide a custom JSON converter with which to serialize the level.
    /// </summary>
    /// <typeparam name="T">The type of the level to serialize.</typeparam>
    /// <param name="jsonConverter">The new level converter.</param>
    public static void OverrideLevelConverter<T>(LevelJsonConverter<T> jsonConverter) where T : ILevel
    {
        for (int i = 0; i < GlobalConverters.Length; i++)
        {
            if (GlobalConverters[i] is ILevelConverter)
            {
                GlobalConverters[i] = jsonConverter;
                return;
            }
        }
    }

    /// <summary>
    /// Initialize the data class.
    /// </summary>
    /// <param name="main">The game instance.</param>
    public static void Initialize(Main main)
    {
        Main = main;
        Main.Exiting += OnGameExiting;
    }

    /// <summary>
    /// Load the JSON file into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize the file as.</typeparam>
    /// <param name="filePath">The path to the file, including the file name with its extension.</param>
    /// <param name="relativeToDataDir">Whether the the given <paramref name="filePath"/> is relative to the data directory.</param>
    /// <returns></returns>
    public static T LoadFromFilePath<T>(string filePath, bool relativeToDataDir = true)
    {
        string jsonBlob;

        if (relativeToDataDir)
        {
            jsonBlob = ReadTextFromRelativeFile(filePath);
        }
        else
        {
            jsonBlob = File.ReadAllText(filePath);
        }

        return JsonConvert.DeserializeObject<T>(jsonBlob, GlobalSerializerSettings);
    }

    /// <summary>
    /// Apply the systems and game settings as overrides to the main class.
    /// </summary>
    /// <param name="main">The main game class.</param>
    public static void ApplyUserSettings(Main game)
    {
        string userSettingsBlob = ReadTextFromRelativeFile("user_settings.json");
        JsonConvert.PopulateObject(userSettingsBlob, game, GlobalSerializerSettings);
        game.Graphics.ApplyChanges();
    }

    /// <summary>
    /// Read all text from the path of the file relative to the data directory.
    /// </summary>
    /// <param name="relativeFilePath">The file path relative to the data directory.</param>
    /// <returns>The raw contents of the file.</returns>
    public static string ReadTextFromRelativeFile(string relativeFilePath)
    {
        return File.ReadAllText(Path.Combine(DataDirectoryPath, relativeFilePath));
    }

    /// <summary>
    /// Serialize the main game instance to JSON and save that to the user settings file.
    /// </summary>
    public static void SaveUserSettings()
    {
        string userSettings;
        var temp = GlobalSerializerSettings.ContractResolver;

        GlobalSerializerSettings.ContractResolver = new UserSettingsContractResolver();
        userSettings = JsonConvert.SerializeObject(Main, Formatting.Indented, GlobalSerializerSettings);
        GlobalSerializerSettings.ContractResolver = temp;

        File.WriteAllText(Path.Combine(DataDirectoryPath, "user_settings.json"), userSettings);
    }

    private static void OnGameExiting(object? sender, EventArgs e)
    {
        SaveUserSettings();
    }
}
