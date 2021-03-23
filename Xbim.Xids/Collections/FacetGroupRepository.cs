using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Xids
{
	public class FacetGroupRepository
	{
		private Xids ids;
		public FacetGroupRepository(Xids ids)
		{
			this.ids = ids;
		}

		private List<FacetGroup> _collection = new List<FacetGroup>();

		[JsonIgnore]
		public int Count => _collection.Count;

		public List<FacetGroup> Collection
		{
			get => _collection;
			set
			{
				_collection = value;
			}
		}

		internal void Add(FacetGroup group)
		{
			_collection.Add(group);
		}

		internal FacetGroup FirstOrDefault(Func<FacetGroup, bool> p)
		{
			return _collection.FirstOrDefault(p);
		}
	}
}
