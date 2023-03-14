using Newtonsoft.Json;
using System;

namespace Disarray.Engine.Serialization;

public abstract class StringMatchConverter<T> : JsonConverter
{
    private string value;

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(T);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        value = (string)reader.Value;
        return GetValueFromLowercaseString(value.ToLower());
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(value);
    }

    public abstract T GetValueFromLowercaseString(string str);
}
