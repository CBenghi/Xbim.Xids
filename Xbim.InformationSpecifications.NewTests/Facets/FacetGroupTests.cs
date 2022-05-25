using FluentAssertions;
using Xunit;

namespace Xbim.InformationSpecifications.Tests
{
    public class FacetGroupTests
	{
		[Fact]
		public void FacetGroupUseCount()
		{
			Xids t = new();
			var spec = t.PrepareSpecification(IfcSchemaVersion.IFC2X3);
			var group = spec.Applicability;
			group.UseCount(t).Should().Be(1);
			
			var spec2 = t.PrepareSpecification(IfcSchemaVersion.IFC2X3);
			spec2.Requirement = group;
			group.UseCount(t).Should().Be(2);
		}
	}
}
