using Newtonsoft.Json;

namespace Disarray.Engine.Serialization;

public class FrameTimeToTimeSpanConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(TimeSpan);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        double frameTime = serializer.Deserialize<double>(reader);
        return TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond * frameTime));
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
