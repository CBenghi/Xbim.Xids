using System;
namespace Xbim.IDS
{
    public partial class HasMaterial : ExpectationFacet
    {
		public string MaterialName { get; set; }
		
		public override bool Validate()
		{
			// Strictly speaking we only need property name
			if (string.IsNullOrWhiteSpace(MaterialName))
				return false;
			if (Guid == Guid.Empty)
				Guid = Guid.NewGuid();
			return true;
		}
	}
}