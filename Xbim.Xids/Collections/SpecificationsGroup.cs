using System.Collections.Generic;

namespace Xbim.Xids
{
	public partial class SpecificationsGroup
	{
		public string Name { get; set; }

		public List<string> Stage { get; set; }

		public Stakeholder Provider { get; set; }

		public List<Stakeholder> Consumer { get; set; }

		public List<Specification> Specifications { get; set; } = new List<Specification>();
	}
}