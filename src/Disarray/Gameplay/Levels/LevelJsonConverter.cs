using Newtonsoft.Json;

namespace Disarray.Gameplay.Levels;

public class LevelJsonConverter<T> : JsonConverter, ILevelConverter where T : ILevel
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ILevel);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        return serializer.Deserialize<T>(reader);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException("You can fill this out if you'd like");
    }
}
