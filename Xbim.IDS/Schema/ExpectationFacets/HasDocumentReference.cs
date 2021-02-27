namespace Xbim.IDS
{
    public partial class HasDocumentReference : IFacet
    {
		public string DocumentName { get; set; }

		public string DocumentStatus { get; set; }

		public string RequiredAttributes { get; set; }
	}
}