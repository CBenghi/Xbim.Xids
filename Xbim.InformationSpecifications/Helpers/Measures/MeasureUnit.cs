using System;
using System.Collections.Generic;
using System.Text;
using Xbim.InformationSpecifications.Generator.Measures;

namespace Xbim.InformationSpecifications.Helpers.Measures
{
    public class MeasureUnit
    {
        public DimensionalExponents Exponent { get; }
        public double Ratio { get; } = 1;
        public double Offset { get; } = 0;

        public bool IsValid { get; private set; } = true;

        public MeasureUnit(string unitString)
        {
            Exponent = new DimensionalExponents();
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
