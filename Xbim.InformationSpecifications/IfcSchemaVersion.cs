using System.Collections.Generic;
using System.Linq;
using Xbim.InformationSpecifications.Generator.Measures;

namespace Xbim.InformationSpecifications
{
	/// <summary>
	/// Closed list of Schema names.
	/// Use <see cref="IfcSchemaVersionHelper"/> to convert to and from <see cref="IdsLib.IfcSchema.IfcSchemaVersions"/>
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

	/// <summary>
	/// Helper class to convert TO and FROM <see cref="IfcSchemaVersion"/> and <see cref="IdsLib.IfcSchema.IfcSchemaVersions"/>
	/// </summary>
	public static class IfcSchemaVersionHelper
	{
		/// <summary>
		/// Converts an <see cref="IfcSchemaVersion"/> to its corresponding <see cref="IdsLib.IfcSchema.IfcSchemaVersions"/> value.
		/// </summary>
		public static IdsLib.IfcSchema.IfcSchemaVersions ToIds(this IfcSchemaVersion version)
		{
			return version switch
			{
				IfcSchemaVersion.IFC2X3 => IdsLib.IfcSchema.IfcSchemaVersions.Ifc2x3,
				IfcSchemaVersion.IFC4 => IdsLib.IfcSchema.IfcSchemaVersions.Ifc4,
				IfcSchemaVersion.IFC4X3 => IdsLib.IfcSchema.IfcSchemaVersions.Ifc4x3,
				_ => IdsLib.IfcSchema.IfcSchemaVersions.IfcNoVersion,
			};
		}

		/// <summary>
		/// Extnsion method to convert from <see cref="IdsLib.IfcSchema.IfcSchemaVersions"/> to <see cref="IfcSchemaVersion"/>
		/// </summary>
		/// <param name="versions">The source version in the XIDS format</param>
		/// <returns>The equivalent IDS enumeration</returns>
		public static IdsLib.IfcSchema.IfcSchemaVersions ToIds(this IEnumerable<IfcSchemaVersion> versions)
		{
			var ret = IdsLib.IfcSchema.IfcSchemaVersions.IfcNoVersion;
			foreach (var v in versions)
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

		/// <summary>
		/// Creates an IEnumerable set of <see cref="IfcSchemaVersion"/> instances that represent the combined schema version information from
		/// the specified collection of IFC schema versions in the IDS lib format.
		/// </summary>
		public static IEnumerable<IfcSchemaVersion> FromIds(this IdsLib.IfcSchema.IfcSchemaVersions versions)
		{
			var ret = new List<IfcSchemaVersion>();
			if (versions.HasFlag(IdsLib.IfcSchema.IfcSchemaVersions.Ifc2x3))
				ret.Add(IfcSchemaVersion.IFC2X3);
			if (versions.HasFlag(IdsLib.IfcSchema.IfcSchemaVersions.Ifc4))
				ret.Add(IfcSchemaVersion.IFC4);
			if (versions.HasFlag(IdsLib.IfcSchema.IfcSchemaVersions.Ifc4x3))
				ret.Add(IfcSchemaVersion.IFC4X3);
			if (ret.Count == 0)
				ret.Add(IfcSchemaVersion.Undefined);
			return ret;
		}
	}
}
