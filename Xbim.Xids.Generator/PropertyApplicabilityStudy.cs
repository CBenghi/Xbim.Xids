using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xbim.Common.Metadata;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Properties;

namespace Xbim.InformationSpecifications.Generator
{
	public class PropertyApplicabilityStudy
	{
		private static Dictionary<Properties.Version, List<string>> includeTypes;
		internal static Dictionary<Properties.Version, List<string>> IncludeTypes
		{
			get
			{
				if (includeTypes == null)
				{
					includeTypes = new Dictionary<Properties.Version, List<string>>();
					// IncludeTypes.Add(Properties.Version.IFC2x3, new List<string>() { "IfcRoot" });
					IncludeTypes.Add(Properties.Version.IFC2x3, new List<string>() { "IfcObject", "IfcTypeProduct" });
					IncludeTypes.Add(Properties.Version.IFC4, new List<string>() { "IfcObject" });
				}
				return includeTypes;
			}
		}

		public static string Execute()
		{
			var dist = new StringBuilder();
			var schemas = new[] { Xbim.Properties.Version.IFC2x3, Xbim.Properties.Version.IFC4 };
			foreach (var schema in schemas)
			{
				System.Reflection.Module module = null;
				if (schema == Properties.Version.IFC2x3)
					module = (typeof(Ifc2x3.Kernel.IfcProduct)).Module;
				else if (schema == Properties.Version.IFC4)
					module = (typeof(Ifc4.Kernel.IfcProduct)).Module;
				var metaD = ExpressMetaData.GetMetadata(module);

				dist.AppendLine($"===================================================================================== {schema}");
				dist.AppendLine($"= {schema}");
				dist.AppendLine($"===================================================================================== {schema}");
				var distinctClassesOfProperties = new List<string>();
				var propertyDefinitions = new Definitions<PropertySetDef>(schema);
				if (propertyDefinitions != null)
					propertyDefinitions.LoadAllDefault();
				foreach (var set in propertyDefinitions.DefinitionSets)
				{
					var classes = set.ApplicableClasses.Select(x => x.ClassName).ToArray();
					distinctClassesOfProperties = distinctClassesOfProperties.Concat(classes).ToList();
				}
				distinctClassesOfProperties = distinctClassesOfProperties.Distinct().ToList();

				// trying to find a set of classes that matches the property types
				List<string> HandledTypes = new List<string>();
				// HandledTypes.AddRange(TreeOf(metaD.ExpressType("IFCROOT")));
				foreach (var item in IncludeTypes[schema])
				{
					HandledTypes.AddRange(TreeOf(metaD.ExpressType(item.ToUpperInvariant())));
				}
				
				dist.AppendLine($"HandledTypes.Count: {HandledTypes.Count}");
				dist.AppendLine($"distinctClassesOfProperties.Count: {distinctClassesOfProperties.Count}");
				foreach (var className in distinctClassesOfProperties)
				{
					var daType = metaD.ExpressType(className.ToUpperInvariant());
					if (HandledTypes.Contains(className))
						continue;
					var t = daType.Type;
					// t.IsAbstract
					var ft = FullH(daType);					
					dist.AppendLine($"Missing: {className}\t{ft}");
				}

				foreach (var className in HandledTypes)
				{
					var daType = metaD.ExpressType(className.ToUpperInvariant());
					if (distinctClassesOfProperties.Contains(className))
						continue;
					var t = daType.Type;
					// t.IsAbstract
					var ft = FullH(daType);
					dist.AppendLine($"Extra: {className}\t{ft}");
				}
			}
			return dist.ToString();
		}

		internal static IEnumerable<string> TreeOf(ExpressType expressType)
		{
			yield return expressType.Name;
			foreach (var sub in expressType.SubTypes)
			{
				foreach (var item in TreeOf(sub))
				{
					yield return item;
				}
			}
		}

		private static string FullH(ExpressType daType)
		{
			if (daType.SuperType != null)
			{
				return FullH(daType.SuperType) + " " + $"{daType.Name} ({daType.AllSubTypes.Count()})";
			}
			return $"{daType.Name} ({daType.AllSubTypes.Count()})";
		}

		private static string newStringArray(string[] classes)
		{
			return @$"new [] {{""{string.Join("\",\"", classes)}""}}";
		}

	}
}