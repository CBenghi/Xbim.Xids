using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xbim.Common.Metadata;
using Xbim.Properties;

namespace Xbim.InformationSpecifications.Generator
{
	class AttributesGenerator
	{
		static public string Execute()
		{
			var source = "";
			var schemas = new[] { Xbim.Properties.Version.IFC2x3, Xbim.Properties.Version.IFC4 };

			foreach (var schema in schemas)
			{
				System.Reflection.Module module = null;
				if (schema == Properties.Version.IFC2x3)
					module = (typeof(Ifc2x3.Kernel.IfcProduct)).Module;
				else if (schema == Properties.Version.IFC4)
					module = (typeof(Ifc4.Kernel.IfcProduct)).Module;
				var metaD = ExpressMetaData.GetMetadata(module);

				var sb = new StringBuilder();

				// trying to find a set of classes that matches the property types
				List<string> HandledTypes = new List<string>();
				foreach (var item in PropertyApplicabilityStudy.IncludeTypes[schema]) // this determines the included types by schema
				{
					HandledTypes.AddRange(PropertyApplicabilityStudy.TreeOf(metaD.ExpressType(item.ToUpperInvariant())));
				}
				Dictionary<string, List<string>> typesByAttribute = new Dictionary<string, List<string>>();
				foreach (var className in HandledTypes)
				{
					var daType = metaD.ExpressType(className.ToUpperInvariant());
					foreach (var prop in daType.Properties.Values)
					{
						if (typesByAttribute.TryGetValue(prop.Name, out var lst))
							lst.Add(className);
						else
						{
							typesByAttribute.Add(prop.Name, new List<string>() { className });
						}
					}
				}
				Debug.WriteLine($"{schema}");
				foreach (var pair in typesByAttribute)
				{
					var attribute = $"\"{pair.Key}\"";
					// trying to remove all subclasses
					var OnlyTopClasses = pair.Value.ToList();
					for (int i = 0; i < OnlyTopClasses.Count; i++)
					{
						var thisClassName = OnlyTopClasses[i];
						var thisClass = metaD.ExpressType(thisClassName.ToUpperInvariant());

						foreach (var sub in thisClass.AllSubTypes)
						{
							OnlyTopClasses.Remove(sub.Name);
						}
					}

					var classesInQuotes = pair.Value.Select(x=>$"\"{x}\"").ToArray();
					var topClassesInQuotes = OnlyTopClasses.Select(x=>$"\"{x}\"").ToArray();
					var line = $"schema{schema}.AddAttribute({attribute}, new[] {{{string.Join(",", topClassesInQuotes)}}}, new[] {{{string.Join(",", classesInQuotes)}}});";
					Debug.WriteLine(line);
				}
			}
			
			
			Debug.WriteLine("The content of output populates the file SchemaInfo.GeneratedAttributes.cs");
			Debugger.Break();
			return source;
		}
	}
}
