using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.InformationSpecifications.Generator
{
	class Program
	{
		public static void Main()
		{
			// Console.Write(PropertiesGenerator.Execute());
			// Console.Write(PropertyApplicabilityStudy.Execute());;
			Console.Write(ClassGenerator.Execute()); ;
		}
	}
}
