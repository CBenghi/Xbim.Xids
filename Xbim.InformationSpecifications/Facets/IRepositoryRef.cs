using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.InformationSpecifications
{
	/// <summary>
	/// Any facet with weak pointers to a repository implements this class to restore strong pointers and
	/// resolve relation queries.
	/// </summary>
	internal interface IRepositoryRef
	{
		/// <summary>
		/// Used to restore the contextual XIDS of the weak pointers.
		/// </summary>
		/// <param name="context"></param>
		void SetContextIds(Xids context);

		/// <summary>
		/// Returns any FacetGroups that are referenced by the current facet.
		/// </summary>
		/// <returns>The enumeration of groups</returns>
		IEnumerable<FacetGroup> UsedGroups();
	}
}
