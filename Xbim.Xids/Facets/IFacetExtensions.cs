using System.Collections.Generic;
using System.Linq;
using Xbim.Xids.Helpers;

// todo: 2021: add quantities

namespace Xbim.Xids
{
	public static class IFacetExtensions
	{
		public static bool FilterMatch(this IEnumerable<IFacet> one, IEnumerable<IFacet> other)
		{
			var comp = new MultiSetComparer<IFacet>();
			return comp.Equals(one, other);
		}

		public static bool NullEquals(object one, object two)
		{
			if (one == null && two == null)
				return true;
			if (one == null || two == null)
				return false;
			return one.Equals(two);
		}
	}
}
