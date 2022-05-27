using System;
using System.Text.RegularExpressions;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications.Generator.Measures
{
    public class UnitFactor
    {
        public string UnitSymbol { get; set; }
        public int Exponent { get; set; } = 1;

        static Regex reUnitAndExponent = new("([a-zA-Z]+)(\\d*)");

        public UnitFactor(string unitAndExponent)
        {
            var m = reUnitAndExponent.Match(unitAndExponent);
            if (m.Success)
            {
                UnitSymbol = m.Groups[1].Value;
                if (m.Groups[2].Value != "")
                    Exponent = int.Parse(m.Groups[2].Value);
            }
            else
                UnitSymbol = unitAndExponent;
        }

        internal UnitFactor Invert()
        {
            Exponent *= -1;
            return this;
        }

        internal DimensionalExponents GetDimensionalExponents(MeasureCollection m)
        {
            var t = m.GetByUnit(UnitSymbol);
            if (t == null)
                return null;
            var de = DimensionalExponents.FromString(t.DimensionalExponents);
            if (de == null)
                return null;
            de.Elevate(Exponent);
            return de;
        }
    }
}
