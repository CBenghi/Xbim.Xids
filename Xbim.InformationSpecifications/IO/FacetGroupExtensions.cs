using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Xbim.InformationSpecifications
{
	/// <summary>
	/// Extension methods for FacetGroups persistence and Formatting
	/// </summary>
	public static class FacetGroupExtensions
	{
		/// <summary>
		/// Saves a single facetGroup to file via file name.
		/// The file gets overwritten if it already exists.
		/// </summary>
		/// <param name="groupToSave">The group to persist</param>
		/// <param name="destinationFile">The file name to write to. WARNING: If the file exists it's overwritten without throwing an error.</param>
		/// <param name="logger">optional logging context</param>
		public static void SaveAsJson(this FacetGroup groupToSave, string destinationFile, ILogger? logger = null)
		{
			if (File.Exists(destinationFile))
			{
				var f = new FileInfo(destinationFile);
				logger?.LogWarning("File is being overwritten: {file}", f.FullName);
				File.Delete(destinationFile);
			}
			using var s = File.Create(destinationFile);
			groupToSave.SaveAsJson(s, logger);
		}

		/// <summary>
		/// persists a single facetGroup to a stream
		/// </summary>
		/// <param name="group">the group to persist</param>
		/// <param name="sw">a writeable stream</param>
		/// <param name="logger">optional loggin context</param>
		public static void SaveAsJson(this FacetGroup group, Stream sw, ILogger? logger = null)
		{
			var options = Xids.GetJsonSerializerOptions(logger);
#if DEBUG
			var t = new Utf8JsonWriter(sw, new JsonWriterOptions() { Indented = true });
#else
            var t = new Utf8JsonWriter(sw);
#endif
			JsonSerializer.Serialize(t, group, options);
		}

		/// <summary>
		/// Unpersists a <see cref="FacetGroup"/> instance from a file by file name. 
		/// </summary>
		/// <param name="sourceFile">the file to read</param>
		/// <param name="logger">optional logging context</param>
		/// <returns>null in case of errors, a loaded group otherwise</returns>
		public static FacetGroup? LoadFromJson(string sourceFile, ILogger? logger = null)
		{
			if (!File.Exists(sourceFile))
			{
				var f = new FileInfo(sourceFile);
				logger?.LogError("Json file not found: '{sourceFile}'", f.FullName);
				return null;
			}
			var allfile = File.ReadAllText(sourceFile);
			var t = JsonSerializer.Deserialize<FacetGroup>(allfile, Xids.GetJsonSerializerOptions(logger));
			return t;
		}

		/// <summary>
		/// Unpersists a <see cref="FacetGroup"/> instance from a stream 
		/// </summary>
		/// <param name="sourceStream">a readable stream</param>
		/// <param name="logger">optional logging context</param>
		/// <returns>null in case of errors, a loaded group otherwise</returns>
		public static async Task<FacetGroup?> LoadFromJsonAsync(Stream sourceStream, ILogger? logger = null)
		{
			JsonSerializerOptions options = Xids.GetJsonSerializerOptions(logger);
			var t = await JsonSerializer.DeserializeAsync(sourceStream, typeof(FacetGroup), options) as FacetGroup;
			return t;
		}

		/// <summary>
		/// Short textual description of the applicability of this group of Facets
		/// </summary>
		/// <returns>The stated group named if supplied, else a generated description of the applicability, if meaningful, otherwise the <see cref="FacetGroup.Undefined"/> constant.</returns>
		public static string GetApplicabilityDescription(this FacetGroup group)
		{
			if (!string.IsNullOrWhiteSpace(group.Name))
				return $"{group.Name}";
			if (group.Facets.Any())
			{
				return "All elements " + string.Join(" AND ", group.Facets.Select((x, i) => x.ApplicabilityDescription));
			}
			return FacetGroup.Undefined;
		}

		/// <summary>
		/// Short textual description of the requirement of this group of Facets
		/// </summary>
		/// <returns>The stated group named if supplied, else a generated description of the requirement, if meaningful, otherwise the <see cref="FacetGroup.Undefined"/> constant.</returns>
		public static string GetRequirementDescription(this FacetGroup group)
		{
			if (!string.IsNullOrWhiteSpace(group.Name))
				return $"{group.Name}";
			if (group.Facets.Any())
			{
				return "should " + string.Join(" AND should ", group.Facets.Select((x, i) => group.HandleCardinality(x.RequirementDescription, i, "have ")));
			}
			return FacetGroup.Undefined;
		}

		private static string HandleCardinality(this FacetGroup group, string requirement, int index, string clause = "")
		{
			if (group.RequirementOptions == null)
			{
				return $"{clause}{requirement}"; // default to Expected
			}
			if (group.RequirementOptions.Count > index)
			{
				var cardinality = group.RequirementOptions[index]?.RelatedFacetCardinality;
				return cardinality switch
				{
					RequirementCardinalityOptions.Cardinality.Prohibited => $"NOT {clause}{requirement}",
					RequirementCardinalityOptions.Cardinality.Optional => $"OPTIONALLY {clause}{requirement}",
					RequirementCardinalityOptions.Cardinality.Expected => $"{clause}{requirement}",
					null => $"{clause}{requirement}",
					_ => throw new NotImplementedException(cardinality.ToString()),
				};
			}
			else
			{
				throw new IndexOutOfRangeException($"Insufficient RequirementOptions for the number of Facets: {index}");
			}
		}
	}
}
