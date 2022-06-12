using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications.Generator.Measures
{
    public class UnitFactor
    {
        public string UnitSymbol { get; set; }
        public int Exponent { get; set; } = 1;

        static Regex reUnitAndExponent = new("(['°a-zA-Z]+)(\\d*)"); // letters ' for inches and feet and ° for degrees

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

        public UnitFactor Invert()
        {
            Exponent *= -1;
            return this;
        }

        public bool TryGetDimensionalExponents(
            [NotNullWhen(true)] out DimensionalExponents? exp,
            out double ratio,
            out double offset
            )
        {
            var smb = UnitSymbol;
            ratio = 1.0d;
            offset = 0;
            while (SchemaInfo.TryGetConversion(smb, out var cnv))
            {
                smb = cnv.Equivalent;
                offset = cnv.Offset;
                ratio *= Math.Pow(cnv.MultiplierToEquivalent, Exponent);
            }
            if (SchemaInfo.TryGetUnit(smb, out var oFnd))
            {
                if (oFnd is IfcMeasureInfo mi && mi.Exponents is DimensionalExponents)
                {
                    exp = DimensionalExponents.Elevated(mi.Exponents, Exponent);
                    return true;
                }
            }
            exp = null;
            return false;
        }

        public static List<(Regex rex, string replace)> Replacements = new List<(Regex, string)>
        {
            (new Regex("square (\\w+)\\b"), "$1 2"), // $1 is the group, 2 is the square, the space will be removed later $12 does not work
            (new Regex("cubic (\\w+)\\b"), "$1 3"), // $1 is the group, 3 is the cube, the space will be removed later
        };
            


        public static IEnumerable<UnitFactor> SymbolBreakDown(string unitSymbol)
        {
            var t = unitSymbol;
            foreach (var item in Replacements)
            {
                t = item.rex.Replace(t, item.replace);
            }
            t = Regex.Replace(t, " +(\\d+)", "$1");
            unitSymbol = t;

            var fraction = unitSymbol.Split('/');
            var num = fraction[0];
            var den = fraction.Length == 2 ? fraction[1] : "";

            var numUnits = num.Split(' ');
            var denUnits = den.Split(' ');

            foreach (var item in numUnits.Where(x => x != ""))
                yield return new UnitFactor(item);
            foreach (var item in denUnits.Where(x => x != ""))
                yield return new UnitFactor(item).Invert();
        }
    }
}
