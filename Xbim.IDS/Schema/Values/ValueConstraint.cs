using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.IDS
{
	public class ValueConstraint : IValueConstraint
	{
		public ValueConstraint(string value)
		{
			ExactValue = value;
			BaseType = typeof(string);
		}

		public ValueConstraint(int value)
		{
			ExactValue = value;
			BaseType = typeof(int);
		}

		public object ExactValue { get; set; }

		public Type BaseType { get; set; }

		public bool IsValid(object testObject)
		{
			if (BaseType != null && !BaseType.IsAssignableFrom(testObject.GetType()))
				return false;
			if (ExactValue != null)
				return testObject.Equals(ExactValue);
			return true;
		}
	}
}
