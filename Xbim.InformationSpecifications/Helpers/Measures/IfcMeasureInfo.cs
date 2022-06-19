namespace Xbim.InformationSpecifications.Helpers
{
    // todo: Extension methods for the conversion of the value given 
    // If 
    //    Stringa -> No conversion
    //    number -> No conversion
    // Extension method To/From - con (double value, string unit) (m2/kg)
    // 


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
        public string Unit { get; }
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

        public string GetUnit()
        {
            if (!string.IsNullOrEmpty(Unit))
                return Unit;
            if (Exponents is not null)
                return Exponents.ToUnitSymbol();
            return "";
        }
    }
}
