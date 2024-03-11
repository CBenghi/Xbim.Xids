using System;
using System.Collections.Generic;
using System.Globalization;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications
{
    // to evaluate more XSD types see 
    // see https://www.w3.org/TR/xmlschema-2/#built-in-primitive-datatypes

    /// <summary>
    /// type names in the .NET framework
    /// </summary>
    public enum NetTypeName
    {
        /// <summary>
        /// No type constraint
        /// </summary>
		Undefined,
        /// <summary>
        /// Boolean values as defined in  the .NET framework
        /// </summary>
		Boolean,
        /// <summary>
        /// String values as defined in  the .NET framework
        /// </summary>
		String,
        /// <summary>
        /// Integer values as defined in  the .NET framework
        /// </summary>
		Integer,
        /// <summary>
        /// Floating values as defined in  the .NET framework
        /// </summary>
		Floating,
        /// <summary>
        /// Double values as defined in  the .NET framework
        /// </summary>
		Double,
        /// <summary>
        /// Boolean values as defined in  the .NET framework
        /// </summary>
		Decimal,
        /// <summary>
        /// Date values as defined in  the .NET framework
        /// </summary>
		Date,
        /// <summary>
        /// Time values as defined in  the .NET framework
        /// </summary>
		Time,
        /// <summary>
        /// DateTime values as defined in  the .NET framework
        /// </summary>
		DateTime,
        /// <summary>
        /// Duration values as defined in  the .NET framework
        /// </summary>
		Duration,
        /// <summary>
        /// Uri values as defined in  the .NET framework
        /// </summary>
		Uri,
    }


    public partial class ValueConstraint
    {
        /// <summary>
        /// gets the relevant XSD type string from a C# type enum
        /// </summary>
        /// <param name="type">.NET type enum sought</param>
        /// <returns>mapped xs type name, or empty string is not found.</returns>
		public static string GetXsdTypeString(NetTypeName type)
        {
            return type switch
            {
                NetTypeName.Integer => "xs:integer",
                NetTypeName.String => "xs:string",
                NetTypeName.Boolean => "xs:boolean",
                NetTypeName.Floating => "xs:float",
                NetTypeName.Double => "xs:double",
                NetTypeName.Decimal => "xs:decimal",
                NetTypeName.Date => "xs:date",
                NetTypeName.Time => "xs:time",
                NetTypeName.Duration => "xs:duration",
                NetTypeName.DateTime => "xs:dateTime",
                NetTypeName.Uri => "xs:anyURI",
                _ => "",
            };
        }

        /// <summary>
        /// Create a constraint pattern from a string.
        /// </summary>
        /// <param name="pattern">The regex value that needs to be matched.</param>
        /// <returns>A value constraint, based on string type</returns>
        public static ValueConstraint CreatePattern(string pattern)
        {
            var vc = new ValueConstraint(NetTypeName.String);
            vc.AddAccepted(new PatternConstraint() { Pattern = pattern });
            return vc;
        }

        /// <summary>
        /// Gets the .NET type from the enum value
        /// </summary>
        /// <returns>null if not found</returns>
        public static Type? GetNetType(NetTypeName baseType)
        {
            return baseType switch
            {
                NetTypeName.Integer => typeof(int),
                NetTypeName.String => typeof(string),
                NetTypeName.Boolean => typeof(bool),
                NetTypeName.Floating => typeof(float),
                NetTypeName.Double => typeof(double),
                NetTypeName.Decimal => typeof(decimal),
                NetTypeName.Date or NetTypeName.DateTime => typeof(DateTime),
                NetTypeName.Time or NetTypeName.Duration => typeof(TimeSpan),
                NetTypeName.Uri => typeof(Uri),
                NetTypeName.Undefined => null,
                _ => null,
            };
        }


        /// <summary>
        /// Gets the .NET type enum from the XSD string value
        /// </summary>
        /// <returns>Undefined if not found</returns>
		public static NetTypeName GetNamedTypeFromXsd(string tval)
        {
            return tval switch
            {
                "xs:string" => NetTypeName.String,
                "xs:integer" => NetTypeName.Integer,
                "xs:boolean" => NetTypeName.Boolean,
                "xs:double" => NetTypeName.Double,
                "xs:decimal" => NetTypeName.Decimal,
                "xs:float" => NetTypeName.Floating,
                "xs:date" => NetTypeName.Date,
                "xs:dateTime" => NetTypeName.DateTime,
                "xs:duration" => NetTypeName.Duration,
                "xs:time" => NetTypeName.Time,
                "xs:anyURI" => NetTypeName.Uri,
                _ => NetTypeName.Undefined,
            };
        }

        /// <summary>
        /// Enumeration of all possible XSD constraint types
        /// </summary>
		public enum Constraints
        {
            /// <summary>
            /// Length of an element in characters or digits.
            /// </summary>
			length,
            /// <summary>
            /// Minumum Length of an element in characters or digits
            /// </summary>
			minLength,
            /// <summary>
            /// Maximum Length of an element in characters or digits
            /// </summary>
			maxLength,
            /// <summary>
            /// pattern of a an element (expressed as a string)
            /// </summary>
			pattern,
            /// <summary>
            /// One of the values in the enumeration
            /// </summary>
			enumeration,
            /// <summary>
            /// How do we treat whitespace? TODO: to be documented.
            /// </summary>
			whiteSpace,
            /// <summary>
            /// number of digits defined in a decimal value
            /// </summary>
			totalDigits,
            /// <summary>
            /// number of digits after the unit, in a decimal value
            /// </summary>
			fractionDigits,
            /// <summary>
            /// the minimum boundary of an element, not valid for the interval
            /// </summary>
			minExclusive,
            /// <summary>
            /// the minimum boundary of an element, valid for the interval
            /// </summary>
			minInclusive,
            /// <summary>
            /// the maximum boundary of an element, not valid for the interval
            /// </summary>
			maxExclusive,
            /// <summary>
            /// the minimum boundary of an element, valid for the interval
            /// </summary>
			maxInclusive,
        }

        /// <summary>
        /// Converts the passed <paramref name="value"/> to the provided <paramref name="typeName"/>
        /// </summary>
        /// <param name="value">the value to try to convert</param>
        /// <param name="typeName">the destination type</param>
        /// <returns>Null when conversion is not successful</returns>
        [Obsolete("Prefer ConvertObject()")]
        public static object? GetObject(object value, NetTypeName typeName)
        {
            return ConvertObject(value, typeName);
        }

        /// <summary>
        /// Converts the passed <paramref name="value"/> to the provided <paramref name="typeName"/>
        /// </summary>
        /// <param name="value">the value to try to convert</param>
        /// <param name="typeName">the destination type</param>
        /// <returns>Null when conversion is not successful</returns>
		public static object? ConvertObject(object value, NetTypeName typeName)
        {
            switch (typeName) // if undefined type just return the value, unaltered.
            {
                case NetTypeName.Undefined:
                    return value;
                case NetTypeName.Integer:
                    return Convert.ToInt32(value, CultureHelper.SystemCulture);
                case NetTypeName.Decimal:
                    return Convert.ToDecimal(value, CultureHelper.SystemCulture);
                case NetTypeName.Double:
                    return Convert.ToDouble(value, CultureHelper.SystemCulture);
                case NetTypeName.Floating:
                    return Convert.ToSingle(value, CultureHelper.SystemCulture);
                case NetTypeName.Date:
                case NetTypeName.DateTime:
                    return Convert.ToDateTime(value, CultureHelper.SystemCulture);
                case NetTypeName.Boolean:
                    return Convert.ToBoolean(value);
                case NetTypeName.Time:
                    {
                        var tmp = Convert.ToDateTime(value, CultureHelper.SystemCulture);
                        return tmp.TimeOfDay;
                    }

                case NetTypeName.Uri:
                    {
                        if (Uri.TryCreate(value.ToString(), UriKind.RelativeOrAbsolute, out var val))
                            return val;
                        return null;
                    }
                case NetTypeName.Duration:
                    if (TimeSpan.TryParse(value.ToString(), CultureInfo.InvariantCulture, out var duration))
                        return duration;
                    return null;

                case NetTypeName.String:
                    return value.ToString();
                
                default:
                    return value;
            }
        }

        /// <summary>
        /// Centralised value casting function.
        /// </summary>
        /// <param name="value">The value to cast</param>
        /// <param name="castingObjectForType">An object used to determine the casting</param>
        /// <returns>a cast object according to the type of <paramref name="castingObjectForType"/></returns>
        [Obsolete("Prefer ParseValue()")]
        public static object? CastObject(string value, object castingObjectForType)
        {
            return ParseValue(value, castingObjectForType);
        }

        /// <summary>
        /// Parse the constraint value into a form suitable for the target type
        /// </summary>
        /// <param name="value">The value to cast</param>
        /// <param name="targetType">An object used to determine the type to parse to</param>
        /// <returns>a cast object according to the type of <paramref name="targetType"/></returns>
        public static object? ParseValue(string value, object targetType)
        {
            return targetType switch
            {
                double => ParseValue(value, NetTypeName.Double),
                float => ParseValue(value, NetTypeName.Floating),
                decimal => ParseValue(value, NetTypeName.Decimal),
                short => ParseValue(value, NetTypeName.Integer),
                int => ParseValue(value, NetTypeName.Integer),
                long => ParseValue(value, NetTypeName.Integer),
                DateTime => ParseValue(value, NetTypeName.DateTime),
                TimeSpan => ParseValue(value, NetTypeName.Duration),
                _ => value,
            };
        }

        /// <summary>
        /// Attempt to Parse the passed string <paramref name="value"/> to the provided <paramref name="typeName"/>
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <param name="typeName">The destination type required</param>
        /// <returns>Null if parsing or conversion are not succesful, a nullable string in case of undefined type</returns>
        [Obsolete("Prefer ParseValue()")]
        public static object? GetObject(string? value, NetTypeName typeName)
        {
            return ParseValue(value, typeName);
        }

        /// <summary>
        /// Attempt to Parse the passed string <paramref name="value"/> to the provided <paramref name="typeName"/>
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <param name="typeName">The destination type required</param>
        /// <returns>Null if parsing or conversion are not succesful, a nullable string in case of undefined type</returns>
		public static object? ParseValue(string? value, NetTypeName typeName)
        {
            var culture = CultureHelper.SystemCulture;
            if (string.IsNullOrEmpty(value))
                return null;
            switch (typeName)
            {
                case NetTypeName.Undefined:
                    return value;
                case NetTypeName.Boolean:
                    if (bool.TryParse(value, out var boolval))
                        return boolval;
                    return null;
                case NetTypeName.String:
                    return value;
                case NetTypeName.Integer:
                    // TODO:Review handling of longs
                    if (int.TryParse(value, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, culture, out var ival))
                        return ival;
                    return null;
                case NetTypeName.Floating:
                    if (float.TryParse(value, NumberStyles.Float, culture, out var fval))
                        return fval;
                    return null;
                case NetTypeName.Double:
                    if (double.TryParse(value, NumberStyles.Float, culture, out var dblval))
                        return dblval;
                    return null;
                case NetTypeName.Decimal:
                    if (decimal.TryParse(value, NumberStyles.Number, culture, out var dval))
                        return dval;
                    return null;
                case NetTypeName.Date:
                case NetTypeName.DateTime:
                    if (DateTime.TryParse(value, culture, DateTimeStyles.AssumeUniversal, out var dateval))
                        return dateval.Date;
                    return null;
                case NetTypeName.Time:
                case NetTypeName.Duration:
                    if (TimeSpan.TryParse(value, culture, out var timeval))
                        return timeval;
                    return null;
                case NetTypeName.Uri:
                    if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var urival))
                        return urival;
                    return null;
                default:
                    return value;
            }
        }


        /// <summary>
        /// returns a set of the valid constraint types given a base types 
        /// Documentation taken from:
        /// https://www.w3.org/TR/2004/REC-xmlschema-2-20041028/datatypes.html#string
        /// </summary>
        /// <param name="withType">the base type for the application of the constraints</param>
        /// <returns>An enumeration of constraints.</returns>
        public static IEnumerable<Constraints> CompatibleConstraints(NetTypeName withType)
        {
            return withType switch
            {
                NetTypeName.String => new[] { Constraints.length, Constraints.minLength, Constraints.maxLength, Constraints.pattern, Constraints.enumeration, Constraints.whiteSpace },
                NetTypeName.Boolean => new[] { Constraints.pattern, Constraints.whiteSpace },
                NetTypeName.Decimal or NetTypeName.Integer => new[] { Constraints.totalDigits, Constraints.fractionDigits, Constraints.pattern, Constraints.whiteSpace, Constraints.enumeration, Constraints.maxInclusive, Constraints.maxExclusive, Constraints.minInclusive, Constraints.minExclusive },
                _ => new[] { Constraints.pattern, Constraints.enumeration, Constraints.whiteSpace, Constraints.maxInclusive, Constraints.maxExclusive, Constraints.minInclusive, Constraints.minExclusive },
            };
        }

    }
}
