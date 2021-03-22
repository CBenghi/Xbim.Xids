using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.Xids
{
	public class IsType : ExpectationFacet, IEquatable<IsType>
	{
		public bool Equals(IsType other)
		{
			if (other == null)
				return false;
			if (IfcType.ToLowerInvariant() != other.IfcType.ToLowerInvariant())
				return false;
			if (IncludeSubtypes != other.IncludeSubtypes)
				return false;
			return base.Equals(other as ExpectationFacet);
		}

		public override string Short()
		{
			return ToString();
		}

		public override string ToString()
		{
			return $"{IfcType}-{IncludeSubtypes}";
		}

		public override bool Validate()
		{
			throw new NotImplementedException();
		}

		public string IfcType { get; set; } = "";

		public bool IncludeSubtypes { get; set; } = true;

	}
}
