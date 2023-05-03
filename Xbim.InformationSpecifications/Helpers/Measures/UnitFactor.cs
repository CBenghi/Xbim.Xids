using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications.Generator.Measures
{
    /// <summary>
    /// Class to support a single unit and its exponent
    /// </summary>
    public class UnitFactor
    {
        /// <summary>
        /// Single unit simbol, e.g. m
        /// </summary>
        public string UnitSymbol { get; set; }

        /// <summary>
        /// Single unix exponent, e.g. 2 for m2
        /// </summary>
        public int Exponent { get; set; } = 1;

        static readonly Regex reUnitAndExponent = new("([°'a-zA-Z]+)(\\d*)"); // letters ' for inches and feet and ° for degrees \\xB0 is the degree symbol

        /// <summary>
        /// Constructor of unit with its exponent, e.g. m2
        /// </summary>
        /// <param name="unitAndExponent">e.g. ft3 for cubic feet or N for newton</param>
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

        /// <summary>
        /// Move from numerator to denominator or vice versa.
        /// </summary>
        /// <returns>self, inverted</returns>
        public UnitFactor Invert()
        {
            Exponent *= -1;
            return this;
        }

        /// <summary>
        /// Attempts the resolution of the dimensional exponent of the UnitSymbol, given the known conversion dictionaries
        /// </summary>
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
                if (oFnd is IfcMeasureInfo mi && mi.Exponents is not null)
                {
                    exp = DimensionalExponents.Elevated(mi.Exponents, Exponent);
                    return true;
                }
            }
            exp = null;
            return false;
        }

        /// <summary>
        /// Regex based replacements allow the conversion of some forms of expressing units e.g. Square m to -> m2
        /// </summary>
        public static List<(Regex rex, string replace)> Replacements { get; } = new()
        {
            (new Regex("square (\\w+)\\b"), "$1 2"), // $1 is the group, 2 is the square, the space will be removed later $12 does not work
            (new Regex("cubic (\\w+)\\b"), "$1 3"), // $1 is the group, 3 is the cube, the space will be removed later
        };


        /// <summary>
        /// tries to break down a complex string of multiple unitFactors
        /// using the <see cref="Replacements"/>, then returns a list of all the 
        /// units found with relative exponent.
        /// </summary>
        public static IEnumerable<UnitFactor> SymbolBreakDown(string unitSymbol)
        {
            var t = unitSymbol;
            foreach (var (rex, replace) in Replacements)
            {
                t = rex.Replace(t, replace);
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
