using System;
namespace Xbim.IDS
{
    public partial class HasQuantity : ExpectationFacet
	{
		public string PropertySetName { get; set; }

		public string QuantityName { get; set; }

		public string QuantityType { get; set; }
		
		public override bool Validate()
		{
			// Strictly speaking we only need QuantityName
			if (string.IsNullOrWhiteSpace(QuantityName))
				return false;
			if (Guid == Guid.Empty)
				Guid = Guid.NewGuid();
			return true;
		}
	}
}
