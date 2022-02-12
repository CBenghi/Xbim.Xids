using System;
using System.Text;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications.Generator
{
    internal class AttributesValueGenerator
    {
        /// <summary>
        /// this function is used to generate non-reflection code for the verify dll.
        /// </summary>
        internal static string Execute()
        {
            var schema = SchemaInfo.SchemaIfc4; // todo: make sure to do it for other schemas as well.
            int iCnt = 0;
            StringBuilder s = new StringBuilder();
            foreach (var attName in schema.GetAttributeNames())
            {
                s.AppendLine($"case \"{attName}\":");
                foreach (var className in schema.GetAttributeClasses(attName, true))
                {
                    var classInfo = schema[className];
                    s.AppendLine($"if (entity is {classInfo.NameSpace}.{classInfo.Name} v{iCnt}) return v{iCnt}.{attName};");
                    iCnt++;
                }
                s.AppendLine($"break;");

            }
            return s.ToString();
            
        }
    }
}