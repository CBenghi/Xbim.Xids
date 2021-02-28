using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.IDS
{

	public class ValueConstraint : IValueConstraint, IEquatable<ValueConstraint>
	{
		public ValueConstraint() {}

		public ValueConstraint(string value)
		{
			ExactValue = value;
			BaseType = TypeNames.text;
		}

		public ValueConstraint(int value)
		{
			ExactValue = value;
			BaseType = TypeNames.integer;
		}

		public object ExactValue { get; set; }

		public TypeNames BaseType { get; set; }

		public bool Equals(ValueConstraint other)
		{
			if (other == null)
				return false;
			if (!ExactValue.Equals(other.ExactValue))
				return false;
			if (!BaseType.Equals(other.BaseType))
				return false;
			return true;
		}

		Type resolvedType()
		{
			switch (BaseType)
			{
				case TypeNames.floating:
					return typeof(double);
				case TypeNames.integer:
					return typeof(int);
				default:
					return typeof(string);
			}
		}

		public bool IsValid(object testObject)
		{
			if (BaseType != TypeNames.undefined && !resolvedType().IsAssignableFrom(testObject.GetType()))
				return false;
			if (ExactValue != null)
				return testObject.Equals(ExactValue);
			return true;
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
			return $"{BaseType}:{ExactValue}";
		}
	}
}
