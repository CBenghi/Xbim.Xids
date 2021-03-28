using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.Xids
{
	public partial class Xids
	{
		public void SaveAsJson(string destinationFile)
		{
			using (var sw = new StreamWriter(destinationFile))
			{
				SaveAsJson(sw);
			}
		}

		public void SaveAsJson(StreamWriter sw)
		{
			var serializer = new JsonSerializer
			{
				NullValueHandling = NullValueHandling.Ignore,
				TypeNameHandling = TypeNameHandling.Auto
			};
			serializer.Converters.Add(new StringEnumConverter());
#if DEBUG
			serializer.Formatting = Formatting.Indented;
#endif
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				serializer.Serialize(writer, this);
			}
		}

		private static JsonSerializer readSerializer()
		{
			var serializer = new JsonSerializer
			{
				NullValueHandling = NullValueHandling.Ignore,
				TypeNameHandling = TypeNameHandling.Auto,
			};
			serializer.Converters.Add(new StringEnumConverter());
			return serializer;
		}

		public static Xids LoadFromJson(string sourceFile)
		{
			using (StreamReader file = File.OpenText(sourceFile))
			{
				var serializer = readSerializer();
				Xids unpersisted = (Xids)serializer.Deserialize(file, typeof(Xids));
				return Finalize(unpersisted);
			}
		}

		private static Xids Finalize(Xids unpersisted)
		{
			if (unpersisted == null)
				return null;
			foreach (var req in unpersisted.AllSpecifications())
			{
				req.SetIds(unpersisted);
			}
			return unpersisted;
		}

		public static Xids LoadFromJson(Stream sourceStream)
		{
			var serializer = readSerializer();

			using (var sr = new StreamReader(sourceStream))
			using (var jsonTextReader = new JsonTextReader(sr))
			{
				Xids unpersisted  = (Xids)serializer.Deserialize(jsonTextReader, typeof(Xids));
				return Finalize(unpersisted);
			}
		}
	}
}
