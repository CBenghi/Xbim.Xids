using System;
using System.Collections.Generic;
using System.Linq;

namespace Xbim.Xids
{
	public partial class ValueConstraint : IEquatable<ValueConstraint>
	{
		public ValueConstraint() {}

		public List<IValueConstraint> AcceptedValues { get; set; }

		public ValueConstraint(string value)
		{
			AcceptedValues = new List<IValueConstraint>();
			AcceptedValues.Add(new ExactConstraint(value));
			BaseType = TypeName.String;
		}

		public bool IsSatisfiedBy(object candiatateValue)
		{
			if (AcceptedValues == null)
				return false;
			var cand = GetObject(candiatateValue, BaseType);
			foreach (var av in AcceptedValues)
			{
				if (av.IsSatisfiedBy(cand, this))
					return true;
			}
			return false;
		}

		public ValueConstraint(TypeName value)
		{
			BaseType = value;
		}

		public ValueConstraint(int value)
		{
			AcceptedValues = new List<IValueConstraint>();
			AcceptedValues.Add(new ExactConstraint(value.ToString()));
			BaseType = TypeName.Integer;
		}

		public ValueConstraint(double value)
		{
			AcceptedValues = new List<IValueConstraint>();
			// G17 preserves the entire precision of double
			// see https://stackoverflow.com/questions/42083822/how-to-convert-the-double-value-to-string-without-losing-the-data-in-c-sharp
			AcceptedValues.Add(new ExactConstraint(value.ToString("G17")));
			BaseType = TypeName.Double;
		}

		public TypeName BaseType { get; set; }

		public bool IsEmpty()
		{
			return BaseType == TypeName.Undefined
				&&
				(AcceptedValues == null || !AcceptedValues.Any());
		}
		
		public bool Equals(ValueConstraint other)
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
				if (acceptableValue.IsSatisfiedBy(testObject, this))
					return true;
			}
			return false;
		}

		public static object GetDefault(TypeName tName)
		{
			if (tName == TypeName.String)
				return "";
			if (tName == TypeName.Uri)
				return new Uri(".", UriKind.Relative);
			var newT = GetNetType(tName);
			if (newT == typeof(string))
				return "";
			try
			{
				return Activator.CreateInstance(newT);
			}
			catch
			{
				return null;
			}
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ValueConstraint);
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

		public bool IsSingleExact(out object exact)
		{
			if (AcceptedValues == null || AcceptedValues.Count != 1)
			{
				exact = null;
				return false;
			}
			var unique = AcceptedValues.FirstOrDefault() as ExactConstraint;
			if (unique == null)
			{
				exact = null;
				return false;
			}
			exact = unique.Value;
			return true;
		}

		public static ValueConstraint SingleUndefinedExact(string content)
		{
			ValueConstraint ret = new ValueConstraint()
			{
				BaseType = TypeName.Undefined,
				AcceptedValues = new List<IValueConstraint>() { new ExactConstraint(content) }
			};
			return ret;
		}
	}
}
