using FluentAssertions;
using System.IO;
using System.Linq;
using Xbim.InformationSpecifications.Cardinality;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.DescriptionTests
{
    public class IfcTypeDescriptionTests : BaseDescriptionTests
    {
        [InlineData("IFCWALL", "PARTITIONING", "of entity Wall and of predefined type Partitioning")]
        [InlineData("IFCWALL", null, "of entity Wall and of predefined type <any>")]
        [InlineData("IFCWALL", "", "of entity Wall and of predefined type <any>")]
        [InlineData(null, null, "of entity <any> and of predefined type <any>")]
        [InlineData("IFCWALL,IFCWALLSTANDARDCASE", null, "of entity Wall or Wallstandardcase and of predefined type <any>")]

        [Theory]
        public void IfcTypeFacetsApplicabilityDescribed(string? ifcType, string? predefined, string expected,
            ConstraintType typeConstraint = ConstraintType.Exact)
        {
            var facet = BuildFacetFromInputs(ifcType, predefined, typeConstraint: typeConstraint);
            facet.ApplicabilityDescription.Should().Be(expected);
        }

        [InlineData("IFCWALL", "PARTITIONING", "an entity Wall and of predefined type Partitioning")]
        [InlineData("IFCWALL", null, "an entity Wall and of predefined type <any>")]
        [InlineData(null, null, "an entity <any> and of predefined type <any>")]
        [InlineData("IFCWALL,IFCWALLSTANDARDCASE", null, "an entity Wall or Wallstandardcase and of predefined type <any>")]

        [Theory]
        public void IfcTypeFacetsRequirementDescribed(string? ifcType, string? predefined, string expected,
            ConstraintType typeConstraint = ConstraintType.Exact)
        {
            var facet = BuildFacetFromInputs(ifcType, predefined, typeConstraint: typeConstraint);



            facet.RequirementDescription.Should().Be(expected);
        }


        [Fact]
        public void CanDescribeIfcTypeWithImplicitCardinality()
        {
            //pass-a_matching_entity_should_pass

            var file = new FileInfo(@"bsFiles/others/pass-a_matching_entity_should_pass.ids");
            
            Xids.CanLoad(file).Should().BeTrue("Should be able to load");
            var x = Xids.Load(file);
            x.Should().NotBeNull();

            var firstSpec = x!.AllSpecifications().First();
            firstSpec.Cardinality.Should().BeEquivalentTo(new SimpleCardinality(CardinalityEnum.Required));


            firstSpec.Applicability.GetApplicabilityDescription().Should().Be("All elements of entity Wall and of predefined type <any>");
        }

        private static IfcTypeFacet BuildFacetFromInputs(string? ifcTypeInputs, string? predefined = "", 
            ConstraintType typeConstraint = ConstraintType.Exact, ConstraintType predefinedConstraint = ConstraintType.Exact)
        {
            IfcTypeFacet facet = new()
            {
                IfcType = new ValueConstraint(NetTypeName.String),
                PredefinedType = new ValueConstraint(NetTypeName.String),
            };
            if(ifcTypeInputs != null)
            {
                var names = ifcTypeInputs.Split(',');
                foreach (var name in names)
                {
                    AddConstraint(facet.IfcType, typeConstraint, name);
                }
            }
            else
            {
                facet.IfcType.BaseType = NetTypeName.Undefined;
            }
            if (predefined == null)   // No value
            {
                facet.PredefinedType = null;
                //facet.PredefinedType.BaseType = NetTypeName.Undefined;
                return facet;
            }
            
            var predef = predefined.Split(','); // String or strings
            foreach (var val in predef)
            {
                AddConstraint(facet.PredefinedType, predefinedConstraint, val);
            }
            return facet;
        }

    }
}
