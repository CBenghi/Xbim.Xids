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
	public class ValidityTests
	{
		[TestMethod]
		public void FacetGroupValidityTests()
		{
			FacetGroup g = new FacetGroup();
			Assert.IsFalse(g.IsValid());

			g.Facets.Add(new IfcClassificationFacet() {  });
			Assert.IsTrue(g.IsValid());

			var prop = new IfcPropertyFacet();
			g.Facets.Add(prop);
			Assert.IsFalse(g.IsValid());
			prop.PropertySetName = "NowValid";
			Assert.IsTrue(g.IsValid());

			var type = new IfcTypeFacet();
			g.Facets.Add(type);
			Assert.IsFalse(g.IsValid());
			type.IfcType = "NowValid";
			Assert.IsTrue(g.IsValid());

			var newtype = new IfcTypeFacet() { PredefinedType = "ok" };
			Assert.IsTrue(newtype.IsValid());

			var mat = new MaterialFacet();
			Assert.IsTrue(mat.IsValid());

			var doc = new DocumentReferenceFacet();
			Assert.IsFalse(doc.IsValid());
			doc.DocumentName = "ValidName";
			g.Facets.Add(doc);
			Assert.IsTrue(doc.IsValid());
		}
	}
}
