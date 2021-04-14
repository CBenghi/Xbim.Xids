using System.Collections.Generic;
using Xbim.InformationSpecifications.Helpers;

// todo: 2021: add quantities

namespace Xbim.InformationSpecifications
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

		public static bool NullableStringCaseInsensitiveEquals(string one, string two)
		{
			if (one == null && two == null)
				return true;
			if (one == null || two == null)
				return false;
			return one.ToUpperInvariant().Equals(two.ToUpperInvariant());
		}
	}
}
