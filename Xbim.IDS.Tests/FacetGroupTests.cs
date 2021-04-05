using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Xbim.IDS.Tests
{
	[TestClass]
	public class FacetGroupTests
	{
		[TestMethod]
		public void FacetGroupUseCount()
		{
			Xids.Xids t = new Xids.Xids();
			var spec = t.NewSpecification();
			var group = spec.Applicability;

			Assert.AreEqual(1, group.UseCount(t));
			var spec2 = t.NewSpecification();
			spec2.Requirement = group;
			Assert.AreEqual(2, group.UseCount(t));
		}
	}
}
