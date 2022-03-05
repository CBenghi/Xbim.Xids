namespace Xbim.InformationSpecifications
{
	/// <summary>
	/// Central interface of all facets for a model.
	/// </summary>
	public interface IFacet
	{
		/// <summary>
		/// A generated short textual description of the facet's constraints produced via its properties.
		/// </summary>
		/// <returns>A string</returns>
		string Short();

		/// <summary>
		/// Evaluates if the facet is formally valid, i.e. its required propertries are defined and correct.
		/// </summary>
		/// <returns>true if valid, false otherwise</returns>
		bool IsValid();
	}
}
