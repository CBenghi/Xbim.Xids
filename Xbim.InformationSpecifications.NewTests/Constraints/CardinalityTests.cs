using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.InformationSpecifications.Cardinality;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.Constraints
{
    public class CardinalityTests
    {
        [Fact]
        public void SimpleCardinality_IsSatisfiedBy_Works()
        {
            var sc = new SimpleCardinality(CardinalityEnum.Optional);
            sc.IsSatisfiedBy(0).Should().BeTrue();
            sc.IsSatisfiedBy(1).Should().BeTrue();
            sc.IsSatisfiedBy(10).Should().BeTrue();

            sc = new SimpleCardinality(CardinalityEnum.Required);
            sc.IsSatisfiedBy(0).Should().BeFalse();
            sc.IsSatisfiedBy(1).Should().BeTrue();
            sc.IsSatisfiedBy(10).Should().BeTrue();

            sc = new SimpleCardinality(CardinalityEnum.Prohibited);
            sc.IsSatisfiedBy(0).Should().BeTrue();
            sc.IsSatisfiedBy(1).Should().BeFalse();
            sc.IsSatisfiedBy(10).Should().BeFalse();
        }

        [Fact]
        public void MinMaxCardinality_IsValid_Works()
        {
            var sc = new MinMaxCardinality(); // defaults to optional
            sc.IsValid().Should().BeTrue();
            sc = new MinMaxCardinality(1); // minimum one -> required
            sc.IsValid().Should().BeTrue();
            sc = new MinMaxCardinality(0, 0); // maximum 0 -> prohibited
            sc.IsValid().Should().BeTrue();
            sc = new MinMaxCardinality(1, 3); // not convertible
            sc.IsValid().Should().BeTrue();
            sc = new MinMaxCardinality(2, 2); // not convertible
            sc.IsValid().Should().BeTrue();
            sc = new MinMaxCardinality(3, 2); // not valid
            sc.IsValid().Should().BeFalse();
        }

        [Fact]
        public void MinMaxCardinality_IsSatisfiedBy_Works()
        {
            var sc = new MinMaxCardinality(); // defaults to optional
            sc.IsSatisfiedBy(0).Should().BeTrue();
            sc.IsSatisfiedBy(1).Should().BeTrue();
            sc.IsSatisfiedBy(10).Should().BeTrue();

            sc = new MinMaxCardinality(1); // minimum one -> required
            sc.IsSatisfiedBy(0).Should().BeFalse();
            sc.IsSatisfiedBy(1).Should().BeTrue();
            sc.IsSatisfiedBy(10).Should().BeTrue();

            sc = new MinMaxCardinality(0, 0); // maximum 0 -> prohibited
            sc.IsSatisfiedBy(0).Should().BeTrue();
            sc.IsSatisfiedBy(1).Should().BeFalse();
            sc.IsSatisfiedBy(10).Should().BeFalse();

            sc = new MinMaxCardinality(1, 3); // not convertible
            sc.IsSatisfiedBy(0).Should().BeFalse();
            sc.IsSatisfiedBy(1).Should().BeTrue();
            sc.IsSatisfiedBy(3).Should().BeTrue();
            sc.IsSatisfiedBy(4).Should().BeFalse();

            sc = new MinMaxCardinality(2, 2); // not convertible
            sc.IsSatisfiedBy(1).Should().BeFalse();
            sc.IsSatisfiedBy(2).Should().BeTrue();
            sc.IsSatisfiedBy(3).Should().BeFalse();
        }

        [Fact]
        public void MinMaxCardinality_Conversion_Works()
        {
            var sc = new MinMaxCardinality(); // defaults to optional
            sc.Simplify().Should().BeEquivalentTo(new SimpleCardinality(CardinalityEnum.Optional));

            sc = new MinMaxCardinality(1); // minimum one -> required
            sc.Simplify().Should().BeEquivalentTo(new SimpleCardinality(CardinalityEnum.Required));

            sc = new MinMaxCardinality(0, 0); // maximum 0 -> prohibited
            sc.Simplify().Should().BeEquivalentTo(new SimpleCardinality(CardinalityEnum.Prohibited));
            
            sc = new MinMaxCardinality(1, 3); // not convertible
            sc.Simplify().Should().NotBeEquivalentTo(new SimpleCardinality(CardinalityEnum.Optional));
            sc.Simplify().Should().NotBeEquivalentTo(new SimpleCardinality(CardinalityEnum.Required));
            sc.Simplify().Should().NotBeEquivalentTo(new SimpleCardinality(CardinalityEnum.Prohibited));

        }
    }
}
