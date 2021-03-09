using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.IDS
{
	public partial class IDS
	{
		public void SaveAsJson(string destinationFile)
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
			// serializer.Converters.Add(new JavaScriptDateTimeConverter());

			using (StreamWriter sw = new StreamWriter(destinationFile))
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				serializer.Serialize(writer, this);
			}
		}

		public static IDS LoadFromJson(string sourceFile)
		{
			using (StreamReader file = File.OpenText(sourceFile))
			{
				var serializer = new JsonSerializer
				{
					NullValueHandling = NullValueHandling.Ignore,
					TypeNameHandling = TypeNameHandling.Auto,			
				};
				serializer.Converters.Add(new StringEnumConverter());
				IDS unpersisted = (IDS)serializer.Deserialize(file, typeof(IDS));
				foreach (var req in unpersisted.AllRequirements())
				{
					req.SetIds(unpersisted);
				}

				return unpersisted;
			}
		}
	}
}
