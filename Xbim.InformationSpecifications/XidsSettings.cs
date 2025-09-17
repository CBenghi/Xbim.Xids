namespace Xbim.InformationSpecifications
{
	/// <summary>
	/// Settings controlling system behaviour
	/// </summary>
	public class XidsSettings
	{
		/// <summary>
		/// Determines if the XML output should be indented for readability. (Default: true)
		/// </summary>
		public bool PrettyOutput { get; set; } = true;

		/// <summary>
		/// Determine the maximum number of values to output when describing a Facet. (Default: truncate at 10)
		/// </summary>
		/// <remarks>Can be used to limit the description of facets with ValueConstraints comprised of very long enumeration lists</remarks>
		public int MaximumConstraintEnumsToDescribe { get; set; } = 10;

		/// <summary>
		/// Determines whether numeric prefixs should be applied to IDS filenames when exporting multiple <see cref="SpecificationsGroup"/>s
		/// </summary>
		/// <remarks>Numeric prefixes are always used when a group has no name</remarks>
		public bool ApplyPrefixToSpecGroupFileNames { get; set; } = true;
	}
}
