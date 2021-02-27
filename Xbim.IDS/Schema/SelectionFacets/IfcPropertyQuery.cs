namespace Xbim.IDS
{
    public enum IfcPropertyQueryPropertyFormat
    {
        text,
        integer,
        floating,
        boolean,
    }


    public partial class IfcPropertyQuery : IFilter
    {
		public string PropertySetName { get; set; }

		public string PropertyName { get; set; }

		public string PropertyValue { get; set; }

		public IfcPropertyQueryPropertyFormat PropertyFormat { get; set; }

        public string Short()
        {
            return ToString();
        }
	}
}
