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

		public static bool CaseInsensitiveEquals(string one, string two)
		{
			if (one == null && two == null)
				return true;
			if (one == null || two == null)
				return false;
			return one.ToUpperInvariant().Equals(two.ToUpperInvariant());
		}
		public static bool CaseInsensitiveEquals(ValueConstraint one, ValueConstraint two)
		{
			if (one == null && two == null) // both null ok
				return true;
			if (one == null || two == null) // just one null not ok
				return false;

			// case sensitivity needed for regex, but ignored strings
			if (!one.BaseType.Equals(two.BaseType))
				return false;
			if (one.AcceptedValues.Count != two.AcceptedValues.Count)
				return false;
			for (int i = 0; i < one.AcceptedValues.Count; i++)
			{
				var ofOne = one.AcceptedValues[i];
				var ofTwo = two.AcceptedValues[i];
				if (ofOne == null && ofTwo == null)
					continue;
				if (one == null || two == null)
					return false;
				if (one.BaseType == TypeName.String
					&& ofOne is ExactConstraint ecOfOne
					&& ofTwo is ExactConstraint ecOfTwo
					)
				{
					if (ecOfOne.Value.ToUpperInvariant().Equals(ecOfTwo.Value.ToUpperInvariant()))
						return false;
				}
				else
				{
					if (!ofOne.Equals(ofTwo))
						return false;
				}
			}
			return true;
		}
	}
}
