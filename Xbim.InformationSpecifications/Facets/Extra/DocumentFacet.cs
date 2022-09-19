using System;

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
        /// <summary>
        /// mapped to DocumentId in IFC2x3 or Identification in Ifc4
        /// </summary>
		public ValueConstraint? DocId { get; set; }
        /// <summary>
        /// Mapped to Name property of IFC entities.
        /// </summary>
        public ValueConstraint? DocName { get; set; }
        /// <summary>
        /// DocumentReferences in IFC2x3 or Location in IFC4
        /// </summary>
		public ValueConstraint? DocLocation { get; set; }
        /// <summary>
        /// Mapped to Purpose property of IFC entities.
        /// </summary>
		public ValueConstraint? DocPurpose { get; set; }
        /// <summary>
        /// Mapped to IntendedUse property of IFC entities.
        /// </summary>
		public ValueConstraint? DocIntendedUse { get; set; }

        /// <inheritdoc />
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

        /// <inheritdoc />
		public override bool Equals(object? obj)
        {
            return this.Equals(obj as DocumentFacet);
        }

        /// <inheritdoc />
		public override string ToString()
        {
            return $"{DocId}-{DocName}-{DocLocation}-{DocPurpose}-{DocIntendedUse}-{base.ToString()}";
        }

        /// <inheritdoc />
		public override int GetHashCode() => (DocId, DocName, DocLocation, DocPurpose, DocIntendedUse).GetHashCode();

        /// <inheritdoc />
		public string Short()
        {
            return ToString();
        }


        /// <summary>
        /// Valid (see <see cref="IFacet.IsValid"/>) if at least one of 
        /// <see cref="DocId"/>, 
        /// <see cref="DocName"/>, 
        /// <see cref="DocLocation"/>,
        /// <see cref="DocPurpose"/> or
        /// <see cref="DocIntendedUse"/>,
        /// are not empty.
        /// </summary>
        /// <returns>true if valid</returns>
        public bool IsValid()
        {

            return
                (// at least one field is not empty
                    ValueConstraint.IsNotEmpty(DocId) ||
                    ValueConstraint.IsNotEmpty(DocName) ||
                    ValueConstraint.IsNotEmpty(DocLocation) ||
                    ValueConstraint.IsNotEmpty(DocPurpose) ||
                    ValueConstraint.IsNotEmpty(DocIntendedUse)
                ) // but they are all valid, if defined
                && FacetBase.IsValidOrNull(DocId)
                && FacetBase.IsValidOrNull(DocName)
                && FacetBase.IsValidOrNull(DocLocation)
                && FacetBase.IsValidOrNull(DocPurpose)
                && FacetBase.IsValidOrNull(DocIntendedUse);
        }
    }
}