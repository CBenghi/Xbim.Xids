using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.Xids
{

	public class Value : IEquatable<Value>
	{
		public Value() {}

		public List<IValueConstraint> AcceptedValues { get; set; }

		public Value(string value)
		{
			AcceptedValues = new List<IValueConstraint>();
			AcceptedValues.Add(new ExactConstraint(value));
			BaseType = TypeName.String;
		}

		public Value(TypeName value)
		{
			BaseType = value;
		}

		public Value(int value)
		{
			AcceptedValues = new List<IValueConstraint>();
			AcceptedValues.Add(new ExactConstraint(value));
			BaseType = TypeName.Integer;
		}

		public TypeName BaseType { get; set; }
		
		public bool Equals(Value other)
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
				if (!comp.Equals(this.AcceptedValues, other.AcceptedValues))
					return false;
			}
			return true;
		}

		public Type ResolvedType()
		{
			switch (BaseType)
			{
				case TypeName.Floating:
					return typeof(float);
				case TypeName.Double:
					return typeof(double);
				case TypeName.Integer:
					return typeof(int);
				case TypeName.Decimal:
					return typeof(decimal);
				case TypeName.Date:
					return typeof(DateTime);
				case TypeName.Time:
					return typeof(TimeSpan);
				case TypeName.String:
					return typeof(string);
				case TypeName.Boolean:
					return typeof(bool);
				case TypeName.Uri:
					return typeof(Uri);
				default:
					return typeof(string);
			}
		}

		public static object GetObject(string value, TypeName t)
		{
			if (t ==  TypeName.String )
				return value;
			if (t == TypeName.Integer)
			{
				if (int.TryParse(value, out var val))
					return val;
				return null;
			}
			if (t == TypeName.Decimal)
			{
				if (decimal.TryParse(value, out var val))
					return val;
				return null;
			}
			if (t == TypeName.Double)
			{
				if (double.TryParse(value, out var val))
					return val;
				return null;
			}
			if (t == TypeName.Floating)
			{
				if (float.TryParse(value, out var val))
					return val;
				return null;
			}
			if (t == TypeName.Date)
			{
				if (DateTime.TryParse(value, out var val))
					return val.Date;
				return null;
			}
			if (t == TypeName.Boolean)
			{
				if (bool.TryParse(value, out var val))
					return val;
				return null;
			}
			if (t == TypeName.Time)
			{
				if (DateTime.TryParse(value, out var val))
					return val.TimeOfDay;
				return null;
			}
			if (t == TypeName.Uri)
			{
				if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var val))
					return val;
				return null;
			}
			return value;
		}

		public static TypeName Resolve(Type t)
		{
			if (t == typeof(string))
				return TypeName.String;
			if (t == typeof(int))
				return TypeName.Integer;
			if (t == typeof(double) || t == typeof(float))
				return TypeName.Floating;
			return TypeName.Undefined;
		}

		public bool IsValid(object testObject)
		{
			if (BaseType != TypeName.Undefined && !ResolvedType().IsAssignableFrom(testObject.GetType()))
				return false;
			foreach (var acceptableValue in AcceptedValues)
			{
				if (acceptableValue.IsSatisfiedBy(testObject))
					return true;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as Value);
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

		public bool IsSingleUndefinedExact(out string exact)
		{
			if (BaseType != TypeName.Undefined || AcceptedValues == null || AcceptedValues.Count != 1)
			{
				exact = "";
				return false;
			}
			var unique = AcceptedValues.FirstOrDefault() as ExactConstraint;
			if (unique == null)
			{
				exact = "";
				return false;
			}
			exact = unique.Value.ToString();
			return true;
		}

		public static Value SingleUndefinedExact(string content)
		{
			Value ret = new Value()
			{
				BaseType = TypeName.Undefined,
				AcceptedValues = new List<IValueConstraint>() { new ExactConstraint(content) }
			};
			return ret;
		}
	}
}
