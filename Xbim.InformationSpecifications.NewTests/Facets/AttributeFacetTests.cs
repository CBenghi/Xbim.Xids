using FluentAssertions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.Facets
{
    public class AttributeFacetTests
    {
        [Theory]
        [MemberData(nameof(GetSingleAttributes))]
        public void AttributeEqualMatchImplementation(AttributeFacet t, AttributeFacet tSame)
        {
            FacetImplementationTests.TestAddRemove(t);
            var aresame = t.Equals(tSame);
            if (!aresame)
            {
                Debug.WriteLine(t);
            }
            aresame.Should().BeTrue();
            t.Equals(null).Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(GetDifferentAttributesPairs))]
        public void AttributeEqualNotMatchImplementation(AttributeFacet one, AttributeFacet other)
        {
            one.Equals(other).Should().BeFalse();
        }

        public static IEnumerable<object[]> GetDifferentAttributesPairs()
        {
            var source = GetDifferentAttributes().ToArray();
            for (int i = 0; i < source.Length; i++)
            {
                for (int t = i + 1; t < source.Length; t++)
                {
                    yield return new object[] { source[i], source[t] };
                }
            }
        }

        public static IEnumerable<object[]> GetSingleAttributes()
        {
            var set1 = GetDifferentAttributes().ToList();
            var set2 = GetDifferentAttributes().ToList();
            for (int i = 0; i < set1.Count; i++)
            {
                yield return new object[]
                {
                    set1[i],
                    set2[i],
                };
            }
        }

        public static IEnumerable<AttributeFacet> GetDifferentAttributes()
        {
            // this enumeration is all facets that are not consiered equal
            yield return new AttributeFacet();
            yield return new AttributeFacet() { AttributeName = "One" };
            yield return new AttributeFacet() { AttributeName = "Two" };
            yield return new AttributeFacet() { AttributeValue = "One" };
            yield return new AttributeFacet() { AttributeValue = "Two" };
            yield return new AttributeFacet() { Instructions = "Two" };
            yield return new AttributeFacet()
            {
                AttributeName = "One",
                AttributeValue = "Two",
                Instructions = "Some instructions",
                Uri = "http://www.google.com",
            };
        }

        [Fact]
        void ValidityTests()
        {
            var t = new AttributeFacet
            {
                AttributeName = "EngagedIn"
            };
            t.IsValid().Should().BeFalse("EngagedIn is an inverse property"); 

            t = new AttributeFacet
            {
                AttributeName = ValueConstraint.CreatePattern("Enga.*In") // no matching direct property should be found
            };
            t.IsValid().Should().BeFalse("only properties matched by the pattern (EngagedIn) are invalid because inverse"); 


            t = new AttributeFacet
            {
                AttributeName = "Name"
            };
            t.IsValid().Should().BeTrue("Name is a direct property");

            t = new AttributeFacet
            {
                AttributeName = ValueConstraint.CreatePattern("Repre.*ation") 
            };
            t.IsValid().Should().BeTrue("Representation should be matched and should be valid"); 
            
        }

        
    }
}
