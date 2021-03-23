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
			BaseType = TypeName.text;
		}

		public Value(TypeName value)
		{
			BaseType = value;
		}

		public Value(int value)
		{
			AcceptedValues = new List<IValueConstraint>();
			AcceptedValues.Add(new ExactConstraint(value));
			BaseType = TypeName.integer;
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

		Type ResolvedType()
		{
			switch (BaseType)
			{
				case TypeName.floating:
					return typeof(double);
				case TypeName.integer:
					return typeof(int);
				default:
					return typeof(string);
			}
		}

		public static TypeName Resolve(Type t)
		{
			if (t == typeof(string))
				return TypeName.text;
			if (t == typeof(int))
				return TypeName.integer;
			if (t == typeof(double) || t == typeof(float))
				return TypeName.floating;
			return TypeName.undefined;
		}

		public bool IsValid(object testObject)
		{
			if (BaseType != TypeName.undefined && !ResolvedType().IsAssignableFrom(testObject.GetType()))
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
			return $"{BaseType}:joined";
		}
	}
}
