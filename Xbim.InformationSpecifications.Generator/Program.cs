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
			var wip = false;
			Console.WriteLine("Press `t` to generate full testfiles, any other key to continue with next steps of generation.");

			if (Console.ReadKey().Key == ConsoleKey.T)
			{
				// Whenever the schema changes
				// 1. get the latest files with the batch command
				// 2. execute the following function
				BuildingSmartSchema.GenerateFulltestFiles();
			}

			// wip
			if (wip)
			{
				var schemas = new[] {
					Properties.Version.IFC2x3,
					Properties.Version.IFC4
				};
				foreach (var schema in schemas)
					Console.Write(ClassRelationTypes.Report(schema));
				return;
			}

			Console.WriteLine("Running code generation...");

			var study = false;
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
			Console.WriteLine("Running properties generation...");
			var tPropGen = PropertiesGenerator.Execute();
            dest = Path.Combine(destPath.FullName, @"Xbim.InformationSpecifications\Helpers\PropertySetInfo.Generated.cs");
            File.WriteAllText(dest, tPropGen);

			// depends on ExpressMetaData and IfcClassStudy classes
			Console.WriteLine("Running class generation...");
			var tClassGen = ClassGenerator.Execute();
            dest = Path.Combine(destPath.FullName, @"Xbim.InformationSpecifications\Helpers\SchemaInfo.GeneratedClass.cs");
            File.WriteAllText(dest, tClassGen);

			// depends on ExpressMetaData and IfcClassStudy classes
			Console.WriteLine("Running attributes generation...");
			var tAttGen = AttributesGenerator.Execute();
			dest = Path.Combine(destPath.FullName, @"Xbim.InformationSpecifications\Helpers\SchemaInfo.GeneratedAttributes.cs");
			File.WriteAllText(dest, tAttGen);

			// depends on ExpressMetaData and IfcClassStudy classes
			var tRelTypeGen = ClassRelationTypes.Execute();
			Console.WriteLine("Running generation of class relationships...");
			dest = Path.Combine(destPath.FullName, @"Xbim.InformationSpecifications\Helpers\SchemaInfo.GeneratedRelTypes.cs");
			File.WriteAllText(dest, tRelTypeGen);

			// end
			var bkp = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine("Completed");
			Console.ForegroundColor = bkp;
		}
	}
}
