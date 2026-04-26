using IdsLib.IfcSchema;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Xbim.InformationSpecifications.Helpers
{
	internal class IfcSchemaHelper
	{
		internal static bool TryGetExactClassInfo(string value,
			[NotNullWhen(true)] out string? className,
			out bool hassubtypes
			)
		{
			var schemas = SchemaInfo.GetSchemas(IfcSchemaVersions.IfcAllVersions);
			foreach (var item in schemas)
			{
				var c = item[value];
				if (c != null)
				{
					className = c.Name;
					hassubtypes = c.SubClasses.Count > 0;
					return true;
				}
			}
			className = null;
			hassubtypes	= false;
			return false;

		}

		internal static bool TryOptimizeTypeConstraint(ValueConstraint ifcType, IfcSchemaVersions schemaVersions, [NotNullWhen(true)] out string? type, out bool includeSubtypes)
		{
			if (ifcType.HasAnyAcceptedValue())
			{
				var exacts = ifcType.AcceptedValues.OfType<ExactConstraint>().Select(x => x.Value).ToArray();
				if (exacts.Length > 1 && exacts.Length == ifcType.AcceptedValues.Count)
				{
					if (SchemaInfo.TrySearchTopClass(exacts, schemaVersions, out var top))
					{
						type = top;
						includeSubtypes = true;
						return true;
					}
				}
			}
			includeSubtypes = false;
			type = null;
			return false;
		}
	}
}
