using System;
using System.Reflection;
using SchemaVersion = Xbim.Properties.Version;

namespace Xbim.InformationSpecifications.Generator
{
    internal static class SchemaHelper
    {
        public static Module GetModule(SchemaVersion schema)
        {
            if (schema == Properties.Version.IFC2x3)
                return (typeof(Ifc2x3.Kernel.IfcProduct)).Module;
            else if (schema == Properties.Version.IFC4)
                return (typeof(Ifc4.Kernel.IfcProduct)).Module;
            else if (schema == Properties.Version.IFC4x3)
                return (typeof(Ifc4x3.Kernel.IfcProduct)).Module;
            else
                throw new NotImplementedException(schema.ToString());
        }
    }
}
