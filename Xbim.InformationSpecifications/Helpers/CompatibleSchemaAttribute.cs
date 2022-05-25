using System;
using System.Linq;

namespace Xbim.InformationSpecifications
{
    public class CompatibleSchemaAttribute : Attribute
    {
        private string[] vs;

        public CompatibleSchemaAttribute(string[] vs)
        {
            this.vs = vs;
        }

        public bool IsCompatibleSchema(string s)
        {
            return vs.Contains(s);
        }
    }

    public static class CompatibleSchemaHelpers
    {
        public static bool IsCompatibleSchema(this PartOfFacet.Container container, string s)
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