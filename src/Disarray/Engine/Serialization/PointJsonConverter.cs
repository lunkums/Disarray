using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Disarray.Engine.Serialization;

public class PointJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var point = (Point) value;
        writer.WriteValue($"{point.X} {point.Y}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        var values = reader.ReadAsMultiDimensional<int>();

        if(values.Length == 2)
            return new Point(values[0], values[1]);

        if (values.Length == 1)
            return new Point(values[0], values[0]);

        throw new InvalidOperationException("Invalid Point");
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Point);
    }
}