﻿using System;
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
			switch (baseType)
			{
				case TypeName.Integer:
					return "xs:integer";
				case TypeName.String:
					return "xs:string";
				case TypeName.Boolean:
					return "xs:boolean";
				case TypeName.Floating:
					return "xs:float";
				case TypeName.Double:
					return "xs:double";
				case TypeName.Decimal:
					return "xs:decimal";
				case TypeName.Date:
					return "xs:date";
				case TypeName.Time:
					return "xs:time";
				case TypeName.Duration:
					return "xs:duration";
				case TypeName.DateTime:
					return "xs:dateTime";
				case TypeName.Uri:
					return "xs:anyURI";
			}
			return "";
		}

		public static Type GetNetType(TypeName baseType)
		{
			switch (baseType)
			{
				case TypeName.Integer:
					return typeof(int);
				case TypeName.String:
					return typeof(string);
				case TypeName.Boolean:
					return typeof(bool);
				case TypeName.Floating:
					return typeof(float);
				case TypeName.Double:
					return typeof(double);
				case TypeName.Decimal:
					return typeof(decimal);
				case TypeName.Date:
				case TypeName.DateTime:
					return typeof(DateTime);
				case TypeName.Time:
				case TypeName.Duration:
					return typeof(TimeSpan);
				case TypeName.Uri:
					return typeof(Uri);
				case TypeName.Undefined:
					return null;
				default:
					return null;
			}
		}



		public static TypeName GetNamedTypeFromXsd(string tval)
		{
			if (tval == "xs:string")
				return TypeName.String;
			else if (tval == "xs:integer")
				return TypeName.Integer;
			else if (tval == "xs:boolean")
				return TypeName.Boolean;
			else if (tval == "xs:double")
				return TypeName.Double;
			else if (tval == "xs:decimal")
				return TypeName.Decimal;
			else if (tval == "xs:float")
				return TypeName.Floating;
			else if (tval == "xs:date")
				return TypeName.Date;
			else if (tval == "xs:dateTime")
				return TypeName.DateTime;
			else if (tval == "xs:duration")
				return TypeName.Duration;
			else if (tval == "xs:time")
				return TypeName.Time;
			else if (tval == "xs:anyURI")
				return  TypeName.Uri;
			return TypeName.Undefined;
		}

		public enum constraints
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

		public static object GetObject(object value, TypeName t)
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
			return value;
		}

		public void Add(IValueConstraint newConstraint)
		{
			AcceptedValues = AcceptedValues ?? new List<IValueConstraint>();
			AcceptedValues.Add(newConstraint);
		}

		public static object GetObject(string value, TypeName t)
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
		public static IEnumerable<constraints> CompatibleConstraints(TypeName withType)
		{
			switch (withType)
			{
				case TypeName.String:
					return new[] { constraints.length, constraints.minLength, constraints.maxLength, constraints.pattern, constraints.enumeration, constraints.whiteSpace };
				case TypeName.Boolean:
					return new[] { constraints.pattern, constraints.whiteSpace };
				case TypeName.Decimal:
				case TypeName.Integer:
					return new[] { constraints.totalDigits, constraints.fractionDigits, constraints.pattern, constraints.whiteSpace, constraints.enumeration, constraints.maxInclusive, constraints.maxExclusive, constraints.minInclusive, constraints.minExclusive };
				case TypeName.Floating:
				case TypeName.Double:
				case TypeName.Duration:
				case TypeName.DateTime:
				case TypeName.Time:
				case TypeName.Uri:
				default:
					return new[] { constraints.pattern, constraints.enumeration, constraints.whiteSpace, constraints.maxInclusive, constraints.maxExclusive, constraints.minInclusive, constraints.minExclusive };
			}
		}

		public void AddAccepted(IValueConstraint constraint)
		{
			AcceptedValues = AcceptedValues ?? new List<IValueConstraint>();
			AcceptedValues.Add(constraint);
		}
	}
}