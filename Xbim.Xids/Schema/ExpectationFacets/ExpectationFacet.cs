using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Xids.Helpers;

namespace Xbim.Xids
{
	abstract public class ExpectationFacet : IEquatable<ExpectationFacet>
	{
		public Uri Uri { get; set; }
		// public Guid Guid { get; set; }

		public bool Equals(ExpectationFacet other)
		{
			if (other == null)
				return false;
			if (Uri != other.Uri)
				return false;
			// we are ignoring guid if they are not set for both.
			//if (Guid != Guid.Empty && other.Guid != Guid.Empty)
			//{
			//	if (!Guid.Equals(other.Guid))
			//		return false;
			//}
			return true;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ExpectationFacet);
		}

		public override int GetHashCode()
		{
			return $"{Uri}".GetHashCode();
		}

		public abstract bool Validate();

		public abstract string Short();
	}

	public static class ExpectationExtensions
	{
		public static bool FilterMatch(this IEnumerable<ExpectationFacet> one, IEnumerable<ExpectationFacet> other)
		{
			var comp = new MultiSetComparer<ExpectationFacet>();
			return comp.Equals(one, other);
		}
	}
}
