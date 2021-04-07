using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.IDS.Tests
{
	[TestClass]
	public class EnumTests
	{
		[TestMethod]
		public void CanSetEnum()
		{
			Xids.IfcClassificationFacet f = new Xids.IfcClassificationFacet();

			var values = Enum.GetValues(typeof(Xids.Location)).Cast<Xbim.Xids.Location>();
			foreach (var val in values)
			{
				f.SetLocation(val);
				Assert.AreEqual(val, f.GetLocation());
			}
		}
	}
}
