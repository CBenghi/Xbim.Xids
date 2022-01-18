using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.InformationSpecifications.Helpers
{
    public class IfcMeasureInfo
    {
        public IfcMeasureInfo(string v1, string v2, string v3, string v4)
        {
            IfcMeasure = v1;
            PhysicalQuantity = v2;
            Unit = v3;
            UnitSymbol = v4;
        }

        public string IfcMeasure { get; set; }
        public string PhysicalQuantity { get; set; }
        public string Unit { get; set; }
        public string UnitSymbol { get; set; }
    }
}
