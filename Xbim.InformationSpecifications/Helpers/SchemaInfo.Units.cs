using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xbim.InformationSpecifications.Helpers.Measures;

namespace Xbim.InformationSpecifications.Helpers
{
	public partial class SchemaInfo
	{
		private static Dictionary<string, UnitConversion>? _units = null;

		public static bool TryGetConversion(string unit, [NotNullWhen(true)] out UnitConversion? Conversion)
		{
			if (_units is null)
			{
				_units = new Dictionary<string, UnitConversion>();
				foreach (var item in units())
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

		private static IEnumerable<UnitConversion> units()
		{
			yield return new UnitConversion(1, "in", 0.0254, "m",
				"inches", "''", "\"");
			yield return new UnitConversion(1, "ft", 0.3048, "m",
				"feet", "'");
			yield return new UnitConversion(1, "yd", 0.9144, "m",
				"yard");
			yield return new UnitConversion(1, "mi", 1609.344, "m",
				"mile");
			yield return new UnitConversion(1, "ch", 20.116, "m",
				"chain");
			yield return new UnitConversion(1, "acre", 4046.87261, "m2");
			yield return new UnitConversion(1, "hectare", 10000, "m2");
			yield return new UnitConversion(1, "lb", 0.4536, "Kg", 
				"pound");
			yield return new UnitConversion(1, "lbf", 4.448222, "N",
				"pound-force");
			yield return new UnitConversion(1, "°C", 1, "K", 273.15);
			yield return new UnitConversion(9, "°F", 5, "K", 459.67);
			yield return new UnitConversion(1, "min", 60, "s");
			yield return new UnitConversion(1, "sec", 1, "s");
			yield return new UnitConversion(1, "gal", 0.00378542, "m3",
				"gallon");
			yield return new UnitConversion(1, "Fahrenheit", 1, "°F");
			yield return new UnitConversion(1, "Celsius", 1, "°C");
			yield return new UnitConversion(1, "day", 86400, "s");
			yield return new UnitConversion(1, "kip", 1000, "lbf");
			yield return new UnitConversion(1, "hour", 3600, "s");
			yield return new UnitConversion(1000, "mm", 1, "m");
			yield return new UnitConversion(100, "cm", 1, "m");
			yield return new UnitConversion(1, "daN", 10, "N");
			//yield return new UnitConversion(1, "mole", 10, "mol");
			yield return new UnitConversion(1, "kg", 1, "Kg");
			yield return new UnitConversion(1, "kgf", 9.80665, "N",
				"kg-force");
		}
	}
}
