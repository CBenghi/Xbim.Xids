using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Xids;

namespace Xbim.IDS.Tests
{
	[TestClass]
	public class JsonTests
	{
		[TestMethod]
		public void CanWriteSimpleFormat()
		{
			var x = new Xids.Xids();
			var newspec = x.NewSpecification();
			newspec.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWall" });

			newspec.Requirement.Facets.Add(
				new IfcPropertyFacet()
				{
					PropertySetName = "Pset",
					PropertyName = "Prop"
				}
				);
			x.SaveAsJson("temp.json");
		}
	}
}
