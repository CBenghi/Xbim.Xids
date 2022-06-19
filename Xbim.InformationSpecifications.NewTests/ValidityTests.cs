using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Xbim.InformationSpecifications.Tests
{

	public class ValidityTests
	{
		[Fact]
		[Obsolete("Because we use an constructor method mark as obsolete for testing, we prevent the warning by marking the test obsolete too.")]
		public void FacetGroupValidityTests()
		{
			FacetGroup g = new();
			g.IsValid().Should().BeFalse();
			new IfcClassificationFacet() { }.IsValid().Should().BeTrue();	
			
			var prop = new IfcPropertyFacet();
			g.Facets.Add(prop);
			g.IsValid().Should().BeFalse();
			
			prop.PropertySetName = "NowValid";
			prop.PropertyName = "NowValid";
			g.IsValid().Should().BeTrue();	
			

			var type = new IfcTypeFacet();
			g.Facets.Add(type);
			g.IsValid().Should().BeFalse();
			
			type.IfcType = "NowValid";
			g.IsValid().Should().BeTrue();

			var newtype = new IfcTypeFacet() { 
				PredefinedType = "Notok" 
			};
			newtype.IsValid().Should().BeFalse();
			newtype.IfcType = "MustBeDefined";
			newtype.IsValid().Should().BeTrue();

			var mat = new MaterialFacet();
			mat.IsValid().Should().BeTrue();
			

			var doc = new DocumentFacet();
			doc.IsValid().Should().BeFalse();
			
			doc.DocName = "ValidName"; // there is an implicit string to ValueConstraint conversion.
			g.Facets.Add(doc);
			doc.IsValid().Should().BeTrue();
		}
	}
}
