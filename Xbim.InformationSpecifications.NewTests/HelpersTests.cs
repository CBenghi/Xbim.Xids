using FluentAssertions;
using Xunit;
using System.Linq;

namespace Xbim.InformationSpecifications.Tests
{

	public class HelpersTests
	{
		[Fact]
		public void HasPropV4()
		{
			Helpers.PropertySetInfo.SchemaIfc4.Any().Should().BeTrue();
			Helpers.PropertySetInfo.SchemaIfc4.Count().Should().NotBe(1);
		}

		[Fact]
		public void HasPropV2x3()
		{
			Helpers.PropertySetInfo.SchemaIfc2x3.Any().Should().BeTrue();
			Helpers.PropertySetInfo.SchemaIfc2x3.Count().Should().NotBe(1);
		}

		[Fact]
		public void HasClassV4()
		{
			Helpers.SchemaInfo.SchemaIfc4.Any().Should().BeTrue();
			Helpers.SchemaInfo.SchemaIfc4.Count().Should().NotBe(2);
			Helpers.SchemaInfo.SchemaIfc4["IfcProduct"].Parent.Name.Should().Be("IfcObject");
			Helpers.SchemaInfo.SchemaIfc4["IfcFeatureElement"].SubClasses.Count().Should().Be(3);
			Helpers.SchemaInfo.SchemaIfc4["IfcFeatureElement"].MatchingConcreteClasses.Count().Should().Be(5);
			Helpers.SchemaInfo.SchemaIfc4["IfcWall"].Is("IfcWall").Should().BeTrue();
			Helpers.SchemaInfo.SchemaIfc4["IfcWallStandardCase"].Is("IfcWall").Should().BeTrue();
			Helpers.SchemaInfo.SchemaIfc4["IfcWall"].Is("IfcWallStandardCase").Should().BeFalse();
		}

		[Fact]
		public void HasClassV2x3()
		{
			Helpers.SchemaInfo.SchemaIfc2x3.Any().Should().BeTrue();
			Helpers.SchemaInfo.SchemaIfc2x3.Count().Should().NotBe(2);
		}

		[Fact]
		public void HasAttributesV2x3()
		{
			var attribs = Helpers.SchemaInfo.SchemaIfc2x3.GetAttributeClasses("NotExisting");
			attribs.Should().BeEmpty();
			
			attribs = Helpers.SchemaInfo.SchemaIfc2x3.GetAttributeClasses("ID");
			attribs.Length.Should().Be(2);	
				

			var attribNames = Helpers.SchemaInfo.SchemaIfc2x3.GetAttributeNames();
			attribNames.Count().Should().Be(179);
		}

		[Fact]
		public void HasAttributesV4()
		{
			var attribs = Helpers.SchemaInfo.SchemaIfc4.GetAttributeClasses("NotExisting");
			attribs.Should().BeEmpty();
			
			attribs = Helpers.SchemaInfo.SchemaIfc4.GetAttributeClasses("UserDefinedOperationType");
			attribs.Length.Should().Be(3);
			
			var attribNames = Helpers.SchemaInfo.SchemaIfc4.GetAttributeNames();
			attribNames.Count().Should().Be(128);
		}
	}
}
