using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.InformationSpecifications.Generator.Measures;

namespace Xbim.InformationSpecifications.Generator
{
	class Program
	{
		public static void Main()
		{
			var avoid = true;
			// wip
			if (avoid)
			{
				var schemas = new[] {
					Properties.Version.IFC2x3,
					Properties.Version.IFC4
				};
				foreach (var schema in schemas)
					Console.Write(AttributesForIfcTypes.Execute(schema));
				
				return;
			}

			var study = true;
			var destPath = new DirectoryInfo(@"..\..\..\..\");
			// Initialization
			if (study)
			{
				Console.Write(IfcClassStudy.ReportMatchesToProperties());  // relevant classes preview
				// Console.Write(MeasureAutomation.Execute()); // measures and dimensional exponents
				return;
			}
			string dest = "";

            // depends on Xbim.Properties assembly
            var tPropGen = PropertiesGenerator.Execute();
            dest = Path.Combine(destPath.FullName, @"Xbim.InformationSpecifications\Helpers\PropertySetInfo.Generated.cs");
            File.WriteAllText(dest, tPropGen);

            // depends on ExpressMetaData and IfcClassStudy classes
            var tClassGen = ClassGenerator.Execute();
            dest = Path.Combine(destPath.FullName, @"Xbim.InformationSpecifications\Helpers\SchemaInfo.GeneratedClass.cs");
            File.WriteAllText(dest, tClassGen);

            // depends on ExpressMetaData and IfcClassStudy classes
            var tAttGen = AttributesGenerator.Execute();
			dest = Path.Combine(destPath.FullName, @"Xbim.InformationSpecifications\Helpers\SchemaInfo.GeneratedAttributes.cs");
			File.WriteAllText(dest, tAttGen);

			
		}
	}
}
