using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Xbim.InformationSpecifications.Helpers;

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
        public bool IsSatisfiedBy(object candidateValue, ValueConstraint context, bool ignoreCase, ILogger? logger = null)
        {

            if (ignoreCase && (context.BaseType == NetTypeName.Undefined || context.BaseType == NetTypeName.String))
                // Ignoring case only makes sense for text / undefined
                return string.Compare(Value, candidateValue.ToString(), CultureHelper.SystemCulture,
                    CompareOptions.IgnoreCase |     // Case Insensitive
                    CompareOptions.IgnoreNonSpace   // Ignore accents etc
                    ) == 0;


            return context.BaseType switch
            {
                NetTypeName.Undefined => IsUndefinedTypeSatisfied(candidateValue),

                NetTypeName.Floating => IsRealWithinTolerance(candidateValue),
                NetTypeName.Double => IsRealWithinTolerance(candidateValue),
                
                // Everything else uses exact string equality - including Decimal
                _ => IsEqualTo(candidateValue)
            };

        }

        private bool IsEqualTo(object candidateValue)
        {
            if(candidateValue is IFormattable f)
            {
                return Value.Equals(f.ToString(null, CultureHelper.SystemCulture));
            }
            else 
            {
                return Value.Equals(candidateValue.ToString());
            }

        }

        private bool IsUndefinedTypeSatisfied(object candidateValue)
        {
            // if we are comparing without a type constraint, we match the type of the 
            // candidate, rather than converting all to string.
            var expectedValue = ValueConstraint.ParseValue(Value, candidateValue);
            if (expectedValue is null)
            {
                return false;
            }
            return FormalEquals(expectedValue, candidateValue);
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
            var v = ValueConstraint.ParseValue(Value, context.BaseType);
            // values need to be succesfully converted
            if (v is null && !string.IsNullOrEmpty(Value))
                return false;
            return true;
        }


        
        internal static bool FormalEquals(object expectedValue, object? candidateValue)
        {
            // special casts for ifcTypes
            if (candidateValue?.GetType().Name == "IfcGloballyUniqueId")
                return expectedValue!.Equals(candidateValue.ToString());

            return candidateValue switch
            {
                float => ValueConstraint.IsEqualWithinTolerance(GetSingle(expectedValue), GetSingle(candidateValue)),
                double => ValueConstraint.IsEqualWithinTolerance(GetDouble(expectedValue), GetDouble(candidateValue)),
                // Use decimal as means to compare equality of integral numbers - boxed int 42 != long 42
                decimal => GetDecimal(expectedValue) == GetDecimal(candidateValue),
                int => GetDecimal(expectedValue) == GetDecimal(candidateValue),
                short => GetDecimal(expectedValue) == GetDecimal(candidateValue),
                long => GetDecimal(expectedValue) == GetDecimal(candidateValue),
                _ => expectedValue.Equals(candidateValue)
            };
        }

        private bool IsRealWithinTolerance(object candidateValue)
        {
            return candidateValue switch
            {
                float =>  ValueConstraint.IsEqualWithinTolerance(GetSingle(Value), GetSingle(candidateValue)),
                double => ValueConstraint.IsEqualWithinTolerance(GetDouble(Value), GetDouble(candidateValue)),
                _ => false
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetSingle(object value)
        {
            return Convert.ToSingle(value, CultureHelper.SystemCulture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double GetDouble(object value)
        {
            return Convert.ToDouble(value, CultureHelper.SystemCulture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static decimal GetDecimal(object value)
        {
            return Convert.ToDecimal(value, CultureHelper.SystemCulture);
        }

    }
}
