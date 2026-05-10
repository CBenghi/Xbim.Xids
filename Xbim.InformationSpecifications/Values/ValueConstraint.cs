using IdsLib.IdsSchema.XsNodes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using Xbim.InformationSpecifications.Helpers;
using Xbim.InformationSpecifications.Values;

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
		private static readonly int MaximumValuesToDisplay =
			Xids.Settings.MaximumConstraintEnumsToDescribe > 0 ? Xids.Settings.MaximumConstraintEnumsToDescribe : 1000;

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
			if (BaseType != NetTypeName.Undefined)
			{
				if (
					!IsCompatible(ResolvedType(BaseType), candidateValue.GetType()) &&
					!IsConvertible(BaseType, candidateValue))
					return false;

			}
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

		private static bool IsConvertible(NetTypeName typename, object candidateValue)
		{
			try
			{
				var val = ConvertObject(candidateValue, typename); // IsConvertible
				return val != null;
			}
			catch (FormatException)
			{
				return false;
			}
			catch (InvalidCastException)
			{
				return false;
			}
			catch (OverflowException)
			{
				return false;
			}
		}

		static private bool IsCompatible([NotNullWhen(true)] Type? destType, Type passedType)
		{
			if (destType is null)
				return false;
			if (
				destType == typeof(long) // int64
				)
			{
				if (typeof(int) == passedType)
					return true;
				if (typeof(long) == passedType)
					return true;
				// Reals can be compatible (sometimes). e.g. 5.0d == 5l
				if (typeof(double) == passedType)
					return true;
				if (typeof(float) == passedType)
					return true;
				if (typeof(decimal) == passedType)
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
			if (destType == typeof(double))
			{
				if (typeof(double) == passedType)
					return true;
				if (typeof(float) == passedType)
					return true;
				if (typeof(decimal) == passedType)
					return true;
				if (typeof(int) == passedType)
					return true;
				if (typeof(long) == passedType)
					return true;
			}
			if (destType == typeof(float))
			{
				if (typeof(float) == passedType)
					return true;
				if (typeof(double) == passedType)
					return true;
				if (typeof(decimal) == passedType)
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
		public ValueConstraint(NetTypeName valueType)
		{
			BaseType = valueType;
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
		/// Constructor by type enumeration and object value
		/// </summary>
		/// <param name="valueType">type</param>
		/// <param name="value">The value will be parsed and tested for compatibility with the <paramref name="valueType"/></param>
		/// <returns>NULL if any checks between the object and the type fail</returns>
		public static ValueConstraint? FromObject(NetTypeName valueType, object? value)
		{
			if (valueType == NetTypeName.Undefined || value is null)
				return null;
			var stringValue = PersistValue(value, valueType);
			if (stringValue == null)
				return null;
			var t = new ValueConstraint()
			{
				BaseType = valueType,
				AcceptedValues = new List<IValueConstraintComponent> { new ExactConstraint(stringValue) }
			};
			return t;
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
				NetTypeName.Duration => typeof(TimeSpan),
				NetTypeName.Time => typeof(TimeOfDay),
				NetTypeName.String => typeof(string),
				NetTypeName.Boolean => typeof(bool),
				NetTypeName.Uri => typeof(Uri),
				NetTypeName.Undefined => null,
				_ => typeof(string),
			};
		}

		/// <summary>
		/// Attempts to parse a string value according to the specified XML Schema (XSD) type name.
		/// </summary>
		/// <remarks>If the input string is not valid for the specified XSD type, or if the type is undefined, the
		/// method returns false and sets returnValue to null. For NetTypeName.Integer, the value is limited to Int32, which
		/// may differ from the unbounded integer type in XML Schema.</remarks>
		/// <param name="raw">The string representation of the value to parse.</param>
		/// <param name="xsdTypeName">The XSD type to use for parsing the value.</param>
		/// <param name="returnValue">When this method returns, contains the parsed value if the conversion succeeded; otherwise, null. The type of the
		/// value corresponds to the specified XSD type.</param>
		/// <returns>TRUE if the value was successfully parsed according to the specified XSD type; otherwise, FALSE.</returns>
		public static bool TryParseXsdValue(string? raw, NetTypeName xsdTypeName, [NotNullWhen(true)] out object? returnValue)
		{
			var baseTypeAsString = XsTypes.GetBaseFrom(GetXsdTypeString(xsdTypeName));
			if (string.IsNullOrEmpty(raw) || xsdTypeName == NetTypeName.Undefined)
			{
				returnValue = null;
				return false;
			}
			if (!XsTypes.IsValid(raw!, baseTypeAsString)) // we defer the check to the ids-lib
			{
				returnValue = null;
				return false;
			}
			returnValue = xsdTypeName switch
			{
				NetTypeName.String => raw,
				NetTypeName.Boolean => XmlConvert.ToBoolean(raw),
				NetTypeName.Integer => XmlConvert.ToInt32(raw), // todo: in xml it is unbounded, here we limit to int32, should we revise this?
				NetTypeName.Double => XmlConvert.ToDouble(raw),
				NetTypeName.Floating => XmlConvert.ToSingle(raw),
				NetTypeName.Decimal => XmlConvert.ToDecimal(raw),
				NetTypeName.Duration => XmlConvert.ToTimeSpan(raw),
				NetTypeName.DateTime => XmlConvert.ToDateTime(raw, XmlDateTimeSerializationMode.RoundtripKind),
				NetTypeName.Date => XmlConvert.ToDateTime(raw, XmlDateTimeSerializationMode.RoundtripKind),
				NetTypeName.Time => FixTime(raw), // encapsulated becase of possible exceptions, even with regex
				NetTypeName.Uri => new Uri(raw, UriKind.RelativeOrAbsolute),
				_ => null
			};
			return returnValue != null;
		}

		private static object? FixTime(string? raw)
		{
			if (raw is null)
				return null;
			try
			{
				// this can throw because correct regex structure could be above allowed numbers, eg. 25 hours.
				return new TimeOfDay(raw);
			}
			catch (Exception)
			{
				return null;
			}
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
				var listLen = AcceptedValues.Count;
				if (listLen > MaximumValuesToDisplay) // truncate very long lists
				{
					var shortList = string.Join("' or '", AcceptedValues.Take(MaximumValuesToDisplay).Select(x => x.Short()).ToArray());

					return $"'{shortList}' or {listLen - MaximumValuesToDisplay} others";
				}
				else if (listLen > 1)   // short list
				{
					if (AcceptedValues.All(v => v is ExactConstraint))
					{
						return "'" + string.Join("' or '", AcceptedValues.OfType<ExactConstraint>().OrderBy(x => x.Value).Select(x => x.Short()).ToArray()) + "'";
					}
					else
					{
						// Complex Constraint - pattern, range etc.
						return string.Join(" or ", AcceptedValues.Select(x => x.Short()).ToArray());
					}
				}
				else // Single item
				{
					return AcceptedValues.Single().Short();
				}
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
		/// Determines whether the current value constraint represents a single, exact value of a specific type, and provides the relevant value and type information if so.
		/// </summary>
		/// <param name="exact">On success, contains the exact value represented by the constraint</param>
		/// <param name="tp">On success, contains the .NET type of the exact value</param>
		/// <returns>TRUE if the constraint represents a single, exact value of a specific type; otherwise, FALSE.</returns>
		public bool IsSingleExactTyped([NotNullWhen(true)] out object? exact, out NetTypeName tp)
		{
			return ValueConstraint.IsSingleExactTyped(this, out exact, out tp);
		}

		/// <summary>
		/// Determines whether the specified value constraint represents a single exact value of a specific type,
		/// and provides the relevant value and type information if so.
		/// </summary>
		/// <remarks>Use this method to extract a single, strongly-typed value from a value constraint when it is
		/// known to represent exactly one value. If the constraint does not represent a single exact value, the output
		/// parameters are set to their default values.</remarks>
		/// <param name="value">The value constraint to evaluate. Must not be null.</param>
		/// <param name="exact">On success, contains the exact value represented by the constraint</param>
		/// <param name="tp">On success, contains the .NET type of the exact value</param>
		/// <returns>TRUE if the constraint represents a single, exact value of a specific type; otherwise, FALSE.</returns>
		public static bool IsSingleExactTyped(
			[NotNullWhen(true)] ValueConstraint? value,
			[NotNullWhen(true)] out object? exact,
			out NetTypeName tp)
		{
			tp = NetTypeName.Undefined;
			exact = null;
			if (value is null)
			{
				return false;
			}
			if (value.AcceptedValues == null || value.AcceptedValues.Count != 1)
			{
				return false;
			}
			if (value.AcceptedValues.FirstOrDefault() is not ExactConstraint unique)
			{
				return false;
			}
			tp = value.BaseType;
			exact = ParseValue(unique.Value, value.BaseType);
			return exact is not null;
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
			var vbt = ConvertObject(val, BaseType); // from IsSingleExact 
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
		/// Determines if a Real value is equal to the expected value accounting for floating point tolerances
		/// </summary>
		/// <param name="expectedValue">The precise double value expected</param>
		/// <param name="candidate">The candidate double value which may have FP imprecisions</param>
		/// <param name="tolerance">The double tolerance. Defaults to 1e-6</param>
		/// <returns></returns>
		internal static bool IsEqualWithinTolerance(double expectedValue, double candidate, double tolerance = RealHelper.DefaultRealPrecision)
		{
			// handle the trivial equality case first
			if (expectedValue == candidate)
				return true;

			// Account for FP precison issues
			// Based on https://github.com/buildingSMART/IDS/issues/36#issuecomment-1014473533
			(var lowerBound, var upperBound) = RealHelper.GetPrecisionBounds(expectedValue, tolerance);


			return candidate >= lowerBound && candidate <= upperBound;

		}

	}
}
