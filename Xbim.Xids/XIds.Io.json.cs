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

		public static Xids LoadFromJson(string sourceFile)
		{
			using (StreamReader file = File.OpenText(sourceFile))
			{
				var serializer = new JsonSerializer
				{
					NullValueHandling = NullValueHandling.Ignore,
					TypeNameHandling = TypeNameHandling.Auto,			
				};
				serializer.Converters.Add(new StringEnumConverter());
				Xids unpersisted = (Xids)serializer.Deserialize(file, typeof(Xids));
				foreach (var req in unpersisted.AllRequirements())
				{
					req.SetIds(unpersisted);
				}

				return unpersisted;
			}
		}
	}
}
