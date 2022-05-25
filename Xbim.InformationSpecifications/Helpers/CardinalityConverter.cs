using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xbim.InformationSpecifications.Cardinality;

namespace Xbim.InformationSpecifications.Helpers
{
    internal class CardinalityConverter : JsonConverter<ICardinality>
    {
        private readonly ILogger? logger;

        public CardinalityConverter(ILogger? logger)
        {
            this.logger = logger;
        }

        public override ICardinality? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var readStr = reader.GetString();
                if (readStr is not null) // this is always true because getString() returns null only if tokentype is null, but we know it's String here.
                {
                    if (Enum.TryParse<CardinalityEnum>(readStr, out var cardinalityEnum))
                    {
                        return new SimpleCardinality() { ApplicabilityCardinality = cardinalityEnum };
                    }
                    else
                    {
                        logger?.LogError("Invalid '{val}' value in string based cardinality.", readStr);
                    }
                }
                else
                    logger?.LogError("Invalid null string in string token.");
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                // it has got to be a MinMaxCardinality
                var item = JsonSerializer.Deserialize(ref reader, typeof( MinMaxCardinality), options) as MinMaxCardinality;
                if (item is null)
                {
                    logger?.LogError("Invalid object values when trying to parse MinMaxCardinality.");
                }
                return item;
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, ICardinality value, JsonSerializerOptions options)
        {
            if (value is SimpleCardinality simple)
            {
                writer.WriteStringValue(simple.ApplicabilityCardinality.ToString());
            }
            else if (value is MinMaxCardinality minMax)
            {
                JsonSerializer.Serialize(writer, minMax, options);
            }
            else
            {
                logger?.LogError("Unexpected type {type} in ICardinality serialization.", value.GetType().Name.ToString());
            }
        }
    }
}
