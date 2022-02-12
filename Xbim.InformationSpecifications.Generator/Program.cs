using System;
using System.Collections.Generic;
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
			// NOTE: IN DEBUG MODE OUTPUT IS LIKELY REDIRECTED TO FILE (file.cs)
			var avoid = false;
			var study = false;

			// Initialization
			if (study)
			{
				Console.Write(IfcClassStudy.Execute());  // relevant classes preview
				Console.Write(MeasureAutomation.Execute()); // measures and dimensional exponents
				return;
			}

			// Ifc classes and properties
			if (avoid) Console.Write(PropertiesGenerator.Execute()); // depends on Xbim.Properties assembly
			if (avoid) Console.Write(ClassGenerator.Execute()); // depends on ExpressMetaData and IfcClassStudy classes
			if (avoid) Console.Write(AttributesGenerator.Execute()); // depends on ExpressMetaData and IfcClassStudy classes

			// for the verification dll 
			// it should be moved there
			if (avoid) Console.Write(AttributesValueGenerator.Execute());
			
			// wip
			if (avoid) Console.Write(AttributesForIfcTypes.Execute());
			
			
			
		}
	}
}
