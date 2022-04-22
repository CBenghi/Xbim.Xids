using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.InformationSpecifications.NewTests
{
    public class NullableAnalysisHelperTests
    {
#nullable enable
		public static void Some()
		{
			ValueConstraint? v = null;
			// v.IsSatisfiedBy(""); // this would have a warning
			if (ValueConstraint.IsNotEmpty(v))
			{
				v.IsSatisfiedBy(""); // this has no warning
			}

		}
#nullable restore
	}
}
