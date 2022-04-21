using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Xbim.InformationSpecifications.NewTests.Facets
{
    public class ValidityTests
    {
        [Theory]
        [MemberData(nameof(GetValidFacets))]
        public void EmptyValidityTests(IFacet facet)
        { 
            // some empty facets are not valid
            facet.IsValid().Should().BeFalse($"{facet.GetType()} should not be valid if empty");
        }

        public static IEnumerable<object[]> GetValidFacets()
        {
            yield return new object[] { new IfcPropertyFacet() };
            yield return new object[] { new AttributeFacet() };
        }
    }
}
