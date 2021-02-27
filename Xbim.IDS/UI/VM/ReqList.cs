using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.IDS.UI.VM
{
	class ReqViewModel
	{
		private Requirement requirement;

		public string ModelPart
		{
			get
			{
				return requirement.ModelSubset.Short();
			}
		}
			

	}
}
