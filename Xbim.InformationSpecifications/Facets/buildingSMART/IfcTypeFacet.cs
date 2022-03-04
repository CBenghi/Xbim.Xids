using System;
using System.Collections.Generic;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications
{
    // todo: xxx: facetbase

    public partial class IfcTypeFacet : FacetBase, IEquatable<IfcTypeFacet>, IFacet
    {
        /// <summary>
        /// Required 
        /// </summary>
        public ValueConstraint IfcType { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        public ValueConstraint PredefinedType { get; set; } 

        /// <summary>
        /// Not part of buildingSmart specification
        /// </summary>
        public bool IncludeSubtypes { get; set; } = true;

        public string Short()
        {
            List<string> desc = new List<string>();
            if (!FacetBase.IsNullOrEmpty(IfcType))
            {
                var tmpT = $"is of type {IfcType}";
                if (IncludeSubtypes)
                    tmpT += " or one of its subtypes";
                desc.Add(tmpT);
            }
            if (!FacetBase.IsNullOrEmpty(PredefinedType))
            {
                desc.Add($"has a predefined type value of '{PredefinedType}'");
            }
            var tmp = string.Join(" and ", desc.ToArray()) + ".";
            return tmp.FirstCharToUpper();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as IfcTypeFacet);
        }

		public override string ToString()
		{
            return $"{IfcType}-{PredefinedType}-{IncludeSubtypes}-{base.ToString()}";
        }

		public override int GetHashCode() => 23 + 31 * (IfcType, PredefinedType, IncludeSubtypes).GetHashCode() + 31 * base.GetHashCode();

		public bool Equals(IfcTypeFacet other)
		{
            if (other == null)
                return false;
            var thisEq = (IfcType, PredefinedType, IncludeSubtypes)
                .Equals((other.IfcType, other.PredefinedType, other.IncludeSubtypes));
            if (!thisEq)
                return false;
            return base.Equals(other);
        }

        /// <summary>
        /// Valid if at least IfcType is meaningful
        /// </summary>
        /// <returns>true if valid</returns>
        public bool IsValid()
		{
            if (FacetBase.IsNullOrEmpty(IfcType))
                return false; 
            return true;
        }
	}
}