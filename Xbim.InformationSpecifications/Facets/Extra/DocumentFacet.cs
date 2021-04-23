using System;

namespace Xbim.InformationSpecifications
{

	// IfcRelAssociatesDocument referenes the IfcDocumentSelect, which can be:
	// -> IfcDocumentInformation
	//     IFC2X3 IFC4
	//     DocumentId -> Identification ** Change
	//     Name 
	//     Description 
	//     DocumentReferences -> Location ** Change
	//     Purpose 
	//     IntendedUse 
	//     Scope 
	//     Revision 
	//     DocumentOwner 
	//     Editors 
	//     CreationTime 
	//     LastRevisionTime 
	//     ElectronicFormat 
	//     ValidFrom 
	//     ValidUntil 
	//     Confidentiality 
	//     Status 

	// 
	// -> IfcDocumentReference
	//     IFC2X3 IFC4
	//	   Location			MODIFIED	** Change
	//	   ItemReference -> Identification		** Change
	//	   () -> Description ADDED	
	//	   () -> ReferencedDocument ADDED	


	public partial class DocumentFacet : FacetBase, IFacet, IEquatable<DocumentFacet>
	{
		public string DocumentName { get; set; } = "";

		public string DocumentStatus { get; set; } = "";

		public string RequiredAttributes { get; set; } = "";

		public bool Equals(DocumentFacet other)
		{
			if (other == null)
				return false;
			var thisEquaal = (DocumentName, DocumentStatus, RequiredAttributes)
				.Equals((other.DocumentName, other.DocumentStatus, other.RequiredAttributes));
			if (!thisEquaal)
				return false;
			return base.Equals(other);
		}
		public override bool Equals(object obj)
		{
			return this.Equals(obj as DocumentFacet);
		}

		public override string ToString()
		{
			return $"{DocumentName}-{DocumentStatus}-{RequiredAttributes}-{base.ToString()}";
		}

		public override int GetHashCode() => (DocumentName, DocumentStatus, RequiredAttributes).GetHashCode();

		public string Short()
		{
			return ToString();
		}

		public bool IsValid()
		{
			return !string.IsNullOrWhiteSpace(DocumentName);
		}
	}
}