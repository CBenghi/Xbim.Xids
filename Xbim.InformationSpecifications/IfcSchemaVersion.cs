using System.Collections.Generic;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// Closed list of Schema names
    /// </summary>
    public enum IfcSchemaVersion
    {
        /// <summary>
        /// When no information is defined
        /// </summary>
        Undefined,
        /// <summary>
        /// Ifc2x3 Schema
        /// </summary>
        IFC2X3,
        /// <summary>
        /// Ifc4 schema
        /// </summary>
        IFC4,
        /// <summary>
        /// Ifc4x3 schema
        /// </summary>
        IFC4X3,
    }

    internal static class IfcSchemaVersionHelper
    {
        public static IdsLib.IfcSchema.IfcSchemaVersions ToIds(this IEnumerable<IfcSchemaVersion> versions) 
        {
            var ret = IdsLib.IfcSchema.IfcSchemaVersions.IfcNoVersion;
            foreach ( var v in versions)
            {
                switch (v)
                {
                    case IfcSchemaVersion.Undefined:
                        break;
                    case IfcSchemaVersion.IFC2X3:
                        ret |= IdsLib.IfcSchema.IfcSchemaVersions.Ifc2x3;
                        break;
                    case IfcSchemaVersion.IFC4:
                        ret |= IdsLib.IfcSchema.IfcSchemaVersions.Ifc4;
                        break;
                    case IfcSchemaVersion.IFC4X3:
                        ret |= IdsLib.IfcSchema.IfcSchemaVersions.Ifc4x3;
                        break;
                }
            }
            return ret;
        }
    }
}
