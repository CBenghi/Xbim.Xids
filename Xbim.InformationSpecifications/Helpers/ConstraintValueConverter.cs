using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xbim.InformationSpecifications.Helpers
{
	/// <summary>
	/// This class manages the json conversion of ValueConstraint and its <see cref="ValueConstraint.AcceptedValues"/>.
	/// It reuses the converters passed from the main <see cref="JsonSerializerOptions"/> removing itself from the list.
	/// </summary>
	internal class ValueConstraintConverter : JsonConverter<ValueConstraint>
	{
		private static JsonSerializerOptions? _options;
		private readonly ILogger? logger;

		public ValueConstraintConverter(ILogger? logger)
		{
			this.logger = logger;
		}

		private static JsonSerializerOptions GetOptions(JsonSerializerOptions? options)
		{
			if (_options == null)
			{
				options ??= new JsonSerializerOptions();
				_options = new JsonSerializerOptions()
				{
					DefaultIgnoreCondition = options.DefaultIgnoreCondition
				};
				var constraintConverter = new HeterogenousListConverter<IValueConstraintComponent, List<IValueConstraintComponent>>(
					(nameof(ExactConstraint), typeof(ExactConstraint)),
					(nameof(PatternConstraint), typeof(PatternConstraint)),
					(nameof(RangeConstraint), typeof(RangeConstraint)),
					(nameof(StructureConstraint), typeof(StructureConstraint))
					);
				_options.Converters.Add(constraintConverter);
				foreach (var cnv in options.Converters.Where(x => x.GetType() != typeof(ValueConstraintConverter)))
				{
					_options.Converters.Add(cnv);
				}
			}
			return _options;
		}

		/// <summary>
		/// When reading we accept either a full serialization or a plain string
		/// </summary>
		public override ValueConstraint? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions? options)
		{
			ValueConstraint? read = null;
			if (reader.TokenType == JsonTokenType.String)
			{
				var readStr = reader.GetString();
				if (readStr is not null) // this is always true because getString() returns null only if tokentype is null, but it's String here.
					read = readStr; // there's an implicit conversion operator from string
			}
			else
				read = JsonSerializer.Deserialize<ValueConstraint>(ref reader, GetOptions(options));
			return read;
		}

		/// <summary>
		/// When writing we save a single string, if possible, otherwise a full serialization
		/// </summary>
		public override void Write(Utf8JsonWriter writer, ValueConstraint value, JsonSerializerOptions options)
		{
			if (value.IsSingleUndefinedExact(out var exact))
			{
				writer.WriteStringValue(exact);
			}
			else
			{
				JsonSerializer.Serialize(writer, value, GetOptions(options));
			}
		}
	}
}
