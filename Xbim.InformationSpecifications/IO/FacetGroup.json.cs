using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Xbim.InformationSpecifications
{
    public static class FacetGroupExtensions
    {
        public static void SaveAsJson(this FacetGroup group, string destinationFile, ILogger? logger = null)
        {
            if (File.Exists(destinationFile))
                File.Delete(destinationFile);
            using var s = File.OpenWrite(destinationFile);
            group.SaveAsJson(s, logger);
        }
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

        public static FacetGroup? LoadFromJson(string sourceFile, ILogger? logger = null)
        {
            if (!File.Exists(sourceFile))
            {
                logger?.LogError("Json file not found: '{sourceFile}'", sourceFile);
                return null;
            }
            var allfile = File.ReadAllText(sourceFile);
            var t = JsonSerializer.Deserialize<FacetGroup>(allfile, Xids.GetJsonSerializerOptions(logger));
            return t;
        }

        public static async Task<FacetGroup?> LoadFromJsonAsync(Stream sourceStream, ILogger? logger = null)
        {
            JsonSerializerOptions options = Xids.GetJsonSerializerOptions(logger);
            var t = await JsonSerializer.DeserializeAsync(sourceStream, typeof(FacetGroup), options) as FacetGroup;
            return t;
        }
    }
}
