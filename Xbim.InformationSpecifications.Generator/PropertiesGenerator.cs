using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xbim.Properties;

namespace Xbim.InformationSpecifications.Generator
{
    public class PropertiesGenerator 
    {
        public static string Execute()
        {
			var source = stub;
			var schemas = new[] { Xbim.Properties.Version.IFC2x3, Xbim.Properties.Version.IFC4 };		

			foreach (var schema in schemas)
			{
				var sb = new StringBuilder();
				var propertyDefinitions = new Definitions<PropertySetDef>(schema);
				if (propertyDefinitions != null)
					propertyDefinitions.LoadAllDefault();
				foreach (var set in propertyDefinitions.DefinitionSets)
				{
					/*
					 * ApplicableTypeValue is a mess... suspended.
					 * 
					var appType = set.ApplicableTypeValue;

					var re = new Regex(@"(?<type>.+)\.PredefinedType=""(?<ptype>.+)""");
					var re2 = new Regex(@"PSET\.(?<pset>.+)\.(?<prop>.+)=""(?<val>.+)""");
					var m = re.Match(appType);
					var m2 = re2.Match(appType);
					if (string.IsNullOrWhiteSpace(appType))
					{

					}
					else if (m.Success)
					{
						sb.AppendLine($@"			// Type: {m.Groups["type"].Value} -> {m.Groups["ptype"].Value}");
					}
					else if (m2.Success)
					{
						sb.AppendLine($@"			// pset {m2.Groups["pset"].Value} - {m2.Groups["prop"].Value} -> {m2.Groups["val"].Value}");
					}
					else
					{

					}
					*/
					var classes = set.ApplicableClasses.Select(x => x.ClassName).ToArray();
					var properties = set.PropertyDefinitions.Select(x => x.Name).ToArray();
					var cArr = newStringArray(classes);
					var pArr = newStringArray(properties);
					// sb.AppendLine($@"			// {string.Join(",", classes)}");
					sb.AppendLine($@"			schema{schema}.Add(new PropertySetInfo(""{set.Name}"", {pArr}, {cArr} ));");
				}
				source = source.Replace($"<PlaceHolder{schema}>", sb.ToString());
			}
			// context.AddSource("generated2.cs", source);
			return source;
		}

		private static string newStringArray(string[] classes)
		{
			return @$"new [] {{""{string.Join("\",\"", classes)}""}}";
		}

		private const string stub = @"
// generated via source generation from xbim.xids.generator
using System.Collections.Generic;

namespace Xbim.InformationSpecifications.Helpers
{
	public partial class PropertySetInfo
	{
		static partial void GetPropertiesIFC2x3()
		{
			schemaIFC2x3 = new List<PropertySetInfo>();
<PlaceHolderIFC2x3>
		}

		static partial void GetPropertiesIFC4()
		{
			schemaIFC4 = new List<PropertySetInfo>();
<PlaceHolderIFC4>
		}
	}
}
";
	}
}