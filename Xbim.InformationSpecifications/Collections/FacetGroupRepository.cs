using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Xbim.InformationSpecifications
{
	public class FacetGroupRepository
	{
		private readonly Xids ids;

		[Obsolete("Use only for persistence and testing, otherwise prefer other constructors")]
		[JsonConstructor]
		public FacetGroupRepository()
		{
			ids = new Xids();
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
			// just to be on the safe side, let's only add it once.
			if (Collection.Contains(group))
				return;
			Collection.Add(group);
		}

		internal FacetGroup? FirstOrDefault(Func<FacetGroup, bool> p)
		{
			return Collection.FirstOrDefault(p);
		}

		/// <summary>
		/// Creates a new FacetGroup, and associates it with the collection.
		/// </summary>
		/// <returns>A facetgroup that does not need to be added to the collection</returns>
		public FacetGroup CreateNew()
		{
			var ret = new FacetGroup(this);
			return ret;
		}
	}
}
