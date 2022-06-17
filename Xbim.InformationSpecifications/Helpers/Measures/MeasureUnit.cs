using System;
using System.Collections.Generic;
using System.Text;
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
        public double Ratio { get; } = 1;
        public double Offset { get; } = 0;

        public bool IsValid { get; private set; } = true;

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

        public bool TryConvertFromSI(double valueSI, out double valueSourceUnit)
        {
            if (!IsValid)
            {
                valueSourceUnit = valueSI;
                return false;
            }
            valueSourceUnit = valueSI /  Ratio - Offset;
            return true;
        }
    }
}
