using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Xbim.InformationSpecifications.Helpers;


namespace Xbim.InformationSpecifications
{
	public partial class Xids
	{
		/// <summary>
		/// Persists the instance to a file by file name. See the <see cref="LoadFromJson(string, ILogger?)"/> to unpersist.
		/// </summary>
		/// <param name="destinationFile">The file name to write to. WARNING: If the file exists it's overwritten without throwing an error.</param>
		/// <param name="logger">The logging context to be notified.</param>
		public void SaveAsJson(string destinationFile, ILogger? logger = null)
		{
			if (File.Exists(destinationFile))
			{
				var f = new FileInfo(destinationFile);
				logger?.LogWarning("File is being overwritten: {file}", f.FullName);
				File.Delete(destinationFile);
			}
			using var s = File.Create(destinationFile);
			SaveAsJson(s, logger);
		}

		/// <summary>
		/// Persists the instance to a stream.
		/// </summary>
		/// <param name="sw">Any writeable stream</param>
		/// <param name="logger">The logging context to be notified.</param>
		public void SaveAsJson(Stream sw, ILogger? logger = null)
		{
			JsonSerializerOptions options = GetJsonSerializerOptions(logger);
#if DEBUG
			var t = new Utf8JsonWriter(sw, new JsonWriterOptions() { Indented = true });
#else
			var t = new Utf8JsonWriter(sw);
#endif
			Cleanup();
			JsonSerializer.Serialize(t, this, options);
		}

		/// <summary>
		/// The options for the json serializer. 
		/// This is useful for both serialization and deserialization, so that they are consistent. 
		/// It also contains some custom converters to handle the specific needs of the XIDS model, 
		/// such as the heterogenous list of facets and the value constraints.
		/// </summary>
		/// <param name="logger">The logging context to be notified.</param>
		/// <returns>The configured JsonSerializerOptions.</returns>
		public static JsonSerializerOptions GetJsonSerializerOptions(ILogger? logger)
		{
			var options = new JsonSerializerOptions()
			{
				// DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
			};
			var t = true;
			if (t)
			{
				options.TypeInfoResolver = new DefaultJsonTypeInfoResolver()
				{
					Modifiers =
					{
						static typeInfo =>
						{
							foreach (var prop in typeInfo.Properties)
							{
								if (prop.PropertyType == typeof(string))
								{
									var existing = prop.ShouldSerialize;
									prop.ShouldSerialize = existing is null
										? (_, val) => val is string s && !string.IsNullOrEmpty(s)
										: (obj, val) => existing(obj, val) && val is string s && !string.IsNullOrEmpty(s);
								}
							}
						}
					}
				};
			}

			var iFacetListConverter = new HeterogenousListConverter<IFacet, IList<IFacet>>(
				(nameof(IfcClassificationFacet), typeof(IfcClassificationFacet)),
				(nameof(IfcTypeFacet), typeof(IfcTypeFacet)),
				(nameof(IfcPropertyFacet), typeof(IfcPropertyFacet)),
				(nameof(DocumentFacet), typeof(DocumentFacet)),
				(nameof(IfcRelationFacet), typeof(IfcRelationFacet)),
				(nameof(MaterialFacet), typeof(MaterialFacet)),
				(nameof(PartOfFacet), typeof(PartOfFacet)),
				(nameof(AttributeFacet), typeof(AttributeFacet))
			);
			options.Converters.Add(iFacetListConverter);
			options.Converters.Add(new ValueConstraintConverter(logger));
			options.Converters.Add(new CardinalityConverter(logger));
			options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
			return options;
		}

		/// <summary>
		/// Unpersists a XIDS instance from a file by file name. See the <see cref="SaveAsJson(string, ILogger?)"/> to persist.
		/// </summary>
		/// <param name="sourceFile">File name to load the information from</param>
		/// <param name="logger">The logging context to be notified.</param>
		/// <returns>null if any critical error occurs</returns>
		public static Xids? LoadFromJson(string sourceFile, ILogger? logger = null)
		{
			// todo: 2021: json perhaps not efficient for large files.
			if (!File.Exists(sourceFile))
			{
				logger?.LogError("Json file not found: '{sourceFile}'", sourceFile);
				return null;
			}
			var allfile = File.ReadAllText(sourceFile);
			var t = JsonSerializer.Deserialize<Xids>(allfile, GetJsonSerializerOptions(logger));
			return Finalize(t);
		}

		/// <summary>
		/// Unpersists an instance from a stream in json format. 
		/// </summary>
		/// <param name="sourceStream">a readable json stream.</param>
		/// <param name="logger">The logging context to be notified.</param>
		/// <returns>null if any critical error occurs</returns>
		public static async Task<Xids?> LoadFromJsonAsync(Stream sourceStream, ILogger? logger = null)
		{
			JsonSerializerOptions options = GetJsonSerializerOptions(logger);
			var t = await JsonSerializer.DeserializeAsync(sourceStream, typeof(Xids), options) as Xids;
			return Finalize(t);
		}

	}
}
