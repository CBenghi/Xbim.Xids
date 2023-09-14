using Microsoft.Extensions.Logging;
using System;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// A constraint component based on an exact value option
    /// </summary>
    public class ExactConstraint : IValueConstraintComponent, IEquatable<ExactConstraint>
    {
        /// <summary>
        /// Basic constructor setting an exact value.
        /// The string is only evaluated as object upon checking 
        /// <see cref="IsSatisfiedBy(object, ValueConstraint, bool, ILogger?)"/>
        /// </summary>
        /// <param name="value">The value of the constraint, expressed as a string</param>
        public ExactConstraint(string value)
        {
            Value = value.Trim();
        }

        /// <summary>
        /// String representation of the value to match
        /// </summary>
        public string Value { get; set; }

        /// <inheritdoc />
        public bool IsSatisfiedBy(object candiatateValue, ValueConstraint context, bool ignoreCase, ILogger? logger = null)
        {
            if (context.BaseType == NetTypeName.Undefined && !ignoreCase)
            {
                // if we are comparing without a type constraint, we match the type of the 
                // candidate, rather than converting all to string.
                var toCheck = ValueConstraint.CastObject(Value, candiatateValue);
                return ValueConstraint.FormalEquals(candiatateValue, toCheck);
            }
            if (ignoreCase)
                return Value.Equals(candiatateValue.ToString(), comparisonType: StringComparison.OrdinalIgnoreCase);
            return Value.Equals(candiatateValue.ToString());
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (Value == null)
                return "Exact: <null>";
            return $"Exact: '{Value}'";
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            if (Value != null)
                return Value.GetHashCode();
            return base.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return Equals(obj as ExactConstraint);
        }

        /// <inheritdoc />
        public bool Equals(ExactConstraint? other)
        {
            if (other == null)
                return false;
            // using tuple's trick to evaluate equality
            return (Value, true).Equals((other.Value, true));
        }

        /// <inheritdoc />
        public string Short()
        {
            return Value;
        }

        /// <inheritdoc />
        public bool IsValid(ValueConstraint context)
        {
            var v = ValueConstraint.GetObject(Value, context.BaseType);
            // values need to be succesfully converted
            if (v is null && !string.IsNullOrEmpty(Value))
                return false;
            return true;
        }
    }
}
