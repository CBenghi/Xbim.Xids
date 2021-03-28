using System;

namespace Xbim.Xids
{
    public partial class IfcTypeFacet : IFacet, IEquatable<IfcTypeFacet>
    {
        public string IfcType { get; set; }

        public string PredefinedType { get; set; } 

        public bool IncludeSubtypes { get; set; } = true;

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

		public string Short()
        {
            return ToString();
        }
    }
}