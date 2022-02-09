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

			// Console.Write(PropertiesGenerator.Execute());
			// Console.Write(PropertyApplicabilityStudy.Execute());;
			// Console.Write(ClassGenerator.Execute());
			// Console.Write(AttributesGenerator.Execute()); 
			// Console.Write(AttributesValueGenerator.Execute());

			Console.Write(MeasureAutomation.Execute());
#if true
			
#endif
		}
	}
}
