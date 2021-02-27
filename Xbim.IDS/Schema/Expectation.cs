using System.Collections.ObjectModel;

namespace Xbim.IDS
{
	public partial class Expectation
	{
		public ObservableCollection<IFacet> Facets { get; set; } = new ObservableCollection<IFacet>();

		public string Reference { get; set; }

		public string Guid { get; set; }

		public string Description { get; set; }
	}
}