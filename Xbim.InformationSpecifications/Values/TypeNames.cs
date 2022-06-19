using System;
using System.Collections.Generic;

namespace Xbim.InformationSpecifications
{
	// to evaluate more XSD types see 
	// see https://www.w3.org/TR/xmlschema-2/#built-in-primitive-datatypes

	public enum TypeName
	{
		Undefined,
		Boolean,
		String,
		Integer,
		Floating,
		Double,
		Decimal,
		Date,
		Time,
		DateTime,
		Duration,
		Uri,
	}


	public partial class ValueConstraint
	{
		public static string GetXsdTypeString(TypeName baseType)
		{
            return baseType switch
            {
                TypeName.Integer => "xs:integer",
                TypeName.String => "xs:string",
                TypeName.Boolean => "xs:boolean",
                TypeName.Floating => "xs:float",
                TypeName.Double => "xs:double",
                TypeName.Decimal => "xs:decimal",
                TypeName.Date => "xs:date",
                TypeName.Time => "xs:time",
                TypeName.Duration => "xs:duration",
                TypeName.DateTime => "xs:dateTime",
                TypeName.Uri => "xs:anyURI",
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
			var vc = new ValueConstraint(TypeName.String);
			vc.AddAccepted(new PatternConstraint() { Pattern = pattern });
			return vc;
		}

        public static Type? GetNetType(TypeName baseType)
		{
            return baseType switch
            {
                TypeName.Integer => typeof(int),
                TypeName.String => typeof(string),
                TypeName.Boolean => typeof(bool),
                TypeName.Floating => typeof(float),
                TypeName.Double => typeof(double),
                TypeName.Decimal => typeof(decimal),
                TypeName.Date or TypeName.DateTime => typeof(DateTime),
                TypeName.Time or TypeName.Duration => typeof(TimeSpan),
                TypeName.Uri => typeof(Uri),
                TypeName.Undefined => null,
                _ => null,
            };
        }



		public static TypeName GetNamedTypeFromXsd(string tval)
		{
            return tval switch
            {
                "xs:string" => TypeName.String,
                "xs:integer" => TypeName.Integer,
                "xs:boolean" => TypeName.Boolean,
                "xs:double" => TypeName.Double,
                "xs:decimal" => TypeName.Decimal,
                "xs:float" => TypeName.Floating,
                "xs:date" => TypeName.Date,
                "xs:dateTime" => TypeName.DateTime,
                "xs:duration" => TypeName.Duration,
                "xs:time" => TypeName.Time,
                "xs:anyURI" => TypeName.Uri,
                _ => TypeName.Undefined,
            };
        }

		public enum Constraints
		{
			length,
			minLength,
			maxLength,
			pattern,
			enumeration,
			whiteSpace,
			totalDigits,
			fractionDigits,
			minExclusive,
			minInclusive,
			maxExclusive,
			maxInclusive,
		}

		public static object? GetObject(object value, TypeName t)
		{
			if (t == TypeName.Integer)
				return Convert.ToInt32(value);
			if (t == TypeName.Decimal)
				return Convert.ToDecimal(value);
			if (t == TypeName.Double)
				return Convert.ToDouble(value);
			if (t == TypeName.Floating)
				return Convert.ToSingle(value);
			if (t == TypeName.Date)
				return Convert.ToDateTime(value);
			if (t == TypeName.Boolean)
				return Convert.ToBoolean(value);
			if (t == TypeName.Time)
			{
				var tmp = Convert.ToDateTime(value);
				return tmp.TimeOfDay;
			}
			if (t == TypeName.Uri)
			{
				if (Uri.TryCreate(value.ToString(), UriKind.RelativeOrAbsolute, out var val))
					return val;
				return null;
			}
			if (t == TypeName.String)
			{
				return value.ToString();
			}
			return value;
		}



		public static object? GetObject(string? value, TypeName t)
		{
			switch (t)
			{
				case TypeName.Undefined:
					return value;
				case TypeName.Boolean:
					if (bool.TryParse(value, out var boolval))
						return boolval;
					return null;
				case TypeName.String:
					return value;
				case TypeName.Integer:
					if (int.TryParse(value, out var ival))
						return ival;
					return null;
				case TypeName.Floating:
					if (float.TryParse(value, out var fval))
						return fval;
					return null;
				case TypeName.Double:
					if (double.TryParse(value, out var dblval))
						return dblval;
					return null;
				case TypeName.Decimal:
					if (decimal.TryParse(value, out var dval))
						return dval;
					return null;
				case TypeName.Date:
				case TypeName.DateTime:
					if (DateTime.TryParse(value, out var dateval))
						return dateval.Date;
					return null;
				case TypeName.Time:
				case TypeName.Duration:
					if (TimeSpan.TryParse(value, out var timeval))
						return timeval;
					return null;
				case TypeName.Uri:
					if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var urival))
						return urival;
					return null;
				default:
					return value;
			}
		}

		// documentation taken from:
		// https://www.w3.org/TR/2004/REC-xmlschema-2-20041028/datatypes.html#string
		public static IEnumerable<Constraints> CompatibleConstraints(TypeName withType)
		{
            return withType switch
            {
                TypeName.String => new[] { Constraints.length, Constraints.minLength, Constraints.maxLength, Constraints.pattern, Constraints.enumeration, Constraints.whiteSpace },
                TypeName.Boolean => new[] { Constraints.pattern, Constraints.whiteSpace },
                TypeName.Decimal or TypeName.Integer => new[] { Constraints.totalDigits, Constraints.fractionDigits, Constraints.pattern, Constraints.whiteSpace, Constraints.enumeration, Constraints.maxInclusive, Constraints.maxExclusive, Constraints.minInclusive, Constraints.minExclusive },
                _ => new[] { Constraints.pattern, Constraints.enumeration, Constraints.whiteSpace, Constraints.maxInclusive, Constraints.maxExclusive, Constraints.minInclusive, Constraints.minExclusive },
            };
        }


	}
}
