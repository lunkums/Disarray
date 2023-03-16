using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;
using Disarray.Gameplay.Levels;
using Microsoft.Xna.Framework;

namespace Disarray.Engine.Serialization;

public class UserSettingsContractResolver : DefaultContractResolver
{
    private readonly Dictionary<string, Predicate<object>> Predicates;

    public UserSettingsContractResolver()
    {
        Predicates = new()
        {
            { "assets", NeverSerialize },
            { "camera", NeverSerialize },
            { "graphics", NeverSerialize },
            { "input", AlwaysSerialize },
            { "physics", NeverSerialize },
            { "renderer", NeverSerialize },
            { "screen", AlwaysSerialize },
            { "world", NeverSerialize },
            { "level", LevelFactory.SerializeConditionally },
        };
    }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);
        PropertyInfo propertyInfo = member as PropertyInfo;
        string propertyName = property.PropertyName.ToLower();

        if (property.DeclaringType == typeof(Main))
        {
            property.ShouldSerialize
                = Predicates.GetValueOrDefault(propertyName, NeverSerialize);
        }
        else if (property.DeclaringType == typeof(Game))
        {
            property.ShouldSerialize = NeverSerialize;
        }
        else if (property.DeclaringType == typeof(Input))
        {
            // Custom logic for serializing input
            property.ShouldSerialize = (obj) =>
            {
                Input input = obj as Input;

                if (propertyName == "gamepadsupported") return false;
                if (propertyName == "gamepaddeadzone") return false;
                if (propertyName == "gamepadcontroller" && !input.GamePadSupported) return false;
                return propertyInfo.GetSetMethod() != null;
            };
        }
        else if (propertyInfo != null)
        {
            property.ShouldSerialize = propertyInfo.GetSetMethod() == null ? NeverSerialize : AlwaysSerialize;
        }

        return property;
    }

    private bool AlwaysSerialize(object obj)
    {
        return true;
    }

    private bool NeverSerialize(object obj)
    {
        return false;
    }
}