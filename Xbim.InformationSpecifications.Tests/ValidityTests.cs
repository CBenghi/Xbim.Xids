using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Xbim.InformationSpecifications.Tests
{
	[TestClass]
	public class ValidityTests
	{
		[TestMethod]
		[Obsolete]
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
			prop.PropertyName = "NowValid";
			Assert.IsTrue(g.IsValid());

			var type = new IfcTypeFacet();
			g.Facets.Add(type);
			Assert.IsFalse(g.IsValid());
			type.IfcType = "NowValid";
			Assert.IsTrue(g.IsValid());

			var newtype = new IfcTypeFacet() { 
				PredefinedType = "Notok" 
			};
			Assert.IsFalse(newtype.IsValid());
			newtype.IfcType = "MustBeDefined";
			Assert.IsTrue(newtype.IsValid());

			var mat = new MaterialFacet();
			Assert.IsTrue(mat.IsValid());

			var doc = new DocumentFacet();
			Assert.IsFalse(doc.IsValid());
			doc.DocName = "ValidName"; // there is an implicit string to ValueConstraint conversion.
			g.Facets.Add(doc);
			Assert.IsTrue(doc.IsValid());
		}
	}
}
