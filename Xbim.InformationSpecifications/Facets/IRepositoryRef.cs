using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.InformationSpecifications
{
	interface IRepositoryRef
	{
		void SetIds(Xids unpersisted);

		IEnumerable<FacetGroup> UsedGroups();
	}
}
