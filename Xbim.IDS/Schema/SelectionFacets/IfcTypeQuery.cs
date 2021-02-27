namespace Xbim.IDS
{
    public partial class IfcTypeQuery : IFilter
    {
		public string IfcType { get; set; }

        public bool IncludeSubtypes { get; set; } = true;

        public string Short()
        {
            return ToString();
        }
    }
}