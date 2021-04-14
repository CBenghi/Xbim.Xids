using System.Collections.Generic;

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



		public static IList<PropertySetInfo> schemaIFC4;
		public static IList<PropertySetInfo> SchemaIfc4
		{
			get
			{
				if (schemaIFC4 == null)
					GetPropertiesIFC4();
				return schemaIFC4;
			}
		}

		public static IList<PropertySetInfo> schemaIFC2x3;
		public static IList<PropertySetInfo> SchemaIfc2x3
		{
			get
			{
				if (schemaIFC2x3 == null)
					GetPropertiesIFC2x3();
				return schemaIFC2x3;
			}
		}

		static partial void GetPropertiesIFC2x3();
		static partial void GetPropertiesIFC4();

	}
}
