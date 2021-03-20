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

		public int Count => _parts.Count;

		internal void Add(ModelPart modelPart)
		{
			_parts.Add(modelPart);
		}

		internal ModelPart FirstOrDefault(Func<ModelPart, bool> p) => _parts.FirstOrDefault(p);
	}
}
