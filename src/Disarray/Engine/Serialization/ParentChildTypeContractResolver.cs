using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Disarray.Engine.Serialization;

/// <summary>
/// Serializes properties whose declaring type is the first generic parameter only if the property type is the second
/// generic parameter (i.e. if <typeparamref name="T"/> is <see cref="Main"/> and <typeparamref name="U"/> is
/// <see cref="Input"/>, then if the current property's declaring type is <see cref="Main"/>, the property will not be
/// writable unless the property's type is <see cref="Input"/>).
/// </summary>
public class ParentChildTypeContractResolver<T, U> : UserSettingsContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        // JSON property returned by Newtonsoft
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        // Serialize other members normally; only conditionally check properties and fail if the property is already
        // not writable
        if (property.Ignored) return property;

        if (property.DeclaringType == typeof(T))
        {
            property.Ignored = property.PropertyType != typeof(U);
        }

        return property;
    }
}