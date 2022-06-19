using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Xbim.InformationSpecifications.Helpers
{
    // https://stackoverflow.com/questions/59743382/is-there-a-simple-way-to-manually-serialize-deserialize-child-objects-in-a-custo/59744188
    // note that the old explicit type approach of newtonsoft json was a security concern
    //
    // reading the .NET type name specified as a string within the JSON payload (such as $type metadata property)
    // to create your objects is not recommended since it introduces potential security concerns
    // (see https://github.com/dotnet/corefx/issues/41347#issuecomment-535779492 for more info).
    //

    public class HeterogenousListConverter<TItem, TList> : JsonConverter<TList>

    where TList : IList<TItem>, new()
    where TItem : class
    {
        public HeterogenousListConverter(params (string key, Type type)[] mappings)
        {
            foreach (var (key, type) in mappings)
                KeyTypeLookup.Add(key, type);
        }

        public ReversibleLookup<string, Type> KeyTypeLookup = new();

        public override bool CanConvert(Type typeToConvert)
            => typeof(TList).IsAssignableFrom(typeToConvert);

        public override TList Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Helper function for validating where you are in the JSON    
            static void validateToken(Utf8JsonReader innerReader, JsonTokenType tokenType)
            {
                if (innerReader.TokenType != tokenType)
                    throw new JsonException($"Invalid token: Was expecting a '{tokenType}' token but received a '{innerReader.TokenType}' token");
            }

            validateToken(reader, JsonTokenType.StartArray);

            var results = new TList();

            reader.Read(); // Advance to the first object after the StartArray token. This should be either a StartObject token, or the EndArray token. Anything else is invalid.

            while (reader.TokenType == JsonTokenType.StartObject)
            { // Start of 'wrapper' object

                reader.Read(); // Move to property name
                validateToken(reader, JsonTokenType.PropertyName);

                var typeKey = reader.GetString();
                if (typeKey is null)
                    continue;
                reader.Read(); // Move to start of object (stored in this property)
                validateToken(reader, JsonTokenType.StartObject); // Start of vehicle

                if (KeyTypeLookup.TryGetValue(typeKey, out var concreteItemType))
                {
                    if (JsonSerializer.Deserialize(ref reader, concreteItemType, options) is TItem item)
                        results.Add(item);
                    else
                        throw new JsonException($"Invalid token: Was expecting a '{concreteItemType}' token but received a '{reader.TokenType}' token");
                }
                else
                {
                    throw new JsonException($"Unknown type key '{typeKey}' found");
                }

                reader.Read(); // Move past end of item object
                reader.Read(); // Move past end of 'wrapper' object
            }

            validateToken(reader, JsonTokenType.EndArray);

            return results;
        }

        public override void Write(Utf8JsonWriter writer, TList items, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in items)
            {
                if (item is null)
                    continue;
                var itemType = item.GetType();
                writer.WriteStartObject();

                if (KeyTypeLookup.ReverseLookup.TryGetValue(itemType, out var typeKey))
                {
                    writer.WritePropertyName(typeKey);
                    JsonSerializer.Serialize(writer, item, itemType, options);
                }
                else
                {
                    throw new JsonException($"Unknown type '{itemType.FullName}' found");
                }

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member