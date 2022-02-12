using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xbim.Common.Metadata;
using Xbim.Properties;

namespace Xbim.InformationSpecifications.Generator
{
    public class ClassGenerator
	{
		/// <summary>
		/// SchemaInfo.GeneratedClass.cs
		/// </summary>
		public static string Execute()
        {
			var source = stub;
			var schemas = new[] { Xbim.Properties.Version.IFC2x3, Xbim.Properties.Version.IFC4 };		

			foreach (var schema in schemas)
			{
				System.Reflection.Module module = null;
				if (schema == Properties.Version.IFC2x3)
					module = (typeof(Ifc2x3.Kernel.IfcProduct)).Module;
				else if (schema == Properties.Version.IFC4)
					module = (typeof(Ifc4.Kernel.IfcProduct)).Module;
				var metaD = ExpressMetaData.GetMetadata(module);

				var sb = new StringBuilder();

				// trying to find a set of classes that matches the property types
				List<string> HandledTypes = new List<string>();
				foreach (var item in IfcClassStudy.IncludeTypes[schema]) // this determines the included types by schema
				{
					HandledTypes.AddRange(IfcClassStudy.TreeOf(metaD.ExpressType(item.ToUpperInvariant())));
				}

				foreach (var className in HandledTypes)
				{
					var daType = metaD.ExpressType(className.ToUpperInvariant());

					var propPdefT = daType.Properties.Values.FirstOrDefault(x=>x.Name == "PredefinedType");
					var predType = "null";
					if (propPdefT != null)
					{
						var pt = propPdefT.PropertyInfo.PropertyType;
						pt = Nullable.GetUnderlyingType(pt) ?? pt;
						var vals = Enum.GetValues(pt);


						List<string> pdtypes = new List<string>();
						foreach (var val in vals)
						{
							pdtypes.Add(val.ToString());
						}
						predType = newStringArray(pdtypes.ToArray());
					}

					var t = daType.Type;
					var abstractOrNot = t.IsAbstract ? "ClassType.Abstract" : "ClassType.Concrete";

					var ns = t.Namespace.Substring(5);

					sb.AppendLine($@"			schema{schema}.Add(new ClassInfo(""{daType.Name}"", ""{daType.SuperType.Name}"", {abstractOrNot}, {predType}, ""{ns}""));");
				}
				source = source.Replace($"<PlaceHolder{schema}>\r\n", sb.ToString());
			}
			return source;
		}

		private static string newStringArray(string[] classes)
		{
			return @$"new[] {{ ""{string.Join("\", \"", classes)}"" }}";
		}

		private const string stub = @"// generated code via xbim.xids.generator, any changes made directly here will be lost

namespace Xbim.InformationSpecifications.Helpers
{
	public partial class SchemaInfo
	{
		static partial void GetClassesIFC2x3()
		{
			schemaIFC2x3 = new SchemaInfo();
<PlaceHolderIFC2x3>
		}
		static partial void GetClassesIFC4()
		{
			schemaIFC4 = new SchemaInfo();
<PlaceHolderIFC4>
		}
	}
}
";
	}
}