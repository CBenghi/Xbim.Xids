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
		/// <summary>
		/// Computes the GetPropertiesIFC2x3 and GetPropertiesIFC4 of the PropertySetInfo.Generated.cs file 
		/// Depends on the Xbim.Properties assembly.
		/// </summary>
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
					var classes = set.ApplicableClasses.Select(x => x.ClassName).ToArray();
					var properties = set.PropertyDefinitions.Select(x => x.Name).ToArray();
					var cArr = newStringArray(classes);
					var pArr = newStringArray(properties);
					// sb.AppendLine($@"			// {string.Join(",", classes)}");
					sb.AppendLine($@"			ret.Add(new PropertySetInfo(""{set.Name}"", {pArr}, {cArr} ));");
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
			var ret = new List<PropertySetInfo>();
<PlaceHolderIFC2x3>
			return ret;
		}

		static partial void GetPropertiesIFC4()
		{
			var ret = new List<PropertySetInfo>();
<PlaceHolderIFC4>
			return ret;
		}
	}
}
";
	}
}