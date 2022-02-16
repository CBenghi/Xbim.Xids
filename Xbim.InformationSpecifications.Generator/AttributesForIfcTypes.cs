using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Metadata;

namespace Xbim.InformationSpecifications.Generator
{
	internal class AttributesForIfcTypes
	{
		internal static object Execute(Properties.Version schema)
		{
			Debug.WriteLine($"# Schema {schema}");
            string mainTypeObjectName;
            System.Reflection.Module module;
            if (schema == Properties.Version.IFC2x3)
            {
                module = (typeof(Ifc2x3.Kernel.IfcProduct)).Module;
                mainTypeObjectName = "IfcTypeObject";
            }
            else if (schema == Properties.Version.IFC4)
            {
                module = (typeof(Ifc4.Kernel.IfcProduct)).Module;
                mainTypeObjectName = "IfcTypeObject";
            }
            else
                throw new NotImplementedException();
            var metaD = ExpressMetaData.GetMetadata(module);

			// get all types
			//
			var TypesToObjectMatch = new Dictionary<ExpressType, ExpressType>();
			var ObjectToTypesMatch = new Dictionary<ExpressType, List<ExpressType>>();
			var mainType = metaD.ExpressType(mainTypeObjectName.ToUpperInvariant());
			foreach (var typeType in mainType.AllSubTypes)
			{

				var matched = GetMatch(typeType, metaD);
				if (matched == null)
					continue;
				TypesToObjectMatch.Add(typeType, matched);
				if (ObjectToTypesMatch.TryGetValue(matched, out var objToTypesMatch))
                {
					objToTypesMatch.Add(typeType);
                }
				else
                {
					ObjectToTypesMatch.Add(matched, new List<ExpressType>() { typeType });
				}
				
			}

			Debug.WriteLine("## REPORT FOR TYPE ========================");
			foreach (var m in TypesToObjectMatch)
            {
				Debug.WriteLine($"{m.Key} : {m.Value}");
            }
			Debug.WriteLine("## REPORT FOR Objects ========================");
			foreach (var m in ObjectToTypesMatch)
			{
				Debug.WriteLine($"{m.Key} : {string.Join(", ", m.Value)}");
			}
			return "";
		}

        private static ExpressType GetMatch(ExpressType typeType, ExpressMetaData metaD)
        {
			// string indent = new string('\t', v);
			string indent = "";
			var search = typeType.Name;
			if (search.EndsWith("Type"))
				search = search.Substring(0, search.Length - 4);
			else if (search.EndsWith("Style"))
				search = search.Substring(0, search.Length - 5);
			else
			{
				Debug.WriteLine($"{indent}Skipped: {search}");
			}
			if (!metaD.TryGetExpressType(search.ToUpperInvariant(), out var matched))
			{
				if (typeType.SuperType == null)
					Debug.WriteLine($"{indent}No match: {typeType.Name} ");
				else
                {
					return GetMatch(typeType.SuperType, metaD);
                }
			}
			return matched;
		}
    }
}
