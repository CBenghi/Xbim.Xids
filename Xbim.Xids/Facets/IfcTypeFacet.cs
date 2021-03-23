using System;

namespace Xbim.Xids
{
    public partial class IfcTypeFacet : IFacet, IEquatable<IfcTypeFacet>
    {
        public string IfcType { get; set; } = "";

        public bool IncludeSubtypes { get; set; } = true;

        public override bool Equals(object obj)
        {
            return this.Equals(obj as IfcTypeFacet);
        }

		public override string ToString()
		{
            return $"{IfcType}-{IncludeSubtypes}";
        }

		public override int GetHashCode()
        {
           return ToString().GetHashCode();
        }

        public bool Equals(IfcTypeFacet other)
		{
            if (other == null)
                return false;
            if (IfcType.ToLowerInvariant() != other.IfcType.ToLowerInvariant())
                return false;
            if (IncludeSubtypes != other.IncludeSubtypes)
                return false;
            return true;
		}

		public string Short()
        {
            return ToString();
        }
    }
}