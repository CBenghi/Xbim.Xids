using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Xbim.InformationSpecifications.NewTests
{
	public class FacetBaseTests
	{
		[Fact]
		void DoesTest()
		{
			DocumentFacet f1 = new DocumentFacet();
			DocumentFacet f2 = new DocumentFacet();

			f1.Should().Be(f2);

			f1.Use = Use.required.ToString();
			f1.Should().NotBe(f2);
			f2.Use= Use.required.ToString();
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
