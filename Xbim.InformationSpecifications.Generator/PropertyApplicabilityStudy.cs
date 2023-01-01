using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Common.Metadata;
using Xbim.Properties;

namespace Xbim.InformationSpecifications.Generator
{
    /// <summary>
    /// Provides helpers to define what classes should be included in the treatment of the various schemas
    /// </summary>
    public class IfcClassStudy
    {
        private static Dictionary<Properties.Version, List<string>> includeTypes;

        /// <summary>
        /// This determines the classes for each schema, methods below help visualise them.
        /// </summary>
        internal static Dictionary<Properties.Version, List<string>> IncludeTypes
        {
            get
            {
                if (includeTypes == null)
                {
                    includeTypes = new Dictionary<Properties.Version, List<string>>();
                    IncludeTypes.Add(Properties.Version.IFC2x3, new List<string>() { "IfcObject", "IfcTypeObject" });
                    IncludeTypes.Add(Properties.Version.IFC4, new List<string>() { "IfcObject", "IfcTypeObject" });
                    IncludeTypes.Add(Properties.Version.IFC4x3, new List<string>() { "IfcObject", "IfcTypeObject" });
                }
                return includeTypes;
            }
        }

        /// <summary>
        /// Compares the classes included in the export with the ones taken from the defined property sets.
        /// </summary>
        public static string ReportMatchesToProperties()
        {
            var report = new StringBuilder();
            var schemas = new[] { Xbim.Properties.Version.IFC2x3, Xbim.Properties.Version.IFC4, Version.IFC4x3 };
            foreach (var schema in schemas)
            {
                System.Reflection.Module module = SchemaHelper.GetModule(schema);
                var metaD = ExpressMetaData.GetMetadata(module);

                report.AppendLine($"===================================================================================== {schema}");
                report.AppendLine($"= {schema}");
                report.AppendLine($"===================================================================================== {schema}");

                // start from the available properties, and get the classes that they apply to
                var distinctClassesFromPropertySets = new List<string>();
                var propertyDefinitions = new Definitions<PropertySetDef>(schema);
                if (propertyDefinitions != null)
                    propertyDefinitions.LoadAllDefault();
                foreach (var set in propertyDefinitions.DefinitionSets)
                {
                    var classes = set.ApplicableClasses.Select(x => x.ClassName).ToArray();
                    distinctClassesFromPropertySets = distinctClassesFromPropertySets.Concat(classes).ToList();
                }
                distinctClassesFromPropertySets = distinctClassesFromPropertySets.Distinct().OrderBy(k => k).ToList();

                // trying to find a set of classes that matches the property types
                List<string> handledTypes = new();
                if (!IncludeTypes.ContainsKey(schema))
                {
                    report.AppendLine($"No included types for {schema}.");
                    continue;
                }
                foreach (var item in IncludeTypes[schema])
                {
                    var t = metaD.ExpressType(item.ToUpperInvariant());
                    if (t != null)
                        handledTypes.AddRange(TreeOf(t));
                    else
                        report.AppendLine($"{item} not found");
                }

                report.AppendLine($"HandledTypes.Count: {handledTypes.Count}");
                report.AppendLine($"distinctClassesFromPropertySets.Count: {distinctClassesFromPropertySets.Count}");
                foreach (var className in distinctClassesFromPropertySets)
                {
                    var daType = metaD.ExpressType(className.ToUpperInvariant());
                    if (handledTypes.Contains(className))
                        continue;
                    var t = daType.Type;
                    var ft = FullH(daType);
                    report.AppendLine($"Has property but no class: {className}\t{ft}");
                }

                foreach (var className in handledTypes.OrderBy(k => k))
                {
                    var daType = metaD.ExpressType(className.ToUpperInvariant());
                    if (distinctClassesFromPropertySets.Contains(className))
                        continue;
                    var t = daType.Type;
                    var ft = FullH(daType);
                    report.AppendLine($"Has class but no property: {className}\t{ft}");
                }
            }
            return report.ToString();
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
    }
}