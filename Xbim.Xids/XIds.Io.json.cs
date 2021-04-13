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
		public void SaveAsJson(string destinationFile)
		{
			if (File.Exists(destinationFile))
				File.Delete(destinationFile);
			using (var s = File.OpenWrite(destinationFile))
			{
				SaveAsJson(s);
			}
		}

		public void SaveAsJson(Stream sw)
		{
			JsonSerializerOptions options = GetJsonSerializerOptions();
#if DEBUG
			var t = new Utf8JsonWriter(sw, new JsonWriterOptions() { Indented = true });
#else
			var t = new Utf8JsonWriter(sw);
#endif
			JsonSerializer.Serialize(t, this, options);

		}

		private static JsonSerializerOptions GetJsonSerializerOptions()
		{
			JsonSerializerOptions options = new JsonSerializerOptions()
			{
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			};
			var facetConverter = new HeterogenousListConverter<IFacet, ObservableCollection<IFacet>>(
				(nameof(IfcClassificationFacet), typeof(IfcClassificationFacet)),
				(nameof(IfcTypeFacet), typeof(IfcTypeFacet)),
				(nameof(IfcPropertyFacet), typeof(IfcPropertyFacet)),
				(nameof(MaterialFacet), typeof(MaterialFacet))
			);
			var constraintConverter = new HeterogenousListConverter<IValueConstraint, List<IValueConstraint>>(
				(nameof(ExactConstraint), typeof(ExactConstraint)),
				(nameof(PatternConstraint), typeof(PatternConstraint)),
				(nameof(RangeConstraint), typeof(RangeConstraint)),
				(nameof(StructureConstraint), typeof(StructureConstraint))
				);

			options.Converters.Add(facetConverter);
			options.Converters.Add(constraintConverter);
			options.Converters.Add(
				new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
				);
			return options;
		}

		public static Xids LoadFromJson(string sourceFile)
		{
			// todo: 2021: json perhaps not efficient for large files.
			if (!File.Exists(sourceFile))
				throw new FileNotFoundException($"File missing: '{sourceFile}'");
			var allfile = File.ReadAllText(sourceFile);
			var t = JsonSerializer.Deserialize<Xids>(allfile, GetJsonSerializerOptions());
			return Finalize(t);
		}

		public static Xids Finalize(Xids unpersisted)
		{
			if (unpersisted == null)
				return null;
			foreach (var req in unpersisted.AllSpecifications())
			{
				req.SetIds(unpersisted);
			}
			return unpersisted;
		}

		public static async Task<Xids> LoadFromJsonAsync(Stream sourceStream)
		{
			JsonSerializerOptions options = GetJsonSerializerOptions();
			var t = await JsonSerializer.DeserializeAsync(sourceStream, typeof(Xids), options) as Xids;
			return Finalize(t);
		}

		
	}
}
