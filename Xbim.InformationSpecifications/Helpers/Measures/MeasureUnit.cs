using IdsLib.IfcSchema;
using Xbim.InformationSpecifications.Generator.Measures;

namespace Xbim.InformationSpecifications.Helpers.Measures
{
    /// <summary>
    /// Main class to deal with unit conversion to and from constraint values.
    /// 
    /// Measurable units are stored in standard SI units in the schema, this class
    /// helps providing conversion methods against a variety of compatible units.
    /// </summary>
    public class MeasureUnit
    {
        /// <summary>
        /// Composition of the unit in the 7 fundamental units.
        /// </summary>
        public DimensionalExponents Exponent { get; }
        /// <summary>
        /// Conversion ratio between units
        /// </summary>
        public double Ratio { get; } = 1;
        /// <summary>
        /// Any scale offset (used for temperature conversions)
        /// </summary>
        public double Offset { get; } = 0;

        /// <summary>
        /// Evaluates if the Unit was correctly parsed during construction.
        /// </summary>
        public bool IsValid { get; private set; } = true;

        /// <summary>
        /// Constructor requiring a valid unit string.
        /// </summary>
        /// <param name="unitString">the unit to evaluate conversion for, e.g. "lb/m2"</param>
        public MeasureUnit(string unitString)
        {
            Exponent = new DimensionalExponents();
            if (unitString == "1")
                unitString = "";
            foreach (var item in UnitFactor.SymbolBreakDown(unitString))
            {
                if (item.TryGetDimensionalExponents(out var exp, out var ratio, out var off))
                {
                    Offset = off;
                    Exponent = Exponent.Multiply(exp);
                    Ratio *= ratio;
                }
                else
                {
                    IsValid = false;
                }
            }
            if (!Exponent.Equals(new DimensionalExponents(0, 0, 0, 0, 1, 0, 0)))
                Offset = 0;
        }

        /// <summary>
        /// Attempts conversion of the value to SI starting from the source unit
        /// </summary>
        /// <param name="valueSourceUnit">double value to convert</param>
        /// <param name="valueSI">out value in SI units</param>
        /// <returns>true if conversion is valid, false otherwise</returns>
        public bool TryConvertToSI(double valueSourceUnit, out double valueSI)
        {
            if (!IsValid)
            {
                valueSI = valueSourceUnit;
                return false;
            }
            valueSI = (valueSourceUnit + Offset) * Ratio;
            return true;
        }

        /// <summary>
        /// Attempts conversion of the value to source unit starting from the SI 
        /// </summary>
        /// <param name="valueSI">double value in SI units</param>
        /// <param name="valueSourceUnit">out double value converted</param>
        /// <returns>true if conversion is valid, false otherwise</returns>
        public bool TryConvertFromSI(double valueSI, out double valueSourceUnit)
        {
            if (!IsValid)
            {
                valueSourceUnit = valueSI;
                return false;
            }
            valueSourceUnit = valueSI / Ratio - Offset;
            return true;
        }
    }
}
