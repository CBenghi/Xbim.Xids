using System;

namespace Xbim.IDS
{
    public partial class IsClassified : ExpectationFacet
    {
		public string ClassificationName { get; set; }

		public string ClassificationValue { get; set; }

		public IsClassifiedValueMode ValueMode { get; set; }

      	public override bool Validate()
		{
			// Strictly speaking we only need ClassificationValue
			if (string.IsNullOrWhiteSpace(ClassificationValue))
				return false;
			if (Guid == Guid.Empty)
				Guid = Guid.NewGuid();
			return true;
		}
	}

    public enum IsClassifiedValueMode
    {
        Exact,
        Regex,
    }
}
