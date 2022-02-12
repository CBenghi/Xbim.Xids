using System.Collections.Generic;
using System.Linq;

namespace Xbim.InformationSpecifications.Helpers
{
	public partial class PropertySetInfo
	{
		public string Name { get; set; }
		public IList<string> PropertyNames { get; set; }
		public IList<string> ApplicableClasses { get; set; }


		public PropertySetInfo(
			string propertySetName,
			IList<string> propertyNames,
			IList<string> applicableClasses
			)
		{
			Name = propertySetName;
			PropertyNames = propertyNames;
			ApplicableClasses = applicableClasses;
		}

		private static IList<PropertySetInfo> schemaIFC4;
		public static IList<PropertySetInfo> SchemaIfc4
		{
			get
			{
				if (schemaIFC4 == null)
					schemaIFC4 = GetPropertiesIFC4().ToList();
				return schemaIFC4;
			}
		}

		private static IList<PropertySetInfo> schemaIFC2x3;
		public static IList<PropertySetInfo> SchemaIfc2x3
		{
			get
			{
				if (schemaIFC2x3 == null)
					schemaIFC2x3 = GetPropertiesIFC2x3().ToList();
				return schemaIFC2x3;
			}
		}
	}
}
