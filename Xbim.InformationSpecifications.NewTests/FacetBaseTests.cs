using FluentAssertions;
using Xunit;

namespace Xbim.InformationSpecifications.Tests
{
    public class FacetBaseTests
    {
        [Fact]
        public void DoesTest()
        {
            DocumentFacet f1 = new();
            DocumentFacet f2 = new();
            f1.Should().Be(f2);

            f1.Instructions = "some";
            f1.Should().NotBe(f2);
            f2.Instructions = "some";
            f1.Should().Be(f2);

            f1.Uri = "someUri";
            f1.Should().NotBe(f2);
            f2.Uri = "someUri";
            f1.Should().Be(f2);

        }
    }
}
