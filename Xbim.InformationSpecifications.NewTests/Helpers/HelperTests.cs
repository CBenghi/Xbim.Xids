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
            };

            foreach (var i in schemas)
            {
                var t = i.GetAttributeRelations("GlobalId").ToArray();
                t.Should().Contain(x => x.Connection == SchemaInfo.ClassAttributeMode.ViaRelationType);
                t.Should().Contain(x => x.Connection == SchemaInfo.ClassAttributeMode.ViaElement);
            }            
        }

    }
}
