using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.InformationSpecifications.Helpers
{
    public struct IfcMeasureInfo
    {
        public IfcMeasureInfo(string id, string measure, string description, string unit, string symbol, string exponents)
        {
            ID = id;
            IfcMeasure = measure;
            Description = description;
            Unit = unit;
            UnitSymbol = symbol;
            Exponents = DimensionalExponents.FromString(exponents);
        }

        public string ID { get; }
        public string IfcMeasure { get; }
        public string Description { get; }
        public string Unit { get;  }
        public string UnitSymbol { get; }
        public DimensionalExponents? Exponents { get; }


    }
}
