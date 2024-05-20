using FluentAssertions;
using System;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.DescriptionTests
{
    public class FacetGroupDescriptionTests
    {
        [InlineData(null,
            "All elements of entity IfcDoor and of predefined type Door AND with attribute Name with value D001")]
        [InlineData(new[] { RequirementCardinalityOptions.Cardinality.Expected, RequirementCardinalityOptions.Cardinality.Expected },
            "All elements of entity IfcDoor and of predefined type Door AND with attribute Name with value D001")]
        //[InlineData(new[] { RequirementCardinalityOptions.Cardinality.Expected, RequirementCardinalityOptions.Cardinality.Prohibited },
        //    "All elements of entity IfcDoor and of predefined type Door AND NOT with attribute Name with value D001")]
        //[InlineData(new[] { RequirementCardinalityOptions.Cardinality.Prohibited, RequirementCardinalityOptions.Cardinality.Optional },
        //    "All elements NOT of entity IfcDoor and of predefined type Door AND OPTIONALLY with attribute Name with value D001")]
        [Theory]
        [Obsolete("Because we use an constructor method mark as obsolete for testing, we prevent the warning by marking the test obsolete too.")]
        public void FacetGroupApplicabilityDescribed(RequirementCardinalityOptions.Cardinality[]? reqs, string expected)
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

			var options = new System.Collections.ObjectModel.ObservableCollection<RequirementCardinalityOptions>();
			for (int i = 0; i < facetGroup.Facets.Count; i++)
            {
				IFacet? facet = facetGroup.Facets[i];
                var t = reqs?[i] ?? RequirementCardinalityOptions.DefaultCardinality;
				var thisOpt = new RequirementCardinalityOptions(facet, t);
                options.Add(thisOpt);
			}
            if (reqs != null)
                facetGroup.RequirementOptions = options;


			facetGroup.GetApplicabilityDescription().Should().Be(expected);
        }

        [InlineData(null,
            "should have an entity IfcDoor and of predefined type Door AND should have an attribute Name with value D001")]
        [InlineData(new[] { RequirementCardinalityOptions.Cardinality.Expected, RequirementCardinalityOptions.Cardinality.Expected },
            "should have an entity IfcDoor and of predefined type Door AND should have an attribute Name with value D001")]
        [InlineData(new[] { RequirementCardinalityOptions.Cardinality.Expected, RequirementCardinalityOptions.Cardinality.Prohibited },
            "should have an entity IfcDoor and of predefined type Door AND should NOT have an attribute Name with value D001")]
        [InlineData(new[] { RequirementCardinalityOptions.Cardinality.Prohibited, RequirementCardinalityOptions.Cardinality.Optional },
            "should NOT have an entity IfcDoor and of predefined type Door AND should OPTIONALLY have an attribute Name with value D001")]
        [Theory]
        [Obsolete("Because we use an constructor method mark as obsolete for testing, we prevent the warning by marking the test obsolete too.")]
        public void FacetGroupRequirementDescribed(RequirementCardinalityOptions.Cardinality[]? reqs, string expected)
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

			var options = new System.Collections.ObjectModel.ObservableCollection<RequirementCardinalityOptions>();
			for (int i = 0; i < facetGroup.Facets.Count; i++)
			{
				IFacet? facet = facetGroup.Facets[i];
				var t = reqs?[i] ?? RequirementCardinalityOptions.Cardinality.Expected;
				var thisOpt = new RequirementCardinalityOptions(facet, t);
				options.Add(thisOpt);
			}
			if (reqs != null)
				facetGroup.RequirementOptions = options;

			facetGroup.GetRequirementDescription().Should().Be(expected);
        }
    }
}
