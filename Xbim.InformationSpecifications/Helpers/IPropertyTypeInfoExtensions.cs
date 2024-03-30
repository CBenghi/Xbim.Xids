using IdsLib.IfcSchema;
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
        public static bool IsMeasureProperty(this IPropertyTypeInfo prop, [NotNullWhen(true)] out IfcMeasureInformation? measure)
        {
            if (prop is not SingleValuePropertyType svp)
            {
                measure = null;
                return false;
            }
            
            if (!IdsLib.IfcSchema.SchemaInfo.TryParseIfcDataType(svp.DataType, out var tmeasure, false) || tmeasure.Measure is null)
            {
                measure = null;
                return false;
            }
            measure = tmeasure.Measure;
            return true;
        }
    }
}
