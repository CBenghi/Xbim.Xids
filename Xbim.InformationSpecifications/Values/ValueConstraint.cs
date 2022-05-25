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
		public ValueConstraint() { }

		public void Add(IValueConstraint newConstraint)
		{
			AcceptedValues ??= new List<IValueConstraint>();
			AcceptedValues.Add(newConstraint);
		}

		public void AddAccepted(IValueConstraint constraint)
		{
			AcceptedValues ??= new List<IValueConstraint>();
			AcceptedValues.Add(constraint);
		}

		public List<IValueConstraint>? AcceptedValues { get; set; }

		public bool IsSatisfiedBy([NotNullWhen(true)] object? candiatateValue, bool ignoreCase, ILogger? logger = null)
		{
			if (candiatateValue is null)
				return false;
			if (BaseType != TypeName.Undefined && !IsCompatible(ResolvedType(BaseType), candiatateValue.GetType()))
				return false;
			// if there are no constraints it's satisfied by default
			if (AcceptedValues == null || !AcceptedValues.Any())
				return true;
			var cand = GetObject(candiatateValue, BaseType);
			if (cand is null)
				return false;
			foreach (var av in AcceptedValues)
			{
				if (av.IsSatisfiedBy(cand, this, ignoreCase, logger))
					return true;
			}
			return false;
		}

		public bool IsSatisfiedBy([NotNullWhen(true)] object? candiatateValue, ILogger? logger = null)
		{
			return IsSatisfiedBy(candiatateValue, false, logger);
		}
		public bool IsSatisfiedIgnoringCaseBy([NotNullWhen(true)] object? candiatateValue, ILogger? logger = null)
		{
			return IsSatisfiedBy(candiatateValue, true, logger);
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
			AcceptedValues = new List<IValueConstraint>
			{
				new ExactConstraint(value)
			};
			BaseType = TypeName.String;
		}


		public ValueConstraint(TypeName value)
		{
			BaseType = value;
			AcceptedValues = new List<IValueConstraint>();
		}

		public ValueConstraint(TypeName valueType, string value)
		{
			AcceptedValues = new List<IValueConstraint>
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
			AcceptedValues = new List<IValueConstraint>
			{
				new ExactConstraint(value.ToString())
			};
			BaseType = TypeName.Integer;
		}

		/// <summary>
		/// Initializes a constraint on decimal exact value, and decimal <see cref="BaseType"/>
		/// </summary>
		/// <param name="value">The value to set as exact decimal constraint</param>
		public ValueConstraint(decimal value)
		{
			AcceptedValues = new List<IValueConstraint>
			{
				new ExactConstraint(value.ToString())
			};
			BaseType = TypeName.Decimal;
		}

		/// <summary>
		/// Initializes a constraint on double exact value, and double <see cref="BaseType"/>
		/// </summary>
		/// <param name="value">The value to set as exact double constraint</param>
		public ValueConstraint(double value)
		{
			AcceptedValues = new List<IValueConstraint>
			{
				new ExactConstraint(value.ToString())
			};
			BaseType = TypeName.Double;
		}

		public TypeName BaseType { get; set; }

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
			return BaseType == TypeName.Undefined
				&&
				(AcceptedValues == null || !AcceptedValues.Any());
		}
		
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
				var comp = new Helpers.MultiSetComparer<IValueConstraint>();
				if (!comp.Equals(AcceptedValues, other.AcceptedValues))
					return false;
			}
			return true;
		}

		public static Type? ResolvedType(TypeName Name)
		{
            return Name switch
            {
                TypeName.Floating => typeof(float),
                TypeName.Double => typeof(double),
                TypeName.Integer => typeof(long),
                TypeName.Decimal => typeof(decimal),
                TypeName.Date or TypeName.DateTime => typeof(DateTime),
                TypeName.Time or TypeName.Duration => typeof(TimeSpan),
                TypeName.String => typeof(string),
                TypeName.Boolean => typeof(bool),
                TypeName.Uri => typeof(Uri),
                TypeName.Undefined => null,
                _ => typeof(string),
            };
        }

		public static object? GetDefault(TypeName tName, ILogger? logger = null)
		{
			if (tName == TypeName.String)
				return "";
			if (tName == TypeName.Uri)
				return new Uri(".", UriKind.Relative);
			var newT = GetNetType(tName);
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
				logger?.LogWarning("Default value for {0} provided as null for activator failure.", tName);
				return null;
			}
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as ValueConstraint);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override string ToString()
		{
			if (AcceptedValues == null || !AcceptedValues.Any())
				return $"{BaseType}";
			var joined = string.Join(",", AcceptedValues.Select(x => x.ToString()).ToArray());
			return $"{BaseType}:{joined}";
		}

		/// <summary>
		/// Produces a short text description of the constraint
		/// </summary>
		/// <returns>A description string</returns>
		public string Short()
		{
			if (IsSingleUndefinedExact(out var exact))
			{
				return $"of value '{exact}'";
			}
			var ret = new List<string>();
			if (BaseType != TypeName.Undefined)
				ret.Add($"of type {BaseType.ToString().ToLowerInvariant()}");
			else
				ret.Add($"of any type");
			if (AcceptedValues != null && AcceptedValues.Any())
			{
				ret.Add($"valid if value {string.Join (" or ", AcceptedValues.Select(x=>x.Short()).ToArray())}");
			}
			return string.Join(", ", ret);
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
			if (BaseType != TypeName.Undefined || AcceptedValues == null || AcceptedValues.Count != 1)
			{
				exact = null;
				return false;
			}
            if (AcceptedValues.FirstOrDefault() is not ExactConstraint unique)
            {
                exact = null;
                return false;
            }
            exact = unique.Value.ToString();
			return true;
		}

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
		/// Checks that there'a single exactConstraint in the accepted values and its value is of <see cref="RequiredType"/>, and provides it for consumption 
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
			var vbt = GetObject(val, BaseType);
			if (vbt is RequiredType exactAs)
			{
				exact = exactAs;
				return true;
			}
			return false;
		}


		public static implicit operator ValueConstraint(string singleUndefinedExact) => SingleUndefinedExact(singleUndefinedExact);

		public static ValueConstraint SingleUndefinedExact(string content)
		{
			var ret = new ValueConstraint()
			{
				BaseType = TypeName.Undefined,
				AcceptedValues = new List<IValueConstraint>() { new ExactConstraint(content) }
			};
			return ret;
		}
	}
}
