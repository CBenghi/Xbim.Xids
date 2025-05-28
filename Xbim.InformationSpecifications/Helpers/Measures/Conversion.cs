using IdsLib.IfcSchema;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Xbim.InformationSpecifications.Helpers.Measures
{
	/// <summary>
	/// Support class for conversion
	/// </summary>
	public static partial class Conversion
	{
		private static Dictionary<string, IfcMeasureInformation>? _dicUnits;

		internal static bool TryGetUnit(string unit, [NotNullWhen(true)] out IfcMeasureInformation? found)
		{
			if (_dicUnits == null)
			{
				_dicUnits = new Dictionary<string, IfcMeasureInformation>();
				var measures = IdsLib.IfcSchema.SchemaInfo.AllMeasureInformation;
				foreach (var item in measures)
				{
					if (!string.IsNullOrWhiteSpace(item.UnitSymbol) && !_dicUnits.ContainsKey(item.UnitSymbol))
					{
						_dicUnits.Add(item.UnitSymbol, item);
					}
					if (!string.IsNullOrWhiteSpace(item.DefaultDisplay) && !_dicUnits.ContainsKey(item.DefaultDisplay))
					{
						_dicUnits.Add(item.DefaultDisplay, item);
					}
				}
			}
			if (_dicUnits.ContainsKey(unit))
			{
				found = _dicUnits[unit];
				return true;
			}
			found = null;
			return false;
		}

		private static Dictionary<string, IfcConversionUnitInformation>? _units = null;

		internal static bool TryGetConversion(string unit, [NotNullWhen(true)] out IfcConversionUnitInformation? Conversion)
		{
			if (_units is null)
			{
				_units = new Dictionary<string, IfcConversionUnitInformation>();
				foreach (var item in UnitConversions)
				{
					foreach (var alias in item.ConversionUnitNames)
					{
						_units.Add(alias, item);
					}
				}
				foreach (var pair in _conversionAliases)
				{
					_units.Add(pair.alias, _units[pair.existing]);
				}
			}
			var existingFound = _units.TryGetValue(unit, out Conversion);
			// if not found we can try to find the unit breaking it down via SI prefixes
			if (!existingFound && IfcMeasureInformation.TryGetSIUnitFromString(unit, out var exp, out var prefix, out var pow))
			{
				if (prefix == IfcConversionUnitInformation.SiPrefix.NONE)
					// no need to add conversion.
					return false;
				var ratio = IfcConversionUnitInformation.SiMultiplier(prefix);
				var preferred = GetPreferredMeasure(exp);
				if (preferred == null)
					return false;
				if (pow != 1)
				{
					// todo deal with exponent
					ratio *= Math.Pow(10, pow);
				}
				Conversion = new IfcConversionUnitInformation(unit, preferred.IfcMeasure, ratio, preferred.UnitSymbol);
				_units.Add(unit, Conversion);
				return true;
			}
			return existingFound;
		}

		private static IfcMeasureInformation? GetPreferredMeasure(DimensionalExponents exp)
		{
			var measures = IdsLib.IfcSchema.SchemaInfo.AllMeasureInformation;
			var match = measures.Where(x => x.Exponents.Equals(exp)).ToList();
			if (match.Count == 1)
				return match.First();
			match = match.Where(x => DimensionalExponents.UnitMeasures.Contains(x.IfcMeasure)).ToList();
			if (match.Count == 1)
				return match.First();
			return null;
		}

		private static List<(string alias, string existing)> _conversionAliases = [
			("lb", "pound"),
			("ft", "foot"),
			("feet", "foot"),
			("'", "foot"),
			("\"", "inch"),
			("inches", "inch"),
			("''", "inch"),
			("in", "inch"),
			("yd", "yard"),
			("mi", "mile"),
			("gal", "gallon US"),
			];
		private static IfcConversionUnitInformation[] _extraConversions = [
            // modified from the ids repository
            new IfcConversionUnitInformation("foot", "IFCLENGTHMEASURE", 304.8, "mm"),
			new IfcConversionUnitInformation("US survey foot", "IFCLENGTHMEASURE", 304.80060960122, "mm"),
			new IfcConversionUnitInformation("yard", "IFCLENGTHMEASURE", 914.4, "mm"),
			new IfcConversionUnitInformation("mile", "IFCLENGTHMEASURE", 1609.344, "m"),
			new IfcConversionUnitInformation("square inch", "IFCAREAMEASURE", 0.00064516, "m2"),
			new IfcConversionUnitInformation("square foot", "IFCAREAMEASURE", 0.09290304, "m2"),
			new IfcConversionUnitInformation("square yard", "IFCAREAMEASURE", 0.83612736, "m2"),
			new IfcConversionUnitInformation("acre", "IFCAREAMEASURE", 4046.8564224, "m2"),
			new IfcConversionUnitInformation("square mile", "IFCAREAMEASURE", 2589988.110336, "m2"),
			new IfcConversionUnitInformation("cubic inch", "IFCVOLUMEMEASURE", 0.000016387064, "m3"),
			new IfcConversionUnitInformation("cubic foot", "IFCVOLUMEMEASURE", 0.028316846592, "m3"),
			new IfcConversionUnitInformation("cubic yard", "IFCVOLUMEMEASURE", 0.764554857984, "m3"),
			new IfcConversionUnitInformation(["litre", "L"], "IFCVOLUMEMEASURE", 0.001, "m3"),
			new IfcConversionUnitInformation("fluid ounce UK", "IFCVOLUMEMEASURE", 0.0000284130625, "m3"),
			new IfcConversionUnitInformation("fluid ounce US", "IFCVOLUMEMEASURE", 0.0000295735295625, "m3"),
			new IfcConversionUnitInformation("pint UK", "IFCVOLUMEMEASURE", 0.00056826125, "m3"),
			new IfcConversionUnitInformation("pint US", "IFCVOLUMEMEASURE", 0.000473176473, "m3"),
			new IfcConversionUnitInformation("gallon UK", "IFCVOLUMEMEASURE", 0.00454609, "m3"),
			new IfcConversionUnitInformation("gallon US", "IFCVOLUMEMEASURE", 0.003785411784, "m3"),
			new IfcConversionUnitInformation("degree", "IFCPLANEANGLEMEASURE", 0.017453292519943295, "rad"),
			new IfcConversionUnitInformation("ounce", "IFCMASSMEASURE", 0.028349523125, "kg"),
			new IfcConversionUnitInformation("pound", "IFCMASSMEASURE", 0.45359237, "kg"),
			new IfcConversionUnitInformation(["ton UK", "long ton", "gross ton", "shipper's ton"], "IFCMASSMEASURE", 1016.0469088, "kg", null),
			new IfcConversionUnitInformation(["ton US", "short ton", "net ton", "ton"], "IFCMASSMEASURE", 907.18474, "kg", null),
			new IfcConversionUnitInformation(["lbf", "pound-force"], "IFCFORCEMEASURE", 4.4482216152605, "N", null),
			new IfcConversionUnitInformation(["kip", "kilopound-force"], "IFCFORCEMEASURE", 4448.2216152605, "N", null),
			new IfcConversionUnitInformation(["psi", "pound-force per square inch"], "IFCPRESSUREMEASURE", 6894.757293168361, "Pa", null),
			new IfcConversionUnitInformation(["ksi", "kilopound-force per square inch"], "IFCPRESSUREMEASURE", 6894757.293168361, "Pa", null),
			new IfcConversionUnitInformation(["minute", "min"], "IFCTIMEMEASURE", 60, "s"),
			new IfcConversionUnitInformation(["hour", "h"], "IFCTIMEMEASURE", 3600, "s"),
			new IfcConversionUnitInformation("day", "IFCTIMEMEASURE", 86400, "s"),
			new IfcConversionUnitInformation(["btu", "British Thermal Unit"], "IFCENERGYMEASURE", 1055.05585262, "J", null),
			new IfcConversionUnitInformation("mm", "IFCLENGTHMEASURE", 0.001, "m"),
			new IfcConversionUnitInformation("g", "IFCMASSMEASURE", 0.001, "kg"),
			new IfcConversionUnitInformation("Kg", "IFCMASSMEASURE", 1, "kg"),
			new IfcConversionUnitInformation(["°C", "Celsius"], "IFCTHERMODYNAMICTEMPERATUREMEASURE", 1, "°K", 273.15),
			new IfcConversionUnitInformation(["°F", "Fahrenheit"], "IFCTHERMODYNAMICTEMPERATUREMEASURE", 0.5555555555555556, "°K", 459.67),
			new IfcConversionUnitInformation(["°R", "Rankine"], "IFCTHERMODYNAMICTEMPERATUREMEASURE", 0.5555555555555556, "°K", 0.0),
			new IfcConversionUnitInformation(["K"], "IFCTHERMODYNAMICTEMPERATUREMEASURE", 1, "°K", 0.0),

            // extras
            new IfcConversionUnitInformation(["sec", "second"], "IFCTIMEMEASURE", 1, "s", 0),
			new IfcConversionUnitInformation(["ch", "chain"], "IFCLENGTHMEASURE", 20.116, "m", 0),
			new IfcConversionUnitInformation(["hectare", "ha"], "IFCAREAMEASURE", 10000, "m2", 0),
			new IfcConversionUnitInformation(["mole"], "IFCAMOUNTOFSUBSTANCEMEASURE", 1, "mol", 0),
			new IfcConversionUnitInformation(["kgf", "kg-force"], "IFCFORCEMEASURE", 9.80665, "N", 0),
			new IfcConversionUnitInformation(["dyn", "dyne"], "IFCFORCEMEASURE", 1e-5, "N", 0),
			new IfcConversionUnitInformation("inch", "IFCLENGTHMEASURE", 25.4, "mm"),
			new IfcConversionUnitInformation("bar", "IFCPRESSUREMEASURE", 100000, "Pa"),

			];

		/// <summary>
		/// triggers the reloads the conversion units defined in <see cref="UnitConversions"/> upon the next <see cref="TryGetConversion"/>.
		/// </summary>
		public static void ReloadConversion()
		{
			_units = null;
		}

		/// <summary>
		/// Allows the specification of conversion factors between unit symbols. 
		/// Any unit here must be added ahead of the first <see cref="TryGetConversion"/> attempt.
		/// Otherwise the ReloadConversion
		/// </summary>
		public static List<IfcConversionUnitInformation> UnitConversions { get; } = new
			(
				//IdsLib.IfcSchema.SchemaInfo.StandardConversionUnits
				//    // .Where(x=>!x.ConversionUnitNames.Any(x=>x.Contains("°")))
				//    .Concat(_extraConversions)
				_extraConversions
			);


	}
}
