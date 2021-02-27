namespace Xbim.IDS
{
    public partial class HasProperty : IFacet
	{
		public string PropertySetName { get; set; }

		public string PropertyName { get; set; }

		public string PropertyType { get; set; }

		public IValueConstraint PropertyConstraint { get; set; }
	}
}