using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Xids
{
	public class ExpectationCollection
	{
		private Xids ids;
		public ExpectationCollection(Xids ids)
		{
			this.ids = ids;
		}

		private List<Expectation> _parts = new List<Expectation>();

		[JsonIgnore]
		public int Count => _parts.Count;

		public List<Expectation> Expectations
		{
			get => _parts;
			set
			{
				_parts = value;
			}
		}

		internal void Add(Expectation expectation)
		{
			_parts.Add(expectation);
		}

		internal Expectation FirstOrDefault(Func<Expectation, bool> p)
		{
			return _parts.FirstOrDefault(p);
		}
	}
}
