using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Xids;

namespace Xbim.IDS.Tests
{
	[TestClass]
	public class HelpersTests
	{
		[TestMethod]
		public void HasPropV4()
		{
			Assert.IsTrue(Xids.Helpers.PropertySetInfo.SchemaIfc4.Any());
			Assert.AreNotEqual(1, Xids.Helpers.PropertySetInfo.SchemaIfc4.Count());
		}

		[TestMethod]
		public void HasPropV2x3()
		{
			Assert.IsTrue(Xids.Helpers.PropertySetInfo.SchemaIfc2x3.Any());
			Assert.AreNotEqual(1, Xids.Helpers.PropertySetInfo.SchemaIfc2x3.Count());
		}

		[TestMethod]
		public void HasClassV4()
		{
			Assert.IsTrue(Xids.Helpers.SchemaInfo.SchemaIfc4.Any());
			Assert.AreNotEqual(2, Xids.Helpers.SchemaInfo.SchemaIfc4.Count());
			Assert.AreEqual("IfcObject", Xids.Helpers.SchemaInfo.SchemaIfc4["IfcProduct"].Parent.Name);
			Assert.AreEqual(3, Xids.Helpers.SchemaInfo.SchemaIfc4["IfcFeatureElement"].SubClasses.Count());
			Assert.AreEqual(5, Xids.Helpers.SchemaInfo.SchemaIfc4["IfcFeatureElement"].MatchingConcreteClasses.Count());
			Assert.IsTrue(Xids.Helpers.SchemaInfo.SchemaIfc4["IfcWall"].Is("IfcWall"));
			Assert.IsTrue(Xids.Helpers.SchemaInfo.SchemaIfc4["IfcWallStandardCase"].Is("IfcWall"));
			Assert.IsFalse(Xids.Helpers.SchemaInfo.SchemaIfc4["IfcWall"].Is("IfcWallStandardCase"));
		}

		[TestMethod]
		public void HasClassV2x3()
		{
			Assert.IsTrue(Xids.Helpers.SchemaInfo.SchemaIfc2x3.Any());
			Assert.AreNotEqual(2, Xids.Helpers.SchemaInfo.SchemaIfc2x3.Count());
		}



	}
}
