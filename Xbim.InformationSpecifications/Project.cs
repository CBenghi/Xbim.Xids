namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// Contains project metadata for the identification of the XIDS
    /// </summary>
    public partial class Project
    {
        /// <summary>
        /// Optional project name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Optional project description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Optional project UniqueIdentifier
        /// </summary>
        public string? Guid { get; set; }
    }
}