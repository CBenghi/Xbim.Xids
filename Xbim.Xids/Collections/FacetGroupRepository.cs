using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Xbim.Xids
{
	public class FacetGroupRepository
	{
		private Xids ids;

		[Obsolete("Use only for persistence and testing, otherwise prefer other constructors")]
		[JsonConstructor]
		public FacetGroupRepository()
		{

		}

		public FacetGroupRepository(Xids ids)
		{
			this.ids = ids;
		}

		[JsonIgnore]
		public int Count => Collection.Count;

		public List<FacetGroup> Collection { get; set; } = new List<FacetGroup>();

		internal void Add(FacetGroup group)
		{
			Collection.Add(group);
		}

		internal FacetGroup FirstOrDefault(Func<FacetGroup, bool> p)
		{
			return Collection.FirstOrDefault(p);
		}
	}
}
