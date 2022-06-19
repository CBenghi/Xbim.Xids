using System;

namespace Xbim.InformationSpecifications.Helpers.Measures
{
    public class UnitConversion
    {
        public string Name { get; }
        public string Equivalent { get; }
        public double MultiplierToEquivalent { get; } = 1;
        public double Offset { get; } = 0;
        public string[] Aliases { get; }

        public UnitConversion(double OrigQty, string name, double equivalentQty, string equivalent, double offset)
        {
            Name = name;
            Equivalent = equivalent;
            MultiplierToEquivalent = equivalentQty / OrigQty;
            Offset = offset;
            Aliases = Array.Empty<string>();
        }
        public UnitConversion(double OrigQty, string name, double equivalentQty, string equivalent, params string[] aliases)
        {
            Name = name;
            Equivalent = equivalent;
            MultiplierToEquivalent = equivalentQty / OrigQty;
            Aliases = aliases;
        }
    }
}
