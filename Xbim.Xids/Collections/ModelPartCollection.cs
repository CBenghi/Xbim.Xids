using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.Xids
{
	public class ModelPartCollection
	{
		private List<ModelPart> _parts = new List<ModelPart>();
		private Xids ids;

		public ModelPartCollection(Xids ids)
		{
			this.ids = ids;
		}

		[JsonIgnore]
		public int Count => _parts.Count;

		public List<ModelPart> Parts
		{
			get
			{
				return _parts;
			}
			set
			{
				_parts = value;
			}
		}

		internal void Add(ModelPart modelPart)
		{
			_parts.Add(modelPart);
		}

		internal ModelPart FirstOrDefault(Func<ModelPart, bool> p) => _parts.FirstOrDefault(p);
	}
}
