using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Xbim.InformationSpecifications.Helpers
{
    public static class IPropertyTypeInfoExtensions
    {
        /// <summary>
        /// Helper method to determine if a measure rule is appropriate for a given property, given the value property type.
        /// </summary>
        /// <param name="prop">The property to be evaluated</param>
        /// <param name="measure"></param>
        /// <returns></returns>
        public static bool IsMeasureProperty(this IPropertyTypeInfo prop, [NotNullWhen(true)] out IfcMeasures? measure)
        {
            if (prop is not SingleValuePropertyType svp)
            {
                measure = null;
                return false;
            }
            var t = SchemaInfo.GetMeasure(svp.DataType);
            if (t.HasValue && Enum.TryParse<IfcMeasures>(t.Value.ID, out var found))
            {
                measure = found;
                return true;
            }
            switch (svp.DataType)
            {
                //// these could be number, but it needs to be addressed
                //case "IfcNormalisedRatioMeasure":
                //case "IfcThermalTransmittanceMeasure":
                //    measure = IfcMeasures.Undefined;
                //    return false;

                case "IfcText":
                case "IfcLabel":
                case "IfcBoolean":
                case "IfcLogical":
                case "IfcIdentifier":
                case "IfcDateTime": // from schema = STRING;
                case "IfcDate": // from schema = STRING;
                case "IfcDuration": // from schema = STRING;
                case "IfcTime": // from schema = STRING;
                    measure = IfcMeasures.String;
                    return true;
                case "IfcInteger":
                case "IfcReal":
                case "IfcThermalTransmittanceMeasure":
                case "IfcCountMeasure":
                case "IfcWarpingConstantMeasure":
                case "IfcThermalResistanceMeasure":
                case "IfcThermalExpansionCoefficientMeasure":
                    measure = IfcMeasures.Number;
                    return true;
                case "IfcNonNegativeLengthMeasure":
                case "IfcPositiveLengthMeasure":
                    measure = IfcMeasures.Length;
                    return true;
                case "IfcPositiveRatioMeasure":
                case "IfcNormalisedRatioMeasure":
                    measure = IfcMeasures.Ratio;
                    return true;
                case "IfcPositivePlaneAngleMeasure":
                    measure = IfcMeasures.PlaneAngle;
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
