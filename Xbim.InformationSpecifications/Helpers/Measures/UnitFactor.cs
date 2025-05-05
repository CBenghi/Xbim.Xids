using IdsLib.IfcSchema;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Xbim.InformationSpecifications.Helpers;
using Xbim.InformationSpecifications.Helpers.Measures;

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

        /// <summary>
        /// Constructor of unit with its exponent, e.g. m2
        /// </summary>
        /// <param name="unitAndExponent">e.g. ft3 for cubic feet or N for newton</param>
        public UnitFactor(string unitAndExponent)
        {
            var m = IfcMeasureInformation.BroadUnitComponentMatcher.Match(unitAndExponent);
            if (m.Success)
            {
                UnitSymbol = m.Groups["chars"].Value;
                Exponent = GetExponent(m.Groups["pow"].Value);
            }
            else
                UnitSymbol = unitAndExponent;
        }

        /// <summary>
        /// Cleans a string representing an exponent and returns it as an int
        /// </summary>
        [Obsolete("Use GetExponent(string) in ids-lib once available (from 1.0.92)")]
        private static int GetExponent(string exponentString)
        {
            if (string.IsNullOrWhiteSpace(exponentString))
                return 1;
            exponentString = exponentString.Replace("²", "2");
            exponentString = exponentString.Replace("³", "3");
            if (int.TryParse(exponentString, out var pow))
                return pow;
            return 0;
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

        /// <inheritdoc/>
        public override string ToString()
        {
            if (Exponent != 1)
                return $"{UnitSymbol}{Exponent}";
            return $"{UnitSymbol}";
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
            while (Conversion.TryGetConversion(smb, out var cnv))
            {
                smb = cnv.BaseUnit;
                offset = cnv.ConversionOffset ?? 0;
                ratio *= Math.Pow(cnv.ConversionValue, Exponent);
            }
            if (Conversion.TryGetUnit(smb, out var oFnd))
            {
                if (oFnd is IfcMeasureInformation mi && mi.Exponents is not null)
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
        public static (Regex rex, string replace)[] Replacements { get; } =
        [
            (new Regex("square (\\w+)\\b"), "$1 2"), // $1 is the group, 2 is the square, the space will be removed later $12 does not work
            (new Regex("cubic (\\w+)\\b"), "$1 3"), // $1 is the group, 3 is the cube, the space will be removed later
        ];

        private static char[] multiplicationSplitters = [' ', '\t', '·', '*', '×', '⋅', '⁎', '∗'];
        private static char[] divisionSplitters = ['/', ':'];

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

            var fraction = unitSymbol.Split(divisionSplitters, StringSplitOptions.RemoveEmptyEntries);
            var num = fraction[0].Trim();
            var den = fraction.Length == 2 ? fraction[1].Trim() : "";
            
            var numUnits = num.Split(multiplicationSplitters, StringSplitOptions.RemoveEmptyEntries);
            var denUnits = den.Split(multiplicationSplitters, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in numUnits)
                yield return new UnitFactor(item);
            foreach (var item in denUnits)
                yield return new UnitFactor(item).Invert();
        }
    }
}
