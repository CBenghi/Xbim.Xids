namespace Xbim.InformationSpecifications.Generator.Measures
{
    public class Measure
    {
        // public string Key { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string UnitSymbol { get; set; } = string.Empty;
        public string IfcMeasure { get; set; } = string.Empty;
        public string DimensionalExponents { get; set; } = string.Empty;
        public string UnitEnum { get; set; } = string.Empty;
    }
}
