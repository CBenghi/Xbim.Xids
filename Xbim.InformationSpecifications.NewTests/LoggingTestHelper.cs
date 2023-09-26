using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.InformationSpecifications.Tests
{
    internal class LoggingTestHelper
    {
        internal static void NoIssues<T>(ILogger<T> loggerMock)
		{
            var loggingCalls = loggerMock.ReceivedCalls().Select(x => GetFirstArg(x)).ToArray(); // this creates the array of logging calls
            loggingCalls.Where(call => call is not null &&
                (
                    call == "Error"
                    || call == "Warning"
                    || call == "Critical"
                )
                ).Should().BeEmpty("no calls to errors or warnings are expected");
        }

		private static string GetFirstArg(ICall x)
		{
			var first = x.GetOriginalArguments().FirstOrDefault();
			if (first != null)
				return first.ToString() ?? "";
			return "<null>";
		}

		internal static void SomeIssues<T>(ILogger<T> loggerMock)
        {
            var loggingCalls = loggerMock.ReceivedCalls().Select(x => GetFirstArg(x)).ToArray(); // this creates the array of logging calls
            loggingCalls.Where(call => call is not null &&
                (
                    call == "Error"
                    || call == "Warning"
                    || call == "Critical"
                )
                ).Should().NotBeEmpty("some calls to errors or warnings are expected");
        }
    }
}
