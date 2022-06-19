using FluentAssertions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.Facets
{
    public class PartOfFacetTests
    {
        [Theory]
        [MemberData(nameof(GetSingleAttributes))]
        public void AttributeEqualMatchImplementation(PartOfFacet t, PartOfFacet tSame)
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
        public void AttributeEqualNotMatchImplementation(PartOfFacet one, PartOfFacet other)
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

        public static IEnumerable<PartOfFacet> GetDifferentAttributes()
        {
            yield return new PartOfFacet();
            yield return new PartOfFacet() { EntityName = "name", };
            yield return new PartOfFacet() { Instructions = "instr", };
            yield return new PartOfFacet() { Uri = "uri", };
            yield return new PartOfFacet() { Entity = PartOfFacet.Container.IfcElementAssembly.ToString() };
            yield return new PartOfFacet()
            {
                EntityName = "name",
                Instructions = "instr",
                Uri = "uri",
                Entity = PartOfFacet.Container.IfcElementAssembly.ToString()
            };
        }
    }
}
