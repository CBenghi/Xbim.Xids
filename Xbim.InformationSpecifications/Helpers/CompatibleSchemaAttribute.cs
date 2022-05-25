using System;
using System.Linq;

namespace Xbim.InformationSpecifications
{
    public class CompatibleSchemaAttribute : Attribute
    {
        private IfcSchemaVersion[] vs;

        public CompatibleSchemaAttribute(IfcSchemaVersion[] vs)
        {
            this.vs = vs;
        }

        public bool IsCompatibleSchema(IfcSchemaVersion s)
        {
            return vs.Contains(s);
        }
    }

    public static class CompatibleSchemaHelpers
    {
        public static bool IsCompatibleSchema(this PartOfFacet.Container container, string schemaVersionString)
        {
            if (Enum.TryParse<IfcSchemaVersion>(schemaVersionString, out var version))
                return IsCompatibleSchema(container, version);
            return false;
        }

        public static bool IsCompatibleSchema(this PartOfFacet.Container container, IfcSchemaVersion s)
        {
            try
            {
                var enumType = typeof(PartOfFacet.Container);
                var memberInfos = enumType.GetMember(container.ToString());
                var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
                if (enumValueMemberInfo is null)    
                    return false; 
                var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(CompatibleSchemaAttribute), false);
                CompatibleSchemaAttribute foundAttribute = (CompatibleSchemaAttribute)valueAttributes[0];
                return foundAttribute.IsCompatibleSchema(s);
            }
            catch
            {
                return false;
            }
        }

    }
}