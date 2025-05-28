using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Xbim.InformationSpecifications.IO
{
	internal class JsonParseHelp
	{
		public static string QuickFindValue(Stream stream, string match)
		{
			var buffer = new byte[4096];
			_ = stream.Read(buffer, 0, buffer.Length);

			// We set isFinalBlock to false since we expect more data in a subsequent read from the stream.
			var reader = new Utf8JsonReader(buffer, isFinalBlock: false, state: default);
			// Debug.WriteLine($"String in buffer is: {Encoding.UTF8.GetString(buffer)}");

			// Search for "match" property name
			while (reader.TokenType != JsonTokenType.PropertyName || !reader.ValueTextEquals(match))
			{
				if (!reader.Read())
				{
					return string.Empty;
				}
			}

			// Found the "match" property name.
			while (!reader.Read())
			{
				return string.Empty;
			}
			// value of Summary property
			var vrs = reader.GetString();
			if (vrs == null)
				return string.Empty;
			return vrs;

		}
	}
}
