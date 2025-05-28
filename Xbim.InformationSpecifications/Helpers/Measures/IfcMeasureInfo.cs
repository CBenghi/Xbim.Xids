using IdsLib.IfcSchema;
using System;
using Xbim.InformationSpecifications.Helpers.Measures;

namespace Xbim.InformationSpecifications.Helpers
{
	// todo: Extension methods for the conversion of the value given 
	// If 
	//    String -> No conversion
	//    number -> No conversion
	// Extension method To/From - via (double value, string unit) (m2/kg)

	/// <summary>
	/// Metadata about measure conversion behaviours.
	/// Use <see cref="Measures.MeasureUnit" /> for the quantitative conversion services.
	/// </summary>
	[Obsolete("Use relevant class in the idslib.")]
	public readonly struct IfcMeasureInfo : IValueProvider
	{
		/// <summary>
		/// basic constructor
		/// </summary>
		public IfcMeasureInfo(string measure, string description, string unit, string symbol, string exponents, string[] concrete, string unitTypeEnum)
		{
			Id = measure;
			IfcMeasure = measure;
			Description = description;
			Unit = unit;
			UnitSymbol = symbol;
			var tmpExponents = DimensionalExponents.FromString(exponents);
			Exponents = tmpExponents ?? new DimensionalExponents();
			ConcreteClasses = concrete;
			UnitTypeEnum = unitTypeEnum;
		}

		/// <summary>
		/// The string ID found in the XML persistence
		/// </summary>
		public string Id { get; }
		/// <summary>
		/// String of the Ifc type expected
		/// </summary>
		public string IfcMeasure { get; }

		/// <summary>
		/// A textual description, e.g. "Amount of substance"
		/// </summary>
		public string Description { get; }
		/// <summary>
		/// Full name of the unit.
		/// </summary>
		public string Unit { get; }
		/// <summary>
		/// Symbol used to present the unit.
		/// </summary>
		public string UnitSymbol { get; }
		/// <summary>
		/// Dimensional exponents useful for conversion to other units.
		/// </summary>
		public DimensionalExponents Exponents { get; }

		/// <summary>
		/// Concrete implementing classes with namespace
		/// </summary>
		public string[] ConcreteClasses { get; }

		/// <summary>
		/// The string value of the UnitType enum of a valid matching unit
		/// </summary>
		public string UnitTypeEnum { get; }

		/// <summary>
		/// Returns the SI preferred unit.
		/// </summary>
		/// <returns>empty string for measures that do not have expected measures</returns>
		public readonly string GetUnit()
		{
			if (!string.IsNullOrEmpty(Unit))
				return Unit;
			if (Exponents is not null)
				return Exponents.ToUnitSymbol();
			return "";
		}
	}
}
