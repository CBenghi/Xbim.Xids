using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.InformationSpecifications.Helpers;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.Helpers
{
    public class StringHelpersTests
    {
        [Fact]
        public void CapitalisationHelpersWorkAsExpected()
        {
            StringExtensions.FirstCharToUpper("some").Should().Be("Some");
            StringExtensions.FirstCharToUpper("Some").Should().Be("Some");
            StringExtensions.FirstCharToUpper("o").Should().Be("O");
            Assert.Throws<ArgumentException>(() => StringExtensions.FirstCharToUpper(""));
            Assert.Throws<ArgumentNullException>(() => StringExtensions.FirstCharToUpper(null));
        }
    }
}
