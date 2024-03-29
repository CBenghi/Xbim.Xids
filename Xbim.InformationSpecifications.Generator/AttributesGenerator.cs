﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xbim.Common.Metadata;

namespace Xbim.InformationSpecifications.Generator
{
    class AttributesGenerator
    {
        /// <summary>
        /// SchemaInfo.GeneratedAttributes.cs
        /// </summary>
        static public string Execute()
        {
            var source = stub;
            var schemas = new[] { Xbim.Properties.Version.IFC2x3, Xbim.Properties.Version.IFC4, Properties.Version.IFC4x3 };

            foreach (var schema in schemas)
            {
                System.Reflection.Module module = SchemaHelper.GetModule(schema);
                var metaD = ExpressMetaData.GetMetadata(module);

                var sb = new StringBuilder();

                // trying to find a set of classes that matches the property types
                List<string> HandledTypes = new();
                foreach (var item in IfcClassStudy.IncludeTypes[schema]) // this determines the included types by schema
                {
                    HandledTypes.AddRange(IfcClassStudy.TreeOf(metaD.ExpressType(item.ToUpperInvariant())));
                }
                Dictionary<string, List<string>> typesByAttribute = new();
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

                    var classesInQuotes = pair.Value.Select(x => $"\"{x}\"").ToArray();
                    var topClassesInQuotes = OnlyTopClasses.Select(x => $"\"{x}\"").ToArray();
                    var line = $"\t\t\tdestinationSchema.AddAttribute({attribute}, new[] {{ {string.Join(", ", topClassesInQuotes)} }}, new[] {{ {string.Join(", ", classesInQuotes)} }});";

                    sb.AppendLine(line);
                }
                source = source.Replace($"<PlaceHolder{schema}>\r\n", sb.ToString());
            }
            source = source.Replace($"<PlaceHolderVersion>", VersionHelper.GetFileVersion(typeof(ExpressMetaData)));
            return source;
        }
        private const string stub = @"// generated code via xbim.xids.generator using Xbim.Essentials <PlaceHolderVersion> - any changes made directly here will be lost

using System;

namespace Xbim.InformationSpecifications.Helpers
{
	public partial class SchemaInfo
	{
		static partial void GetAttributesIFC2x3(SchemaInfo destinationSchema)
		{
<PlaceHolderIFC2x3>
		}

		static partial void GetAttributesIFC4(SchemaInfo destinationSchema)
		{
<PlaceHolderIFC4>
		}

        static partial void GetAttributesIFC4x3(SchemaInfo destinationSchema)
		{
<PlaceHolderIFC4x3>
		}
	}
}
";
    }
}