using System;
using System.Diagnostics.CodeAnalysis;

namespace Xbim.InformationSpecifications
{
	// IfcRelAssociatesDocument references the IfcDocumentSelect, which can be:
	// -> IfcDocumentInformation
	// 	
	//     IFC2X3             | IFC4                                              | Notes
	//     ------------------ | ------------------------------------------------  | ----------
	//     DocumentId         | Identification ** Change (type: IfcIdentifier)    | Required
	//     Name               | (type: IfcLabel)                                  | Required
	//     Description 
	//     DocumentReferences | Location  (Change) (type: IfcURIReference)
	//     Purpose            | (type: IfcText)
	//     IntendedUse        | (type: IfcText)
	//     Scope 
	//     Revision 
	//     DocumentOwner 
	//     Editors 
	//     CreationTime 
	//     LastRevisionTime 
	//     ElectronicFormat 
	//     ValidFrom 
	//     ValidUntil 
	//     Confidentiality    | 
	//     Status             | DRAFT, FINALDRAFT, FINAL, REVISION, NOTDEFINED

	// 
	// -> IfcDocumentReference
	//     IFC2X3         | IFC4
	//     -------------- | ------------------
	//	   Location			MODIFIED	** Change
	//	   ItemReference  | Identification		
	//	                  | Description ADDED	
	//	   () -> ReferencedDocument ADDED	


	/// <summary>
	/// Constrain model parts on the ground of a document attached via relation.
	/// </summary>
	public partial class DocumentFacet : FacetBase, IFacet, IEquatable<DocumentFacet>
	{
		public ValueConstraint? DocId { get; set; } // Ide
		public ValueConstraint? DocName { get; set; }
		public ValueConstraint? DocLocation { get; set; }
		public ValueConstraint? DocPurpose { get; set; }
		public ValueConstraint? DocIntendedUse { get; set; }


		public bool Equals(DocumentFacet? other)
		{
			if (other == null)
				return false;
			var thisEqual = (DocId, DocName, DocLocation, DocPurpose, DocIntendedUse)
				.Equals((other.DocId, other.DocName, other.DocLocation, other.DocPurpose, other.DocIntendedUse));
			if (!thisEqual)
				return false;
			return base.Equals(other);
		}
		public override bool Equals(object? obj)
		{
			return this.Equals(obj as DocumentFacet);
		}

		public override string ToString()
		{
			return $"{DocId}-{DocName}-{DocLocation}-{DocPurpose}-{DocIntendedUse}-{base.ToString()}";
		}

		public override int GetHashCode() => (DocId, DocName, DocLocation, DocPurpose, DocIntendedUse).GetHashCode();

		public string Short()
		{
			return ToString();
		}

		public bool IsValid()
		{
			// at least one field is not empty
			return ValueConstraint.IsNotEmpty(DocId) ||
				ValueConstraint.IsNotEmpty(DocName) ||
				ValueConstraint.IsNotEmpty(DocLocation) ||
				ValueConstraint.IsNotEmpty(DocPurpose) ||
				ValueConstraint.IsNotEmpty(DocIntendedUse);
		}
	}
}