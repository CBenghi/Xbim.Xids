using FluentAssertions;
using System;
using System.Linq;
using Xbim.InformationSpecifications.Helpers;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.Helpers
{
    public class GeneratedCodeTests
    {
        [Fact]
        public void AttributeHelpersWork()
        {
            var schemas = new[] {
                SchemaInfo.SchemaIfc2x3,
                SchemaInfo.SchemaIfc4,
                SchemaInfo.SchemaIfc4x3,
            };

            foreach (var i in schemas)
            {
                var t = i.GetAttributeRelations("GlobalId").ToArray();
                t.Should().Contain(x => x.Connection == SchemaInfo.ClassAttributeMode.ViaRelationType);
                t.Should().Contain(x => x.Connection == SchemaInfo.ClassAttributeMode.ViaElement);
            }
        }

        [Fact]
        public void IfcMeasure_Helpers_Populated()
        {
            var area = SchemaInfo.IfcMeasures["IfcAreaMeasure"];
            area.Should().NotBeNull();
            area.Exponents.Should().NotBeNull();
            area.Exponents.ToUnitSymbol().Should().Be("m2");

            foreach (var item in Enum.GetValues<IfcValue>())
            {
                SchemaInfo.GetMeasure(item.ToString()).Should().NotBeNull($"{item} is a valid key in the schema.");
            }
        }
    }
}
