using System;
using System.Diagnostics.CodeAnalysis;

namespace Xbim.InformationSpecifications.Helpers
{
    /// <summary>
    /// Extension methods for IPropertyTypeInfo
    /// </summary>
    public static class IPropertyTypeInfoExtensions
    {
        /// <summary>
        /// Helper method to determine if a measure rule is appropriate for a given property, given the value property type.
        /// </summary>
        /// <param name="prop">The property to be evaluated</param>
        /// <param name="measure"></param>
        /// <returns></returns>
        public static bool IsMeasureProperty(this IPropertyTypeInfo prop, [NotNullWhen(true)] out IfcValue? measure)
        {
            if (prop is not SingleValuePropertyType svp)
            {
                measure = null;
                return false;
            }
            var t = SchemaInfo.GetMeasure(svp.DataType);
            if (t is not null && Enum.TryParse<IfcValue>(t.Id, out var found))
            {
                measure = found;
                return true;
            }

            if (Enum.TryParse<IfcValue>(svp.DataType, out var fnd))
            {
                measure = fnd;
                return true;
            }

            switch (svp.DataType)
            {
                case "IfcText":
                case "IfcLabel":
                case "IfcBoolean":
                case "IfcLogical":
                case "IfcIdentifier":
                case "IfcDateTime": // from schema = STRING;
                case "IfcDate": // from schema = STRING;
                case "IfcDuration": // from schema = STRING;
                case "IfcTime": // from schema = STRING;
                    measure = null; 
                    return false;
                case "IfcInteger":
                case "IfcReal":
                case "IfcThermalTransmittanceMeasure":
                case "IfcCountMeasure":
                case "IfcWarpingConstantMeasure":
                case "IfcThermalResistanceMeasure":
                case "IfcThermalExpansionCoefficientMeasure":
                    measure = null;
                    return false;
                case "IfcNonNegativeLengthMeasure":
                case "IfcPositiveLengthMeasure":
                    measure = IfcValue.IfcLengthMeasure;
                    return true;
                case "IfcPositiveRatioMeasure":
                case "IfcNormalisedRatioMeasure":
                    measure = IfcValue.IfcRatioMeasure;
                    return true;
                case "IfcPositivePlaneAngleMeasure":
                    measure = IfcValue.IfcPlaneAngleMeasure;
                    return true;
                default:
                    break;
            }
            // Debug.WriteLine($"not found: {svp.DataType}");
            measure = null;
            return false;
        }
    }
}
