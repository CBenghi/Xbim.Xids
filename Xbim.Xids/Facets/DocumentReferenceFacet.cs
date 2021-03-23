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
			if (other == null)
				return false;
			if (DocumentName.ToLowerInvariant() != other.DocumentName.ToLowerInvariant())
				return false;
			if (DocumentStatus.ToLowerInvariant() != other.DocumentStatus.ToLowerInvariant())
				return false;
			if (RequiredAttributes.ToLowerInvariant() != other.RequiredAttributes.ToLowerInvariant())
				return false;
			return true;
		}
		public override bool Equals(object obj)
		{
			return this.Equals(obj as DocumentReferenceFacet);
		}

		public override string ToString()
		{
			return $"{DocumentName}-{DocumentStatus}-{RequiredAttributes}";
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public string Short()
		{
			throw new NotImplementedException();
		}
	}
}