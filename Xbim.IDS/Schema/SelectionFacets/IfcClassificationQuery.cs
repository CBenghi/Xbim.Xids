namespace Xbim.IDS
{
	public partial class IfcClassificationQuery : IFilter
	{
		/// <summary>
		/// A string identifying the relevant classification system
		/// </summary>
		public string ClassificationSystem { get; set; }

		/// <summary>
		/// The specific class element within the tree of the <see cref="ClassificationSystem"/> 
		/// </summary>
		public string Node { get; set; }

		public string Short()
		{
			return "SomeClassificationFilter";
		}
	}
}