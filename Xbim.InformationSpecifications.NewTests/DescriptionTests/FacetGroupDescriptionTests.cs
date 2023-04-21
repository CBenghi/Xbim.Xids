using FluentAssertions;
using System;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.DescriptionTests
{
    public class FacetGroupDescriptionTests
    {
        [InlineData(null,
            "All elements of entity IfcDoor and of predefined type Door AND with attribute Name with value D001")]
        [InlineData(new[] { RequirementCardinalityOptions.Expected, RequirementCardinalityOptions.Expected },
            "All elements of entity IfcDoor and of predefined type Door AND with attribute Name with value D001")]
        [InlineData(new[] { RequirementCardinalityOptions.Expected, RequirementCardinalityOptions.Prohibited },
            "All elements of entity IfcDoor and of predefined type Door AND NOT with attribute Name with value D001")]
        [InlineData(new[] { RequirementCardinalityOptions.Prohibited, RequirementCardinalityOptions.Optional },
            "All elements NOT of entity IfcDoor and of predefined type Door AND OPTIONALLY with attribute Name with value D001")]
        [Theory]
        [Obsolete("Because we use an constructor method mark as obsolete for testing, we prevent the warning by marking the test obsolete too.")]
        public void FacetGroupApplicabilityDescribed(RequirementCardinalityOptions[] reqs, string expected)
        {

            var type = new IfcTypeFacet
            {
                IfcType = new ValueConstraint("IfcDoor"),
                PredefinedType = new ValueConstraint("Door")
            };
            var attr = new AttributeFacet
            {
                AttributeName = new ValueConstraint("Name"),
                AttributeValue = new ValueConstraint("D001"),
            };

            var facetGroup = new FacetGroup();
            facetGroup.Facets.Add(type);
            facetGroup.Facets.Add(attr);

            if(reqs != null)
                facetGroup.RequirementOptions = new System.Collections.ObjectModel.ObservableCollection<RequirementCardinalityOptions>(reqs);

            facetGroup.GetApplicabilityDescription().Should().Be(expected);
        }

        [InlineData(null,
            "should have an entity IfcDoor and of predefined type Door AND should have an attribute Name with value D001")]
        [InlineData(new[] { RequirementCardinalityOptions.Expected, RequirementCardinalityOptions.Expected },
            "should have an entity IfcDoor and of predefined type Door AND should have an attribute Name with value D001")]
        [InlineData(new[] { RequirementCardinalityOptions.Expected, RequirementCardinalityOptions.Prohibited },
            "should have an entity IfcDoor and of predefined type Door AND should NOT have an attribute Name with value D001")]
        [InlineData(new[] { RequirementCardinalityOptions.Prohibited, RequirementCardinalityOptions.Optional },
            "should NOT have an entity IfcDoor and of predefined type Door AND should OPTIONALLY have an attribute Name with value D001")]
        [Theory]
        [Obsolete("Because we use an constructor method mark as obsolete for testing, we prevent the warning by marking the test obsolete too.")]
        public void FacetGroupRequirementDescribed(RequirementCardinalityOptions[] reqs, string expected)
        {

            var type = new IfcTypeFacet
            {
                IfcType = new ValueConstraint("IfcDoor"),
                PredefinedType = new ValueConstraint("Door")
            };
            var attr = new AttributeFacet
            {
                AttributeName = new ValueConstraint("Name"),
                AttributeValue = new ValueConstraint("D001"),
            };

            var facetGroup = new FacetGroup();
            facetGroup.Facets.Add(type);
            facetGroup.Facets.Add(attr);

            if (reqs != null)
                facetGroup.RequirementOptions = new System.Collections.ObjectModel.ObservableCollection<RequirementCardinalityOptions>(reqs);

            facetGroup.GetRequirementDescription().Should().Be(expected);
        }
    }
}
