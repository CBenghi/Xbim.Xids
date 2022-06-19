using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Metadata;

namespace Xbim.InformationSpecifications.Generator
{
	class ClassRelationTypeInfo
    {
		public Dictionary<ExpressType, ExpressType> TypesToObjectMatch = new();
		public Dictionary<ExpressType, List<ExpressType>> ObjectToTypesMatch = new();
	}

	internal static class ClassRelationTypes
	{
		internal static ClassRelationTypeInfo GetRelationTypes(Properties.Version schema)
		{
			ClassRelationTypeInfo c = new();
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
			var mainType = metaD.ExpressType(mainTypeObjectName.ToUpperInvariant());
			foreach (var typeType in mainType.AllSubTypes)
			{

				var matched = GetMatch(typeType, metaD);
				if (matched == null)
					continue;
				c.TypesToObjectMatch.Add(typeType, matched);
				if (c.ObjectToTypesMatch.TryGetValue(matched, out var objToTypesMatch))
                {
					objToTypesMatch.Add(typeType);
                }
				else
                {
					c.ObjectToTypesMatch.Add(matched, new List<ExpressType>() { typeType });
				}
				
			}

			
			return c;
		}

        internal static string Report(Properties.Version schema)
        {
			StringBuilder sb = new();
			sb.AppendLine("## SHEMA START ========================");
			sb.AppendLine($"# Schema {schema}");
			var c = GetRelationTypes(schema);

			
			sb.AppendLine("## REPORT FOR TYPE ========================");
			foreach (var m in c.TypesToObjectMatch)
			{
				sb.AppendLine($"{m.Key} : {m.Value}");
			}
			sb.AppendLine("## REPORT FOR Objects ========================");
			foreach (var m in c.ObjectToTypesMatch)
			{
				sb.AppendLine($"{m.Key} : {string.Join(", ", m.Value)}");
			}
			return sb.ToString();
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

		private static string NewStringArray(string[] classes)
		{
			return @$"new[] {{ ""{string.Join("\", \"", classes)}"" }}";
		}

		public static string Execute()
		{
			var source = stub;
			var schemas = new[] { Xbim.Properties.Version.IFC2x3, Xbim.Properties.Version.IFC4 };

			foreach (var schema in schemas)
			{
				var sb = new StringBuilder();
				var rel = GetRelationTypes(schema);	
				foreach (var pair in rel.ObjectToTypesMatch)
                {
					sb.AppendLine($"\t\t\tschema.SetRelationType(\"{pair.Key.Name}\", {NewStringArray(pair.Value.Select(x=>x.Name).ToArray())});");
                }				
				source = source.Replace($"<PlaceHolder{schema}>\r\n", sb.ToString());
			}
			return source;
		}

		private const string stub = @"// generated code via xbim.xids.generator, any changes made directly here will be lost

using System.Collections.Generic;

namespace Xbim.InformationSpecifications.Helpers
{
	public partial class SchemaInfo
	{
		static partial void GetRelationTypesIFC2x3(SchemaInfo schema)
		{
<PlaceHolderIFC2x3>
		}

		static partial void GetRelationTypesIFC4(SchemaInfo schema)
		{
<PlaceHolderIFC4>
		}
	}
}
";
	}
}
