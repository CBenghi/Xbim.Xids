using System;

namespace Xbim.IDS
{
    public partial class HasDocumentReference : ExpectationFacet
    {
		public string DocumentName { get; set; }

		public string DocumentStatus { get; set; }

		public string RequiredAttributes { get; set; }
		
		public override bool Validate()
		{
			// Strictly speaking we only need DocumentName
			if (string.IsNullOrWhiteSpace(DocumentName))
				return false;
			if (Guid == Guid.Empty)
				Guid = Guid.NewGuid();
			return true;
		}
	}
}