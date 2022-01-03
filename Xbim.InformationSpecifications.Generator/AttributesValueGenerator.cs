using System;
using System.Text;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications.Generator
{
    internal class AttributesValueGenerator
    {
        // this function is used to generate code non-reflection code for the verify dll.
        internal static string Execute()
        {
            var t = SchemaInfo.SchemaIfc4;
            int iCnt = 0;
            StringBuilder s = new StringBuilder();
            foreach (var attName in t.GetAttributeNames())
            {
                s.AppendLine($"case \"{attName}\":");
                foreach (var className in t.GetAttributeClasses(attName, true))
                {
                    var classInfo = t[className];
                    s.AppendLine($"if (entity is {classInfo.NameSpace}.{classInfo.Name} v{iCnt}) return v{iCnt}.{attName};");
                    iCnt++;
                }
                s.AppendLine($"break;");

            }
            return s.ToString();
            
        }
    }
}