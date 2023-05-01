using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.InformationSpecifications.Tests
{
    internal class LoggingTestHelper
    {
        internal static void NoIssues<T>(Mock<ILogger<T>> loggerMock)
        {
            var loggingCalls = loggerMock.Invocations.Select(x => x.ToString()).ToArray(); // this creates the array of logging calls
            loggingCalls.Where(call => call is not null &&
                (
                    call.Contains("Error") 
                    || call.Contains("Warning")
                    || call.Contains("Critical")
                )
                ).Should().BeEmpty("no calls to errors or warnings are expected");
        }

        internal static void SomeIssues(Mock<ILogger<BuildingSmartCompatibilityTests>> loggerMock)
        {
            var loggingCalls = loggerMock.Invocations.Select(x => x.ToString()).ToArray(); // this creates the array of logging calls
            loggingCalls.Where(call => call is not null &&
                (
                    call.Contains("Error")
                    || call.Contains("Warning")
                    || call.Contains("Critical")
                )
                ).Should().NotBeEmpty("some calls to errors or warnings are expected");
        }
    }
}
