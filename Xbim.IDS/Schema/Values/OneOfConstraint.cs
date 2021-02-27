using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.IDS
{
	public class OneOfConstraint : IValueConstraint
	{
		private List<IValueConstraint> constraints;

		public bool IsValid(object testObject)
		{
			foreach (var item in constraints)
			{
				if (item.IsValid(testObject))
					return true;
			}
			return false;
		}

		internal void AddOption(IValueConstraint tVal)
		{
			if (tVal != null)
			{
				if (constraints == null)
					constraints = new List<IValueConstraint>();
				constraints.Add(tVal);
			}
		}
	}
}
