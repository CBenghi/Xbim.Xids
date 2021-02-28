using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.IDS
{
	abstract public class ExpectationFacet
	{
		public Uri Uri { get; set; }
		public Guid Guid { get; set; }
		public abstract bool Validate();

		
	}
}
