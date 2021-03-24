using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.Xids
{
	public class ExactConstraint : IValueConstraint, IEquatable<ExactConstraint>
	{
		public ExactConstraint(object value)
		{
			Value = value;
		}

		public object Value { get; set; }

		public bool IsSatisfiedBy(object testObject)
		{
			return Value.Equals(testObject);
		}

		public override string ToString()
		{
			if (Value != null)
				return Value.ToString();
			return "<null>";
		}

		public override int GetHashCode()
		{
			if (Value != null)
				return Value.GetHashCode();
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ExactConstraint);
		}

		public bool Equals(ExactConstraint other)
		{
			if (other == null)
				return false;
			return (Value, true).Equals((other.Value, true));
		}
	}
}
