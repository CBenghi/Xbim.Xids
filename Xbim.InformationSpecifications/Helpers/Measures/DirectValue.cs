using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.InformationSpecifications.Helpers.Measures
{
    /// <summary>
    /// A value that does not need unit conversion
    /// </summary>
    public class DirectValue : IValueProvider
    {
        /// <summary>
        /// Default constructor by Id and Description
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        public DirectValue(string id, string description)
        {
            Id = id;
            Description = description;
        }

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public DimensionalExponents Exponents => throw new NotImplementedException();

        /// <inheritdoc />
        public string Description { get; }

        /// <inheritdoc />
        public string GetUnit()
        {
            return "";
        }

        internal static Dictionary<IfcValue, DirectValue> DirectValues { get; } = new()
        {
            { IfcValue.IfcText, new DirectValue("IfcText", "") },
            { IfcValue.IfcIdentifier, new DirectValue("IfcIdentifier", "") },
        };
    }
}
