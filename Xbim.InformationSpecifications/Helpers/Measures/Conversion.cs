using IdsLib.IfcSchema;
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

        private static Dictionary<string, UnitConversion>? _units = null;

        internal static bool TryGetConversion(string unit, [NotNullWhen(true)] out UnitConversion? Conversion)
        {
            if (_units is null)
            {
                _units = new Dictionary<string, UnitConversion>();
                foreach (var item in UnitConversions)
                {
                    _units.Add(item.Name, item);
                    foreach (var alias in item.Aliases)
                    {
                        _units.Add(alias, item);
                    }
                }
            }
            return _units.TryGetValue(unit, out Conversion);
        }

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
        public static List<UnitConversion> UnitConversions { get; } = new List<UnitConversion>()
        {
            new(1, "in", 0.0254, "m",
                "inches", "''", "\""),
            new(1, "ft", 0.3048, "m",
                "feet", "'"),
            new(1, "yd", 0.9144, "m",
                "yard"),
            new(1, "mi", 1609.344, "m",
                "mile"),
            new(1, "ch", 20.116, "m",
                "chain"),
            new(1, "acre", 4046.87261, "m2"),
            new(1, "hectare", 10000, "m2"),
            new(1, "lb", 0.4536, "Kg",
                "pound"),
            new(1, "lbf", 4.448222, "N",
                "pound-force"),
            new(1, "°C", 1, "°K", 273.15),
            new(9, "°F", 5, "°K", 459.67),
            new(1, "min", 60, "s"),
            new(1, "sec", 1, "s"),
            new(1, "gal", 0.00378542, "m3",
                "gallon"),
            new(1, "Fahrenheit", 1, "°F"),
            new(1, "Celsius", 1, "°C"),
            new(1, "day", 86400, "s"),
            new(1, "kip", 1000, "lbf"),
            new(1, "hour", 3600, "s"),
            new(1000, "mm", 1, "m"),
            new(100, "cm", 1, "m"),
            new(1, "daN", 10, "N"),
			//new UnitConversion(1, "mole", 10, "mol"),
			new(1, "kg", 1, "Kg"),
            new(1, "kgf", 9.80665, "N",
                "kg-force"),
        };
    }
}
