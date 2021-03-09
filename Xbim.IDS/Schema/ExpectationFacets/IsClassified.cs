using System;

namespace Xbim.IDS
{
    public partial class IsClassified : ExpectationFacet, IEquatable<IsClassified>
	{
		public override string Short()
		{
			return ToString();
		}
		public string ClassificationName { get; set; }

		public string ClassificationValue { get; set; }

		public IsClassifiedValueMode ValueMode { get; set; }

		public override bool Equals(object obj)
		{
			return this.Equals(obj as IsClassified);
		}

		public override int GetHashCode()
		{
			return $"{ClassificationName}-{ClassificationValue}".GetHashCode();
		}

		public bool Equals(IsClassified other)
		{
			if (other == null)
				return false;
			if (ClassificationName.ToLowerInvariant() != other.ClassificationName.ToLowerInvariant())
				return false;
			if (ClassificationValue.ToLowerInvariant() != other.ClassificationValue.ToLowerInvariant())
				return false;
			if (ValueMode != other.ValueMode)
				return false;
			return base.Equals(other as ExpectationFacet);
		}

		public override bool Validate()
		{
			// Strictly speaking we only need ClassificationValue
			if (string.IsNullOrWhiteSpace(ClassificationValue))
				return false;
			//if (Guid == Guid.Empty)
			//	Guid = Guid.NewGuid();
			return true;
		}
	}

    public enum IsClassifiedValueMode
    {
        Exact,
        Regex,
    }
}
