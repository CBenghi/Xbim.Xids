using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.InformationSpecifications.Helpers
{
    public struct IfcMeasureInfo
    {
        public IfcMeasureInfo(string id, string measure, string description, string unit, string symbol, string exponents, string[] concrete)
        {
            ID = id;
            IfcMeasure = measure;
            Description = description;
            Unit = unit;
            UnitSymbol = symbol;
            Exponents = DimensionalExponents.FromString(exponents);
            ConcreteClasses = concrete; 
        }

        /// <summary>
        /// The string ID found in the XML persistence
        /// </summary>
        public string ID { get; }
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
        public string Unit { get;  }
        /// <summary>
        /// Symbol used to present the unit.
        /// </summary>
        public string UnitSymbol { get; }
        /// <summary>
        /// Dimensional exponents useful for conversion to other units.
        /// </summary>
        public DimensionalExponents? Exponents { get; }

        /// <summary>
        /// Concrete implementing classes with namespace
        /// </summary>
        public string[] ConcreteClasses { get; }


    }
}
