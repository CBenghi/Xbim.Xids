using FluentAssertions;
using System;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.DescriptionTests
{
    public class AttributeFacetDescriptionTests: BaseDescriptionTests
    {
        [InlineData("FieldName", "123", "an attribute FieldName with value 123")]
        [InlineData("FieldName", "123.*", "an attribute FieldName with value matching '123.*'", ConstraintType.Pattern)]
        [InlineData("Easting", 56.78, "an attribute Easting with value 56.78")]
        [InlineData("Name", null, "an attribute Name with value <any>")]
        [InlineData("Status", "NOTSTARTED,STARTED,COMPLETED", "an attribute Status with value NOTSTARTED or STARTED or COMPLETED")]
        [InlineData("Name,Description", null, "an attribute Name or Description with value <any>")]
        [InlineData("Name,Description", "P.*", "an attribute Name or Description with value matching 'P.*'", ConstraintType.Pattern)]
        [InlineData("Name,Description", "Foo", "an attribute Name or Description with value Foo")]
        [Theory]
        public void AttributeFacetsRequirementsDescribed(string attributeName, object? attributeValue, string expected,
            ConstraintType valueConstraint = ConstraintType.Exact)
        {
            AttributeFacet facet;
            if (attributeValue is string strValue)
            {

                facet = BuildFacetFromInputs(attributeName, strValue, valueConstraint: valueConstraint);
            }
            else
            {
                facet = BuildFacetFromInputs(attributeName, objInputs:attributeValue, valueConstraint: valueConstraint);
            }

            facet.RequirementDescription.Should().Be(expected);
        }

        [InlineData("FieldName", "123", "with attribute FieldName with value 123")]
        [InlineData("FieldName", "123.*", "with attribute FieldName with value matching '123.*'", ConstraintType.Pattern)]
        [InlineData("Easting", 56.78, "with attribute Easting with value 56.78")]
        [InlineData("Name", null, "with attribute Name with value <any>")]
        [InlineData("Status", "NOTSTARTED,STARTED,COMPLETED", "with attribute Status with value NOTSTARTED or STARTED or COMPLETED")]
        [InlineData("Name,Description", null, "with attribute Name or Description with value <any>")]
        [InlineData("Name,Description", "P.*", "with attribute Name or Description with value matching 'P.*'", ConstraintType.Pattern)]
        [InlineData("Name,Description", "Foo", "with attribute Name or Description with value Foo")]
        [Theory]
        public void AttributeFacetsApplicabilityDescribed(string attributeName, object? attributeValue, string expected,
            ConstraintType valueConstraint = ConstraintType.Exact)
        {
            AttributeFacet facet;
            if (attributeValue is string strValue)
            {

                facet = BuildFacetFromInputs(attributeName, strValue, valueConstraint: valueConstraint);
            }
            else
            {
                facet = BuildFacetFromInputs(attributeName, objInputs: attributeValue, valueConstraint: valueConstraint);
            }

            facet.ApplicabilityDescription.Should().Be(expected);
        }

        [Fact]
        public void RangeAttributeFacetsWork()
        {
            var facet = BuildFacetFromInputs("Northing", null);
            if (facet.AttributeValue is null)
                throw new Exception();
            facet.AttributeValue.AddAccepted(new RangeConstraint("0", true, "360", false));
            facet.RequirementDescription.Should().Be("an attribute Northing with value >=0 and <360");
        }

        [Fact]
        public void StructureAttributeFacetsWork()
        {
            var facet = BuildFacetFromInputs("Tag", null);
            var constraint = new StructureConstraint()
            {
                MaxLength=10,
                MinLength=4
            };
			if (facet.AttributeValue is null)
				throw new Exception();
			facet.AttributeValue.AddAccepted(constraint);
            facet.RequirementDescription.Should().Be("an attribute Tag with value minimum 4 characters and maximum 10 characters");
        }


        private static AttributeFacet BuildFacetFromInputs(string nameInputs, string? valueInputs = "", object? objInputs = null,
            ConstraintType nameConstraint = ConstraintType.Exact, ConstraintType valueConstraint = ConstraintType.Exact)
        {
            AttributeFacet facet = new()
            {
                AttributeName = new ValueConstraint(NetTypeName.String),
                AttributeValue = new ValueConstraint(NetTypeName.String),
            };

            var names = nameInputs.Split(',');
            foreach (var name in names)
            {
                AddConstraint(facet.AttributeName, nameConstraint, name);
            }
            if (valueInputs == null && objInputs == null)   // No value
                return facet;
            if(objInputs is not null)   // Non string
            {
                facet.AttributeValue.BaseType = NetTypeName.Double;
                var val = objInputs.ToString() ?? throw new Exception("Invalid string conversion.");
				AddConstraint(facet.AttributeValue, valueConstraint, val);
                return facet;
            }
            var values = valueInputs!.Split(','); // String or strings
            foreach (var val in values)
            {
                AddConstraint(facet.AttributeValue, valueConstraint, val);
            }
            return facet;
        }

        
    }

}
