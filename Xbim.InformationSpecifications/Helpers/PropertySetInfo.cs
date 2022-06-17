using System.Collections.Generic;
using System.Linq;

namespace Xbim.InformationSpecifications.Helpers
{
	public partial class PropertySetInfo
	{
		public string Name { get; set; }
		public IEnumerable<string> PropertyNames => Properties.Select(p => p.Name);
		public IList<string> ApplicableClasses { get; set; }
		public IList<IPropertyTypeInfo> Properties { get; set; } 

		public IPropertyTypeInfo? GetProperty(string name) => Properties.FirstOrDefault(p => p.Name == name);

		public PropertySetInfo(
			string propertySetName,
			IEnumerable<IPropertyTypeInfo> properties,
			IEnumerable<string> applicableClasses
			)
		{
			Name = propertySetName;
			Properties = properties.ToList();
			ApplicableClasses = applicableClasses.ToList();
		}

		private static IList<PropertySetInfo>? schemaIFC4;
		public static IList<PropertySetInfo> SchemaIfc4
		{
			get
			{
				if (schemaIFC4 == null)
					schemaIFC4 = GetPropertiesIFC4().ToList();
				return schemaIFC4;
			}
		}

		public static IPropertyTypeInfo? Get(IfcSchemaVersion version, string propertySetName, string propertyName)
        {
            IList<PropertySetInfo>? schema = GetSchema(version);
			if (schema == null)
				return null;
            var set = schema.Where(x => x.Name == propertySetName).FirstOrDefault();
            if (set is null)
                return null;
            return set.GetProperty(propertyName);
        }

        public static IList<PropertySetInfo>? GetSchema(IfcSchemaVersion version)
        {
            IList<PropertySetInfo>? schema = version switch
            {
                IfcSchemaVersion.IFC2X3 => SchemaIfc2x3,
                IfcSchemaVersion.IFC4 => SchemaIfc4,
                _ => null,
            };
            return schema;
        }

        private static IList<PropertySetInfo>? schemaIFC2x3;
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
