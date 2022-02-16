using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.InformationSpecifications.Helpers;
using Xunit;

namespace Xbim.InformationSpecifications.NewTests.Helpers
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
