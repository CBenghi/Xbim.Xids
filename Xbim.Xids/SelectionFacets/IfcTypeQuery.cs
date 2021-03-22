using System;

namespace Xbim.Xids
{
    public partial class IfcTypeQuery : IFilter, IEquatable<IfcTypeQuery>
    {
        public string IfcType { get; set; } = "";

        public bool IncludeSubtypes { get; set; } = true;

        public override bool Equals(object obj)
        {
            return this.Equals(obj as IfcTypeQuery);
        }

		public override string ToString()
		{
            return $"{IfcType}-{IncludeSubtypes}";
        }

		public override int GetHashCode()
        {
           return ToString().GetHashCode();
        }

        public bool Equals(IfcTypeQuery other)
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