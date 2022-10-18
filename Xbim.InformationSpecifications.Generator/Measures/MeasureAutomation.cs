using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Xbim.Common.Metadata;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications.Generator.Measures
{
    internal class MeasureAutomation
    {
        static public IEnumerable<Measure> GetFromDocumentation()
        {
            var splitter = new string[] { "|" };
            FileInfo f = new("Files/Units.md");
            var allDocumentation = File.ReadAllLines(f.FullName);
            var isParsing = false;
            var tally = 0;
            foreach (var oneLine in allDocumentation)
            {
                if (isParsing)
                {
                    var parts = oneLine.Split(splitter, StringSplitOptions.TrimEntries);
                    if (parts.Length != 7) 
                    {
                        // we are leaving the loop, check the expected tally
                        if (tally != 50) // need to review the info from the documentation
                            throw new Exception("Unexpected number of measures.");
                        yield break; // no more measures to find.
                    }
                    var retMeasurement = new Measure()
                    {
                        IfcMeasure = parts[1],
                        Description = parts[2],
                        Unit = parts[3],
                        UnitSymbol = parts[4],
                        DimensionalExponents = parts[5],
                    };
                    tally++;
                    yield return retMeasurement;
                }
                else
                {
                    if (oneLine.Contains("-----"))
                    {
                        isParsing = true;
                        continue; // start parsing
                    }
                }
            }
        }

        /// <summary>
        /// Gets measures from the documentation and tries to fill in missing dimensional exponents
        /// </summary>
        public static string Execute_ImproveDocumentation()
        {
            MeasureCollection m = new(GetFromDocumentation());

            bool tryImprove = true;
            while (tryImprove)
            {
                tryImprove = false;
                foreach (var missingExp in m.MeasureList.Where(x => x.DimensionalExponents == ""))
                {
                    var neededSymbols = UnitFactor.SymbolBreakDown(missingExp.UnitSymbol);
                    var allSym = true;
                    foreach (var sym in neededSymbols)
                    {
                        var found = m.GetByUnit(sym.UnitSymbol);
                        if (found != null)
                        {
                            if (found.DimensionalExponents != "")
                            {
                                _ = sym.TryGetDimensionalExponents(out var tde, out _, out _);
                                Debug.WriteLine($"Found '{found.UnitSymbol}' - {found.DimensionalExponents} - {tde.ToUnitSymbol()}");
                            }
                            else
                            {
                                Debug.WriteLine($"Missing dimensional exponents on '{found.UnitSymbol}'");
                                allSym = false;
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"Missing '{sym.UnitSymbol}' - {missingExp.UnitSymbol}");
                            allSym = false;
                        }
                    }
                    if (allSym)
                    {
                        DimensionalExponents d = null;
                        Debug.WriteLine($"Can do {missingExp.Description} - {missingExp.UnitSymbol}");
                        foreach (var sym in neededSymbols)
                        {
                            var found = m.GetByUnit(sym.UnitSymbol);
                            if (d == null)
                                sym.TryGetDimensionalExponents(out d, out _, out _);
                            else
                            {
                                if (sym.TryGetDimensionalExponents(out var t, out _, out _))
                                    d = d.Multiply(t);
                            }
                        }
                        if (d != null)
                        {
                            Debug.WriteLine($"Computed: {d} - {d.ToUnitSymbol()}");
                            missingExp.DimensionalExponents = d.ToString();
                            tryImprove = true;
                        }
                        Debug.WriteLine("");

                    }
                    else
                    {
                        Debug.WriteLine($"Cannot do {missingExp.Description} - {missingExp.UnitSymbol}\r\n");
                    }
                }
            }
            var sb = new StringBuilder();
            foreach (var item in m.MeasureList)
            {
                sb.AppendLine($"{item.Description}\t{item.DimensionalExponents}");
            }
            Debug.WriteLine(sb.ToString());
            return sb.ToString();
        }

        private static string NewStringArray(string[] classes)
        {
            return @$"new[] {{ ""{string.Join("\", \"", classes)}"" }}";
        }

        public static string Execute_GenerateIfcMeasureDictionary()
        {
            var source = stub;
            var sb = new StringBuilder();

            var schemas = new[] { Xbim.Properties.Version.IFC2x3, Xbim.Properties.Version.IFC4 };
            var meta = new List<ExpressMetaData>();

            foreach (var schema in schemas)
            {
                System.Reflection.Module module = null;
                if (schema == Properties.Version.IFC2x3)
                    module = (typeof(Ifc2x3.Kernel.IfcProduct)).Module;
                else if (schema == Properties.Version.IFC4)
                    module = (typeof(Ifc4.Kernel.IfcProduct)).Module;
                var metaD = ExpressMetaData.GetMetadata(module);
                meta.Add(metaD);
            }

            var allUnits = GetSchemaUnits();

            MeasureCollection mCollection = new(GetFromDocumentation().Concat(ExtraMeaures()));
            foreach (var measure in mCollection.MeasureList)
            {
                var concreteClasses = new List<string>();
                
                var expectedUnitsTypes = new[] {
                    measure.Description.ToUpperInvariant().Replace(" ", "") + "UNIT",
                    measure.IfcMeasure[3..^7].ToUpperInvariant() + "UNIT",
                    //measure.Key.ToUpperInvariant() + "VALUEUNIT",
                    //"THERMODYNAMIC" + measure.Key.ToUpperInvariant() + "UNIT",
                    };
                expectedUnitsTypes = expectedUnitsTypes.Distinct().ToArray();
                // special cases
                //
                //if (measure.Key == "Speed")
                //    expectedUnitsTypes = new[] { "LINEARVELOCITYUNIT" }; 
                if (measure.IfcMeasure == "IfcThermalConductivityMeasure")
                    expectedUnitsTypes = new[] { "THERMALCONDUCTANCEUNIT" };
                //else if (measure.Key == "Heating")
                //    expectedUnitsTypes = new[] { "HEATINGVALUEUNIT" };
                //else if (measure.Key == "Temperature")
                //    expectedUnitsTypes = new[] { "THERMODYNAMICTEMPERATUREUNIT"};
                //else if (measure.Key == "Angle")
                //    expectedUnitsTypes = new[] { "PLANEANGLEUNIT" };

                var expectedUnitType = "";
                foreach (var item in expectedUnitsTypes)
                {
                    var t = allUnits.FirstOrDefault(x => x.EndsWith($".{item}"));
                    if (t is not null)
                    {
                        expectedUnitType = t;
                        break;
                    }
                }
                
                var ifcType = measure.IfcMeasure;
                if (ifcType != null)
                {
                    for (int i = 0; i < schemas.Length; i++)
                    {
                        _ = schemas[i];
                        var metaD = meta[i];
                        var tp = metaD.ExpressType(ifcType.ToUpperInvariant());
                        if (tp != null)
                        {
                            var cClass = tp.Type.FullName.Replace("Xbim.", "");
                            concreteClasses.Add(cClass);
                        }
                    }
                }
                sb.AppendLine($"\t\t\t{{ \"{measure.IfcMeasure}\", new IfcMeasureInfo(\"{measure.IfcMeasure}\", \"{measure.Description}\", \"{measure.Unit}\", \"{measure.UnitSymbol}\", \"{measure.DimensionalExponents}\", {NewStringArray(concreteClasses.ToArray())}, \"{expectedUnitType}\") }},");
            }

            source = source.Replace($"\t\t\t<PlaceHolder>\r\n", sb.ToString());
            return source;
        }

        private static IList<string> GetSchemaUnits()
        {
            var all = Enum.GetValues<Ifc2x3.MeasureResource.IfcDerivedUnitEnum>().Select(x => $"IfcDerivedUnitEnum.{x}");
            all = all.Union(Enum.GetValues<Ifc4.Interfaces.IfcDerivedUnitEnum>().Select(x => $"IfcDerivedUnitEnum.{x}"));
            all = all.Union(Enum.GetValues<Ifc2x3.MeasureResource.IfcUnitEnum>().Select(x => $"IfcUnitEnum.{x}"));
            all = all.Union(Enum.GetValues<Ifc4.Interfaces.IfcUnitEnum>().Select(x => $"IfcUnitEnum.{x}"));
            return all.Distinct().OrderBy(x=>x).ToList();
        }

        public static string Execute_GenerateIfcMeasureEnum()
        {
            var source = stubEnums;
            var sb = new StringBuilder();

            var doc = Program.GetBuildingSmartSchemaXML();
            var measureEnum = GetMeasureRestrictionsFromSchema(doc).ToList();
            foreach (var measure in measureEnum)
            {
                if (SchemaInfo.IfcMeasures.TryGetValue(measure, out var found))
                {
                    if (found.Exponents != null)
                    {
                        sb.AppendLine($"\t\t/// {found.Description}, expressed in {found.GetUnit()}");
                    }
                    else
                    {
                        sb.AppendLine($"\t\t/// {measure}, no unit conversion");
                    }
                }
                sb.AppendLine($"\t\t{measure},");
            }

            source = Regex.Replace(source, $"[\t ]*<PlaceHolder>", sb.ToString());
            return source;
        }

        private static IEnumerable<Measure> ExtraMeaures()
        {
            yield break;
            //yield return new Measure() { IfcMeasure = "String" };
            //yield return new Measure() { Key = "Number" };
        }


        /// <summary>
        /// ensures that the measure helpers are fully populated
        /// </summary>
        /// <returns>False if no warnings</returns>
        public static bool Execute_CheckMeasureMetadata()
        {
            foreach (var measVal in SchemaInfo.IfcMeasures.Values)
            {
                if (measVal.UnitTypeEnum == "")
                    Program.Message(ConsoleColor.DarkYellow, $"Warning: Measure '{measVal.ID}' lacks UnitType.");
                // Debug.WriteLine($"{measVal.UnitTypeEnum}");
            }
            return false;
        }

        /// <summary>
        /// ensures that the schema and the helpers are compatible.
        /// </summary>
        /// <returns>False if not compatible</returns>
        public static bool Execute_CheckMeasureEnumeration()
        {
            var doc = Program.GetBuildingSmartSchemaXML();
            var measureEnum = GetMeasureRestrictionsFromSchema(doc).ToList();
            var errors = false;
            foreach (var item in measureEnum)
            {
                if (!SchemaInfo.IfcMeasures.TryGetValue(item, out _))
                {
                    Program.Message(ConsoleColor.Red, $"Value not found in helpers for measure '{item}'");
                    errors = true;
                }
            }
            return errors;
        }

        /// <summary>
        /// Taken from the schema, not the documentation
        /// </summary>
        private static IEnumerable<string> GetMeasureRestrictionsFromSchema(XmlDocument doc)
        {
            // finds the node via xml, then returns the enum
            XmlNode root = doc.DocumentElement;
            var prop = root.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Attributes[0].Value == "propertyType");
            var measure = prop.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Attributes.Count > 0 && n.Attributes[0].Value == "measure");
            var tp = measure.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name == "xs:simpleType");
            var rest = tp.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name == "xs:restriction");
            foreach (var item in rest.ChildNodes.Cast<XmlNode>())
            {
                yield return item.Attributes[0].Value;
            }
        }

        private const string stub = @"// generated running xbim.xids.generator
using System.Collections.Generic;

namespace Xbim.InformationSpecifications.Helpers
{
	public partial class SchemaInfo
	{
		/// <summary>
		/// Repository of valid <see cref=""IfcMeasureInfo""/> metadata given the persistence string defined in bS IDS
		/// </summary>
		public static Dictionary<string, IfcMeasureInfo> IfcMeasures { get; } = new()
		{
			<PlaceHolder>
		};
	}
}
";

        private const string stubEnums = @"// generated running xbim.xids.generator
using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.InformationSpecifications.Helpers
{
    /// <summary>
    /// Determins data type constraints and conversion for measures.
    /// </summary>
    public enum IfcMeasures
    {
        <PlaceHolder>
    }
}
";
    }
}