using DefaultEcs;
using Disarray.Gameplay.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Disarray.Engine.Serialization;

/// <summary>
/// Indicates the way <see cref="Main"/> should be serialized, determining how/which user settings are generated.
/// </summary>
public class UserSettingsContractResolver : DefaultContractResolver
{
    private readonly Dictionary<Type, bool> PropertyIgnoreStatus;

    public UserSettingsContractResolver()
    {
        PropertyIgnoreStatus = new()
        {
            { typeof(Assets), true },
            { typeof(Camera), true },
            { typeof(IGraphicsDeviceManager), true },
            { typeof(GraphicsDeviceManager), true },
            { typeof(GraphicsDevice), true },
            { typeof(Input), false },
            { typeof(Physics), true },
            { typeof(Renderer), true },
            { typeof(Screen), false },
            { typeof(World), true }
        };
    }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        // JSON property returned by Newtonsoft
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        // C# property obtained through System.Reflection 
        PropertyInfo propertyInfo = member as PropertyInfo;

        // Serialize other members normally; only conditionally check properties
        if (propertyInfo == null) return property;

        if (property.DeclaringType == typeof(Main))
        {
            property.Ignored = PropertyIgnoreStatus.GetValueOrDefault(property.PropertyType, true);
        }
        if (property.DeclaringType == typeof(Game))
        {
            property.Ignored = true;
        }

        // The following properties have custom serialization logic
        if (property.DeclaringType == typeof(Input))
        {
            property.Ignored = ShouldSerializeChildOfInput(property);
        }
        if (property.DeclaringType == typeof(ILevel))
        {
            property.Ignored = LevelLoader.ShouldIgnoreMember(member, memberSerialization, property);
        }

        // Always ignore a property without a public or "init" setter
        if (propertyInfo != null)
        {
            property.Ignored = property.Ignored || propertyInfo.GetSetMethod() == null;
        }

        return property;
    }

    /// <summary>
    /// Determine, through custom logic, whether the property is writable given that its declared type is
    /// <see cref="Input"/>.
    /// </summary>
    /// <param name="property">The JSON property to write.</param>
    /// <returns>false if the property is writable, true otherwise.</returns>
    private bool ShouldSerializeChildOfInput(JsonProperty property)
    {
        string propertyName = property.PropertyName.ToLower();

        if ("gamepadsupported".Equals(propertyName, StringComparison.OrdinalIgnoreCase)) return true;
        if ("gamepaddeadzone".Equals(propertyName, StringComparison.OrdinalIgnoreCase)) return true;
        if ("gamepadcontroller".Equals(propertyName, StringComparison.OrdinalIgnoreCase)) return true;
        if ("gamepadcontroller".Equals(propertyName, StringComparison.OrdinalIgnoreCase))
        {
            property.ShouldSerialize = (obj) =>
            {
                return obj is Input input && input.GamePadSupported;
            };
        }

        return property.Ignored;
    }
}