using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.InformationSpecifications;

namespace Xbim.InformationSpecifications.Tests
{
	[TestClass]
	public class HelpersTests
	{
		[TestMethod]
		public void HasPropV4()
		{
			Assert.IsTrue(Helpers.PropertySetInfo.SchemaIfc4.Any());
			Assert.AreNotEqual(1, Helpers.PropertySetInfo.SchemaIfc4.Count());
		}

		[TestMethod]
		public void HasPropV2x3()
		{
			Assert.IsTrue(Helpers.PropertySetInfo.SchemaIfc2x3.Any());
			Assert.AreNotEqual(1, Helpers.PropertySetInfo.SchemaIfc2x3.Count());
		}

		[TestMethod]
		public void HasClassV4()
		{
			Assert.IsTrue(Helpers.SchemaInfo.SchemaIfc4.Any());
			Assert.AreNotEqual(2, Helpers.SchemaInfo.SchemaIfc4.Count());
			Assert.AreEqual("IfcObject", Helpers.SchemaInfo.SchemaIfc4["IfcProduct"].Parent.Name);
			Assert.AreEqual(3, Helpers.SchemaInfo.SchemaIfc4["IfcFeatureElement"].SubClasses.Count());
			Assert.AreEqual(5, Helpers.SchemaInfo.SchemaIfc4["IfcFeatureElement"].MatchingConcreteClasses.Count());
			Assert.IsTrue(Helpers.SchemaInfo.SchemaIfc4["IfcWall"].Is("IfcWall"));
			Assert.IsTrue(Helpers.SchemaInfo.SchemaIfc4["IfcWallStandardCase"].Is("IfcWall"));
			Assert.IsFalse(Helpers.SchemaInfo.SchemaIfc4["IfcWall"].Is("IfcWallStandardCase"));
		}

		[TestMethod]
		public void HasClassV2x3()
		{
			Assert.IsTrue(Helpers.SchemaInfo.SchemaIfc2x3.Any());
			Assert.AreNotEqual(2, Helpers.SchemaInfo.SchemaIfc2x3.Count());
		}

		
		[TestMethod]
		public void HasAttributesV2x3()
		{
			var attribs = Helpers.SchemaInfo.SchemaIfc2x3.GetAttributeClasses("NotExisting");
			Assert.IsNull(attribs);
			attribs = Helpers.SchemaInfo.SchemaIfc2x3.GetAttributeClasses("ID");
			Assert.AreEqual(2, attribs.Length);

			var attribNames = Helpers.SchemaInfo.SchemaIfc2x3.GetAttributeNames();
			Assert.AreEqual(12, attribNames.Count());
		}

		[TestMethod]
		public void HasAttributesV4()
		{
			var attribs = Helpers.SchemaInfo.SchemaIfc4.GetAttributeClasses("NotExisting");
			Assert.IsNull(attribs);
			attribs = Helpers.SchemaInfo.SchemaIfc4.GetAttributeClasses("UserDefinedOperationType");
			Assert.AreEqual(2, attribs.Length);

			var attribNames = Helpers.SchemaInfo.SchemaIfc4.GetAttributeNames();
			Assert.AreEqual(12, attribNames.Count());
		}
	}
}
