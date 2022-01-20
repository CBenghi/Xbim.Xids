using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Xbim.InformationSpecifications.Tests
{
	[TestClass]
	public class FacetGroupTests
	{
		[TestMethod]
		public void FacetGroupUseCount()
		{
			Xids t = new Xids();
			var spec = t.PrepareSpecification("IFC2X3");
			var group = spec.Applicability;

			Assert.AreEqual(1, group.UseCount(t));
			var spec2 = t.PrepareSpecification("IFC2X3");
			spec2.Requirement = group;
			Assert.AreEqual(2, group.UseCount(t));
		}
	}
}
