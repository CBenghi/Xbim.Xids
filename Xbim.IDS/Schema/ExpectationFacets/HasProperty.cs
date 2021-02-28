using System;
namespace Xbim.IDS
{
    public partial class HasProperty : ExpectationFacet
	{
		public string PropertySetName { get; set; }

		public string PropertyName { get; set; }

		public string PropertyType { get; set; }

		public IValueConstraint PropertyConstraint { get; set; }
		

		public override bool Validate()
		{
			// Strictly speaking we only need property name
			if (string.IsNullOrWhiteSpace(PropertyName))
				return false;
			if (Guid == Guid.Empty)
				Guid = Guid.NewGuid();
			return true;
		}
	}
}