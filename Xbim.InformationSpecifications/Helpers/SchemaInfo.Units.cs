using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xbim.InformationSpecifications.Helpers.Measures;

namespace Xbim.InformationSpecifications.Helpers
{
	// todo: move to one of the measures classes.

	public partial class SchemaInfo
	{
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
			new UnitConversion(1, "in", 0.0254, "m",
				"inches", "''", "\""),
			new UnitConversion(1, "ft", 0.3048, "m",
				"feet", "'"),
			new UnitConversion(1, "yd", 0.9144, "m",
				"yard"),
			new UnitConversion(1, "mi", 1609.344, "m",
				"mile"),
			new UnitConversion(1, "ch", 20.116, "m",
				"chain"),
			new UnitConversion(1, "acre", 4046.87261, "m2"),
			new UnitConversion(1, "hectare", 10000, "m2"),
			new UnitConversion(1, "lb", 0.4536, "Kg",
				"pound"),
			new UnitConversion(1, "lbf", 4.448222, "N",
				"pound-force"),
			new UnitConversion(1, "°C", 1, "K", 273.15),
			new UnitConversion(9, "°F", 5, "K", 459.67),
			new UnitConversion(1, "min", 60, "s"),
			new UnitConversion(1, "sec", 1, "s"),
			new UnitConversion(1, "gal", 0.00378542, "m3",
				"gallon"),
			new UnitConversion(1, "Fahrenheit", 1, "°F"),
			new UnitConversion(1, "Celsius", 1, "°C"),
			new UnitConversion(1, "day", 86400, "s"),
			new UnitConversion(1, "kip", 1000, "lbf"),
			new UnitConversion(1, "hour", 3600, "s"),
			new UnitConversion(1000, "mm", 1, "m"),
			new UnitConversion(100, "cm", 1, "m"),
			new UnitConversion(1, "daN", 10, "N"),
			//new UnitConversion(1, "mole", 10, "mol"),
			new UnitConversion(1, "kg", 1, "Kg"),
			new UnitConversion(1, "kgf", 9.80665, "N",
				"kg-force"),
		};
	}
}
