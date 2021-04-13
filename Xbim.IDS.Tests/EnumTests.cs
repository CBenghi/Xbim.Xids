using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Xbim.InformationSpecifications.Tests
{
	[TestClass]
	public class EnumTests
	{
		[TestMethod]
		public void CanSetEnum()
		{
			IfcClassificationFacet f = new IfcClassificationFacet();

			var values = Enum.GetValues(typeof(Location)).Cast<Location>();
			foreach (var val in values)
			{
				f.SetLocation(val);
				Assert.AreEqual(val, f.GetLocation());
			}
		}
	}
}
