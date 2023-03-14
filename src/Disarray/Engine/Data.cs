using Disarray.Engine.Serialization;
using Disarray.Gameplay.Levels;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Serialization;
using Newtonsoft.Json;

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

    static Data()
    {
        GlobalConverters = new JsonConverter[]
        {
            new BlendStateConverter(),
            new DepthStencilStateConverter(),
            new RasterizerStateConverter(),
            new SamplerStateConverter(),
            new ColorJsonConverter(),
            new OrthographicCameraConverter(),
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
    /// The static constructor is always called before an instance of game is created, so this must be called in the
    /// constructor of game to initialize JSON convertors that need an instance of the game class to serialize
    /// correctly.
    /// </summary>
    /// <param name="game">The game instance.</param>
    public static void InitializeConverters(Game game)
    {
        foreach (var converter in GlobalConverters)
        {
            if (converter is IGameConverter gameConverter)
            {
                gameConverter.Game = game;
            }
        }
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
}
