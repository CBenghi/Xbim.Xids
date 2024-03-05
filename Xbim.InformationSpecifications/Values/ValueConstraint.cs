using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// Captures the nature of constraints.
    /// 
    /// The simplest form is a single undefined value, meaning no type is constrained, so the 
    /// check is performed against a string conversion.
    ///
    /// Otherwise it can be composed of several possible constraints.
    /// </summary>
    public partial class ValueConstraint : IEquatable<ValueConstraint>
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        public ValueConstraint() { }

        /// <summary>
        /// Add an accepted constraint component, will be removed, use <see cref="AddAccepted(IValueConstraintComponent)"/>
        /// </summary>
        /// <param name="newConstraint"></param>
        [Obsolete("Will be removed, use AddAccepted")]
        public void Add(IValueConstraintComponent newConstraint)
        {
            AcceptedValues ??= new List<IValueConstraintComponent>();
            AcceptedValues.Add(newConstraint);
        }

        /// <summary>
        /// Add an accepted constraint component
        /// </summary>
        public void AddAccepted(IValueConstraintComponent constraint)
        {
            AcceptedValues ??= new List<IValueConstraintComponent>();
            AcceptedValues.Add(constraint);
        }

        /// <summary>
        /// Cheks that there's at list one entry in the non-null list of accepted values.
        /// </summary>
        /// <returns>True if the list is not null and not empty</returns>
        [MemberNotNullWhen(true, nameof(AcceptedValues))]
        public bool HasAnyAcceptedValue()
        {
            if (AcceptedValues is null)
                return false;
            return AcceptedValues.Any();
        }

        /// <summary>
        /// The list of accepted values, use <see cref="HasAnyAcceptedValue"/> to check for content
        /// </summary>
        public List<IValueConstraintComponent>? AcceptedValues { get; set; }

        /// <summary>
        /// Evaluates a candidate value, against the constraints
        /// </summary>
        /// <param name="candidateValue">value to evaluate</param>
        /// <param name="ignoreCase">when <c>true</c> ignore case and accents; otherwise when <c>false</c> tests for exact match</param>
        /// <param name="logger">logging context</param>
        /// <returns>true if satisfied, false otherwise</returns>
        public bool IsSatisfiedBy([NotNullWhen(true)] object? candidateValue, bool ignoreCase, ILogger? logger = null)
        {
            if (candidateValue is null)
                return false;
            if (BaseType != NetTypeName.Undefined && !IsCompatible(ResolvedType(BaseType), candidateValue.GetType()))
                return false;
            // if there are no constraints it's satisfied by default // todo: should this be revised?
            if (AcceptedValues == null || !AcceptedValues.Any())
                return true;
            var candidateObject = ConvertObject(candidateValue, BaseType);
            if (candidateObject is null)
                return false;

            foreach (var av in AcceptedValues)
            {
                if (av.IsSatisfiedBy(candidateObject, this, ignoreCase, logger))
                    return true;
            }
            
            return false;
        }

        /// <summary>
        /// Evaluates a candidate value, against the constraints, where strings are compared case-sensitively
        /// </summary>
        /// <param name="candidateValue">value to evaluate</param>
        /// <param name="logger">the logging context</param>
        /// <returns>true if satisfied, false otherwise</returns>
        public bool IsSatisfiedBy([NotNullWhen(true)] object? candidateValue, ILogger? logger = null)
        {
            return IsSatisfiedBy(candidateValue, false, logger);
        }

        /// <summary>
        /// Evaluates a candidate value against the constraints, where strings are compared case-insensitively
        /// </summary>
        /// <param name="candidateValue">value to evaluate</param>
        /// <param name="logger">the logging context</param>
        /// <returns>true if satisfied, false otherwise</returns>
        public bool IsSatisfiedIgnoringCaseBy([NotNullWhen(true)] object? candidateValue, ILogger? logger = null)
        {
            return IsSatisfiedBy(candidateValue, true, logger);
        }

        static private bool IsCompatible([NotNullWhen(true)] Type? destType, Type passedType)
        {
            if (destType is null)
                return false;
            if (
                destType == typeof(long) // int64
                )
            {
                if (typeof(double) == passedType)
                    return false;
                if (typeof(int) == passedType)
                    return true;
                if (typeof(long) == passedType)
                    return true;
            }
            if (destType == typeof(decimal))
            {
                if (typeof(double) == passedType)
                    return true;
                if (typeof(int) == passedType)
                    return true;
                if (typeof(long) == passedType)
                    return true;
            }
            if (destType.IsAssignableFrom(passedType))
                return true;
            if (destType == typeof(string))
                return true; // we can always convert to string
            return false;
        }

        /// <summary>
        /// Initializes a constraint on string exact value, and string <see cref="BaseType"/>
        /// </summary>
        /// <param name="value">The value to set as exact string constraint</param>
        public ValueConstraint(string value)
        {
            AcceptedValues = new List<IValueConstraintComponent>
            {
                new ExactConstraint(value)
            };
            BaseType = NetTypeName.String;
        }


        /// <summary>
        /// Constructor by type enumeration
        /// </summary>
        public ValueConstraint(NetTypeName value)
        {
            BaseType = value;
            AcceptedValues = new List<IValueConstraintComponent>();
        }

        /// <summary>
        /// Constructor by string values
        /// </summary>
        public ValueConstraint(IEnumerable<string> stringValues)
        {
            BaseType = NetTypeName.String;
            AcceptedValues = new List<IValueConstraintComponent>(stringValues.Select(x => new ExactConstraint(x)));
        }

        /// <summary>
        /// Constructor by type enumeration and string representation of an acceptable value
        /// </summary>
        /// <param name="valueType">type</param>
        /// <param name="value">The value will be parsed when testing IsSatisfiedBy according to the <paramref name="valueType"/></param>
        public ValueConstraint(NetTypeName valueType, string value)
        {
            AcceptedValues = new List<IValueConstraintComponent>
            {
                new ExactConstraint(value)
            };
            BaseType = valueType;
        }

        /// <summary>
        /// Initializes a constraint on int exact value, and int <see cref="BaseType"/>
        /// </summary>
        /// <param name="value">The value to set as exact int constraint</param>
        public ValueConstraint(int value)
        {
            AcceptedValues = new List<IValueConstraintComponent>
            {
                new ExactConstraint(value.ToString(CultureHelper.SystemCulture))
            };
            BaseType = NetTypeName.Integer;
        }

        /// <summary>
        /// Initializes a constraint on decimal exact value, and decimal <see cref="BaseType"/>
        /// </summary>
        /// <param name="value">The value to set as exact decimal constraint</param>
        public ValueConstraint(decimal value)
        {
            AcceptedValues = new List<IValueConstraintComponent>
            {
                new ExactConstraint(value.ToString(CultureHelper.SystemCulture))
            };
            BaseType = NetTypeName.Decimal;
        }

        /// <summary>
        /// Initializes a constraint on double exact value, and double <see cref="BaseType"/>
        /// </summary>
        /// <param name="value">The value to set as exact double constraint</param>
        public ValueConstraint(double value)
        {
            AcceptedValues = new List<IValueConstraintComponent>
            {
                new ExactConstraint(value.ToString(CultureHelper.SystemCulture))
            };
            BaseType = NetTypeName.Double;
        }

        /// <summary>
        /// BaseType to be used for the evaluation of constraints
        /// </summary>
        public NetTypeName BaseType { get; set; }

        /// <summary>
        /// checks if the value has a meaningful constraint (includes null check)
        /// </summary>
        /// <param name="value">the constraint to check</param>
        /// <returns>true if meaningful, false otherwise</returns>
        public static bool IsNotEmpty([NotNullWhen(true)] ValueConstraint? value)
        {
            if (value == null)
                return false;
            return !value.IsEmpty();
        }

        /// <summary>
        /// checks if the value has no meaningful constraint (includes null check)
        /// </summary>
        /// <param name="value">the constraint to check</param>
        /// <returns>true if meaningful, false otherwise</returns>
        public static bool IsEmpty(ValueConstraint? value)
        {
            if (value == null)
                return true;
            return value.IsEmpty();
        }

        /// <summary>
        /// checks that the value is not significant, i.e. 
        /// 1) its type is undefined, and
        /// 2) there are no accepted values
        /// </summary>
        /// <returns>true if not significant, false otherwise</returns>
        public bool IsEmpty()
        {
            return BaseType == NetTypeName.Undefined
                &&
                (AcceptedValues == null || !AcceptedValues.Any());
        }

        /// <inheritdoc />
        public bool Equals([NotNullWhen(true)] ValueConstraint? other)
        {
            if (other == null)
                return false;
            if (!BaseType.Equals(other.BaseType))
                return false;
            if (AcceptedValues == null && other.AcceptedValues != null)
                return false;
            if (AcceptedValues != null && other.AcceptedValues == null)
                return false;
            if (AcceptedValues != null)
            {
                var comp = new Helpers.MultiSetComparer<IValueConstraintComponent>();
                if (!comp.Equals(AcceptedValues, other.AcceptedValues))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Converts from a net type name to the type
        /// </summary>
        /// <returns>Can return null if the starting enum is Undefined</returns>
        public static Type? ResolvedType(NetTypeName Name)
        {
            return Name switch
            {
                NetTypeName.Floating => typeof(float),
                NetTypeName.Double => typeof(double),
                NetTypeName.Integer => typeof(long),
                NetTypeName.Decimal => typeof(decimal),
                NetTypeName.Date or NetTypeName.DateTime => typeof(DateTime),
                NetTypeName.Time or NetTypeName.Duration => typeof(TimeSpan),
                NetTypeName.String => typeof(string),
                NetTypeName.Boolean => typeof(bool),
                NetTypeName.Uri => typeof(Uri),
                NetTypeName.Undefined => null,
                _ => typeof(string),
            };
        }

        /// <summary>
        /// Returns a default value given a type.
        /// </summary>
        public static object? GetDefault(NetTypeName netTypeName, ILogger? logger = null)
        {
            if (netTypeName == NetTypeName.String)
                return "";
            if (netTypeName == NetTypeName.Uri)
                return new Uri(".", UriKind.Relative);
            var newT = GetNetType(netTypeName);
            if (newT is null)
                return null;
            if (newT == typeof(string))
                return "";
            try
            {
                return Activator.CreateInstance(newT);
            }
            catch
            {
                logger?.LogWarning("Default value for {netTypeName} provided as null for activator failure.", netTypeName);
                return null;
            }
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return Equals(obj as ValueConstraint);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (AcceptedValues == null || !AcceptedValues.Any())
                return $"{BaseType}";
            return string.Join(" or ", AcceptedValues.Select(x => x.Short()).ToArray());
        }

        /// <summary>
        /// Produces a short text description of the constraint
        /// </summary>
        /// <returns>A description string</returns>
        public string Short()
        {
#if OldShort
            if (IsSingleUndefinedExact(out var exact))
            {
                return $"of value '{exact}'";
            }
            var ret = new List<string>();
            if (BaseType != NetTypeName.Undefined)
                ret.Add($"of type {BaseType.ToString().ToLowerInvariant()}");
            else
                ret.Add($"of any type");
            if (AcceptedValues != null && AcceptedValues.Any())
            {
                ret.Add($"valid if value {string.Join(" or ", AcceptedValues.Select(x => x.Short()).ToArray())}");
            }
            return string.Join(", ", ret);
#endif
            if (AcceptedValues != null && AcceptedValues.Any())
            {
                var values = string.Join(" or ", AcceptedValues.Select(x => x.Short()).ToArray());
                return $"{values}";
            }
            else
            {
                return "<any>";
            }
        }

        /// <summary>
        /// Helper function for nullable support. 
        /// Pass a possibly null <see cref="ValueConstraint"/> asserting non null out exact values, if appropriate.
        /// See <see cref=" IsSingleUndefinedExact(out string?)"/>
        /// </summary>
        /// <param name="value">The constraint to check</param>
        /// <param name="exact">the exact single constraint defined, converted to string, or null if the return is false</param>
        /// <returns>true if the test is passed.</returns>
        public static bool IsSingleUndefinedExact(ValueConstraint? value, [NotNullWhen(true)] out string? exact)
        {
            if (value is null)
            {
                exact = null;
                return false;
            }
            return value.IsSingleUndefinedExact(out exact);
        }

        /// <summary>
        /// Tests that the constraint type is undefined and there's a single exactConstraint in the accepted values list.
        /// </summary>
        /// <param name="exact">the exact single constraint defined, converted to string, or null if the return is false</param>
        /// <returns>the boolean result of the test</returns>
        public bool IsSingleUndefinedExact([NotNullWhen(true)] out string? exact)
        {
            if (BaseType != NetTypeName.Undefined || AcceptedValues == null || AcceptedValues.Count != 1)
            {
                exact = null;
                return false;
            }
            if (AcceptedValues.FirstOrDefault() is not ExactConstraint unique)
            {
                exact = null;
                return false;
            }
            exact = unique.Value.ToString(CultureHelper.SystemCulture);
            return true;
        }

        /// <summary>
        /// Helps UI creation for the cases where a ValueConstraint is specified by a single value (the UI can be compressed).
        /// </summary>
        /// <param name="value">the constraint to evaluate</param>
        /// <param name="exact">returns the single exact constraint value as an object, if the return value is true</param>
        public static bool IsSingleExact([NotNullWhen(true)] ValueConstraint? value, [NotNullWhen(true)] out object? exact)
        {
            if (value is null)
            {
                exact = null;
                return false;
            }
            return value.IsSingleExact(out exact);
        }

        /// <summary>
        /// Tests that there'a single exactConstraint in the accepted values, and provides it for consumption.
        /// </summary>
        /// <param name="exact">
        /// The single exact constraint value defining the constraint, null if the test is not passed and return value is false.
        /// This is always a string.
        /// </param>
        /// <returns>true if check is passed, false otherwise</returns>
        public bool IsSingleExact([NotNullWhen(true)] out object? exact)
        {
            if (AcceptedValues == null || AcceptedValues.Count != 1)
            {
                exact = null;
                return false;
            }
            if (AcceptedValues.FirstOrDefault() is not ExactConstraint unique)
            {
                exact = null;
                return false;
            }
            exact = unique.Value;
            return true;
        }

        /// <summary>
        /// Helps UI creation for the cases where a ValueConstraint is specified by a single value (the UI can be compressed).
        /// This override allows the specification of the expected <typeparamref name="RequiredType"/>.
        /// </summary>
        /// <param name="value">the constraint to evaluate</param>
        /// <param name="exact">returns the single exact constraint value as an object, if the return value is true</param>
        public static bool IsSingleExact<RequiredType>([NotNullWhen(true)] ValueConstraint? value, [NotNullWhen(true)] out RequiredType? exact)
        {
            if (value is null)
            {
                exact = default;
                return false;
            }
            return value.IsSingleExact<RequiredType?>(out exact);
        }

        /// <summary>
        /// Checks that there'a single exactConstraint in the accepted values and its value is of type <typeparamref name="RequiredType"/>, and provides it for consumption 
        /// </summary>
        /// <param name="exact">The single exact constraint value defining the constraint, null if the check is not passed</param>
        /// <returns>true if check is passed, false otherwise</returns>
        public bool IsSingleExact<RequiredType>([NotNullWhen(true)] out RequiredType? exact)
        {
            exact = default;
            if (!IsSingleExact(out var val))
                return false;
            if (val is null)
                return false;
            var vbt = ConvertObject(val, BaseType);
            if (vbt is RequiredType exactAs)
            {
                exact = exactAs;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Implicit operator to convert from a string, notice that the required type is undefined
        /// </summary>
        public static implicit operator ValueConstraint(string singleUndefinedExact) => SingleUndefinedExact(singleUndefinedExact);

        /// <summary>
        /// Explicit constructor from string.
        /// </summary>
        /// <param name="content">the value of a single undefined exact string, notice that the required type is undefined</param>
        /// <returns></returns>
        public static ValueConstraint SingleUndefinedExact(string content)
        {
            var ret = new ValueConstraint()
            {
                BaseType = NetTypeName.Undefined,
                AcceptedValues = new List<IValueConstraintComponent>() { new ExactConstraint(content) }
            };
            return ret;
        }

        /// <summary>
        /// Determines if the the constraint is correctly formed
        /// </summary>
        /// <returns>True if the instance is valid, false otherwise</returns>
        public bool IsValid()
        {
            // Todo: idstalk: is empty AcceptedValues valid?
            if (AcceptedValues is not null)
            {
                foreach (var item in AcceptedValues)
                {
                    if (!item.IsValid(this))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// The Default precision to use for Real equality testing
        /// </summary>
        internal const double DefaultRealPrecision = 1e-6;
        /// <summary>
        /// Determines if a Real value is equal to the expected value accounting for floating point tolerances
        /// </summary>
        /// <param name="expectedValue">The precise double value expected</param>
        /// <param name="candidate">The candidate double value which may have FP imprecisions</param>
        /// <param name="tolerance">The double tolerance. Defaults to 0.0000001</param>
        /// <param name="decimals">The number of decimals to round to</param>
        /// <returns></returns>
        internal static bool IsEqualWithinTolerance(double expectedValue, double candidate, double tolerance = DefaultRealPrecision, int decimals = 6)
        {
            // Based on https://github.com/buildingSMART/IDS/issues/36#issuecomment-1014473533
            var lowerBound = Math.Round(expectedValue * (1 - tolerance), decimals);
            var upperBound = Math.Round(expectedValue * (1 + tolerance), decimals);

            return candidate >= lowerBound && candidate <= upperBound;
        }
    }
}
