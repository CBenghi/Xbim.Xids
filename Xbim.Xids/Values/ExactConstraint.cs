using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.Xids
{
	public class ExactConstraint : IValueConstraint
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
	}
}
