using FluentAssertions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.Facets
{
    public class PartOfFacetTests
    {
        [Fact]
        public void HelpersFunctionsWork()
        {
            var f = new PartOfFacet();
            f.GetContainers().Should().HaveCount(0);

            f.SetContainers(new[] { PartOfFacet.Container.IfcSystem, PartOfFacet.Container.IfcDistributionSystem });
            f.GetContainers().Should().HaveCount(2);

            f.SetContainers(new[] { PartOfFacet.Container.IfcSystem });
            f.GetContainers().Should().HaveCount(1);

        }

        [Theory]
        [MemberData(nameof(GetSingleAttributes))]
        public void AttributeEqualMatchImplementation(PartOfFacet t, PartOfFacet tSame)
        {
            FacetImplementationTests.TestAddRemove(t);
            var areSame = t.Equals(tSame);
            if (!areSame)
            {
                Debug.WriteLine(t);
            }
            areSame.Should().BeTrue();
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
            var tp = new IfcTypeFacet() { IfcType = PartOfFacet.Container.IfcElectricalCircuit.ToString() };

            yield return new PartOfFacet();
            yield return new PartOfFacet() { EntityType = tp };
            yield return new PartOfFacet() { Instructions = "instr", };
            yield return new PartOfFacet() { Uri = "uri", };
            yield return new PartOfFacet() { EntityRelation = PartOfFacet.PartOfRelation.IfcRelNests.ToString() };
            yield return new PartOfFacet()
            {
                EntityType = tp,
                Instructions = "instr",
                Uri = "uri",
                EntityRelation = PartOfFacet.PartOfRelation.IfcRelNests.ToString()
            };
        }
    }
}
