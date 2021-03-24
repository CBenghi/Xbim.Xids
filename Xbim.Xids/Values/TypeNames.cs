using System;

namespace Xbim.Xids
{
	// todo: 2021: evaluate more XSD types?
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
		Uri,
	}

	public partial class Xids
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
				case TypeName.Uri:
					return "xs:anyURI";
			}
			return "";
		}

		private static Type GetNetType(string tval)
		{
			if (tval == "xs:string")
				return typeof(string);
			else if (tval == "xs:integer")
				return typeof(int);
			else if (tval == "xs:boolean")
				return typeof(bool);
			else if (tval == "xs:double")
				return typeof(double);
			else if (tval == "xs:decimal")
				return typeof(decimal);
			else if (tval == "xs:float")
				return typeof(float);
			else if (tval == "xs:date")
				return typeof(DateTime);
			else if (tval == "xs:time")
				return typeof(DateTime);
			else if (tval == "xs:anyURI")
				return typeof(string);
			return null;
		}



		private static TypeName GetNamedType(string tval)
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
			else if (tval == "xs:time")
				return TypeName.Time;
			else if (tval == "xs:anyURI")
				return  TypeName.Uri;
			return TypeName.Undefined;
		}
	}
}
