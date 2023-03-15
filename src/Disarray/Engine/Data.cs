using Disarray.Engine.Serialization;
using Disarray.Gameplay.Levels;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Disarray.Engine;

/// <summary>
/// A utility class to apply settings changes to the engine.
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

    public static string ReadTextFromRelativeFile(string relativeFilePath)
    {
        return File.ReadAllText(Path.Combine(DataDirectoryPath, relativeFilePath));
    }

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
