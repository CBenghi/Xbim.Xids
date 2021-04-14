using System.Collections.Generic;
using System.Linq;
using Xbim.IDS.Tools;

namespace Xbim.IDS
{
	public interface IFilter
	{
		string Short();
	}

	public static class IFilterExtensions
	{
		public static bool FilterMatch(this IEnumerable<IFilter> one, IEnumerable<IFilter> other)
		{
			var comp = new MultiSetComparer<IFilter>();
			return comp.Equals(one, other);
		}
	}
}
