using System;

namespace Xbim.Xids
{
    public partial class DocumentReferenceFacet : IFacet, IEquatable<DocumentReferenceFacet>
	{
		public string DocumentName { get; set; } = "";

		public string DocumentStatus { get; set; } = "";

		public string RequiredAttributes { get; set; } = "";

		public bool Equals(DocumentReferenceFacet other)
		{
			return (DocumentName, DocumentStatus, RequiredAttributes)
				.Equals((other.DocumentName, other.DocumentStatus, other.RequiredAttributes));
		}
		public override bool Equals(object obj)
		{
			return this.Equals(obj as DocumentReferenceFacet);
		}

		public override string ToString()
		{
			return $"{DocumentName}-{DocumentStatus}-{RequiredAttributes}";
		}

		public override int GetHashCode() => (DocumentName, DocumentStatus, RequiredAttributes).GetHashCode();

		public string Short()
		{
			return ToString();
		}
	}
}