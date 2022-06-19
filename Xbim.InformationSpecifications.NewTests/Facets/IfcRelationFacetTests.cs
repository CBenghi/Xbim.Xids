using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.Facets
{
    public class IfcRelationFacetTests
    {
        [Theory]
        [MemberData(nameof(GetSingleAttributes))]
        public void AttributeEqualMatchImplementation(IfcRelationFacet t, IfcRelationFacet tSame)
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
        public void AttributeEqualNotMatchImplementation(IfcRelationFacet one, IfcRelationFacet other)
        {
            var result = one.Equals(other);
            if (result == true)
            {
                Debug.WriteLine($"{one} vs {other}");
            }
            result.Should().BeFalse();
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

        private readonly static Guid constantGuid = new();

        public static IEnumerable<IfcRelationFacet> GetDifferentAttributes()
        {
            yield return new IfcRelationFacet();
            yield return new IfcRelationFacet() { SourceId = constantGuid.ToString() };
            yield return new IfcRelationFacet() { Relation = "Voids" }; // warning, if the value is not valid, it gets ignored.
        }
    }
}
