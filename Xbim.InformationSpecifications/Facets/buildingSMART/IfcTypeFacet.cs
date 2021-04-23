using System;
using System.Collections.Generic;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications
{
    public partial class IfcTypeFacet : IFacet, IEquatable<IfcTypeFacet>
    {
        public string IfcType { get; set; }

        public string PredefinedType { get; set; } 

        public bool IncludeSubtypes { get; set; } = true;

        public string Short()
        {
            List<string> desc = new List<string>();
            if (!string.IsNullOrEmpty(IfcType))
            {
                var tmpT = $"is of type {IfcType}";
                if (IncludeSubtypes)
                    tmpT += " or one of its subtypes";
                desc.Add(tmpT);
            }
            if (!string.IsNullOrEmpty(PredefinedType))
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
            return $"{IfcType}-{PredefinedType}-{IncludeSubtypes}";
        }

		public override int GetHashCode() => (IfcType, PredefinedType, IncludeSubtypes).GetHashCode();

		public bool Equals(IfcTypeFacet other)
		{
            if (other == null)
                return false;
            return (IfcType, PredefinedType, IncludeSubtypes)
                .Equals((other.IfcType, other.PredefinedType, other.IncludeSubtypes));
        }

		

        /// <summary>
        /// Valid if at least one of IfcType or PredefinedType are not NullOrWhiteSpace
        /// </summary>
        /// <returns>true if valid</returns>
        public bool IsValid()
		{
            return !( // negated
                string.IsNullOrWhiteSpace(IfcType)
                &&
                string.IsNullOrWhiteSpace(PredefinedType)
                );
        }
	}
}