using System;

namespace Xbim.InformationSpecifications.Helpers.Measures
{
    /// <summary>
    /// Class supporting the conversion from different units of measure
    /// </summary>
    [Obsolete("This class is obsolete. Use ids-lib instead.")]
    public class UnitConversion
    {
        /// <summary>
        /// Name of the unit being described
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Name of the equivalent unit
        /// </summary>
        public string Equivalent { get; }
        /// <summary>
        /// Conversion factor
        /// </summary>
        public double MultiplierToEquivalent { get; } = 1;
        /// <summary>
        /// Offset factor for the measure (i.e. for temperatures)
        /// </summary>
        public double Offset { get; } = 0;
        /// <summary>
        /// Other names of the unit
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        /// Constructor with offset
        /// </summary>
        public UnitConversion(double OrigQty, string name, double equivalentQty, string equivalent, double offset)
        {
            Name = name;
            Equivalent = equivalent;
            MultiplierToEquivalent = equivalentQty / OrigQty;
            Offset = offset;
            Aliases = Array.Empty<string>();
        }
        /// <summary>
        /// Constructor with aliases
        /// </summary>
        /// <param name="OrigQty"></param>
        /// <param name="name"></param>
        /// <param name="equivalentQty"></param>
        /// <param name="equivalent"></param>
        /// <param name="aliases"></param>
        public UnitConversion(double OrigQty, string name, double equivalentQty, string equivalent, params string[] aliases)
        {
            Name = name;
            Equivalent = equivalent;
            MultiplierToEquivalent = equivalentQty / OrigQty;
            Aliases = aliases;
        }
    }
}
