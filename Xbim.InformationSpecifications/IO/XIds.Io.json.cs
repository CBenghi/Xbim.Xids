using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xbim.InformationSpecifications.Helpers;


namespace Xbim.InformationSpecifications
{
	public partial class Xids
	{
		public void SaveAsJson(string destinationFile, ILogger? logger = null)
		{
			if (File.Exists(destinationFile))
				File.Delete(destinationFile);
			using (var s = File.OpenWrite(destinationFile))
			{
				SaveAsJson(s, logger);
			}
		}

		public void SaveAsJson(Stream sw, ILogger? logger = null)
		{
			JsonSerializerOptions options = GetJsonSerializerOptions(logger);
#if DEBUG
			var t = new Utf8JsonWriter(sw, new JsonWriterOptions() { Indented = true });
#else
			var t = new Utf8JsonWriter(sw);
#endif
			JsonSerializer.Serialize(t, this, options);

		}

		internal static JsonSerializerOptions GetJsonSerializerOptions(ILogger? logger)
		{
			JsonSerializerOptions options = new JsonSerializerOptions()
			{
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			};
			var facetConverter = new HeterogenousListConverter<IFacet, ObservableCollection<IFacet>>(
				(nameof(IfcClassificationFacet), typeof(IfcClassificationFacet)),
				(nameof(IfcTypeFacet), typeof(IfcTypeFacet)),
				(nameof(IfcPropertyFacet), typeof(IfcPropertyFacet)),
				(nameof(DocumentFacet), typeof(DocumentFacet)),
				(nameof(IfcRelationFacet), typeof(IfcRelationFacet)),
				(nameof(MaterialFacet), typeof(MaterialFacet)),
				(nameof(PartOfFacet), typeof(PartOfFacet)),
				(nameof(AttributeFacet), typeof(AttributeFacet))
			);
			options.Converters.Add(facetConverter);
			options.Converters.Add(new ValueConstraintConverter());
			options.Converters.Add(new CardinalityConverter(logger));
			options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
			return options;
		}

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

		public static Xids? Finalize(Xids? unpersisted)
		{
			if (unpersisted == null)
				return null;
			foreach (var spec in unpersisted.AllSpecifications())
			{
				spec.SetIds(unpersisted);
			}
			foreach (var facetGroup in unpersisted.FacetRepository.Collection)
			{
				foreach (var facet in facetGroup.Facets)
				{
					if (facet is IRepositoryRef repref)
					{
						repref.SetContextIds(unpersisted);
					}
				}
			}
			return unpersisted;
		}

		public static async Task<Xids?> LoadFromJsonAsync(Stream sourceStream, ILogger? logger = null)
		{
			JsonSerializerOptions options = GetJsonSerializerOptions(logger);
			var t = await JsonSerializer.DeserializeAsync(sourceStream, typeof(Xids), options) as Xids;
			return Finalize(t);
		}

		
	}
}
