using System;
using System.Collections.Generic;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications
{
	internal static class IFacetExtensions
	{
		internal static bool FilterMatch(this IEnumerable<IFacet> one, IEnumerable<IFacet> other)
		{
			var comp = new MultiSetComparer<IFacet>();
			return comp.Equals(one, other);
		}

		internal static bool NullEquals(object? one, object? two)
		{
			if (one == null && two == null)
				return true;
			if (one == null || two == null)
				return false;
			return one.Equals(two);
		}

		internal static bool CaseInsensitiveEquals(string? one, string? two)
		{
			if (one == null && two == null)
				return true;
			if (one == null || two == null)
				return false;
			return one.ToUpperInvariant().Equals(two.ToUpperInvariant());
		}

		internal static bool CaseInsensitiveEquals(ValueConstraint? one, ValueConstraint? two)
		{
			if (one == null && two == null) // both null ok
				return true;
			if (one == null || two == null) // just one null not ok
				return false;

			// case sensitivity needed for regex, but ignored strings
			if (!one.BaseType.Equals(two.BaseType))
				return false;

			// just compare accepted values
			if (two.AcceptedValues is null && one.AcceptedValues is null)
				return true; // both null is ok
			if (one.AcceptedValues is null || two.AcceptedValues is null)
				return false; // only one null is not equal
			if (one.AcceptedValues.Count != two.AcceptedValues.Count)
				return false;
			for (int i = 0; i < one.AcceptedValues.Count; i++)
			{
				var ofOne = one.AcceptedValues[i];
				var ofTwo = two.AcceptedValues[i];
				if (ofOne == null && ofTwo == null)
					continue; // both null is ok
				if (ofOne == null || ofTwo == null)
					return false; // only one null is not equal
				if (one.BaseType == NetTypeName.String
					&& ofOne is ExactConstraint ecOfOne
					&& ofTwo is ExactConstraint ecOfTwo
					)
				{
					if (!ecOfOne.Value.Equals(ecOfTwo.Value, StringComparison.InvariantCultureIgnoreCase))
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
