﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Isam.Esent.Interop;
using NSubstitute;
using NSubstitute.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Xbim.InformationSpecifications.Tests
{
	internal static class LoggingTestHelper
	{
		internal static ILogger<T> GetXunitLogger<T>(ITestOutputHelper OutputHelper)
		{
			var services = new ServiceCollection()
						.AddLogging((builder) => builder.AddXUnit(OutputHelper));
			IServiceProvider provider = services.BuildServiceProvider();
			var logg = provider.GetRequiredService<ILogger<T>>();
			Assert.NotNull(logg);
			return logg;
		}

		internal static void NoIssues<T>(ILogger<T> loggerMock)
		{
			loggerMock.ReceivedCalls().Where(call => call.IsErrorType(true, true, true))
				.Should().BeEmpty("no calls to errors or warnings are expected");
		}

		internal static bool IsErrorType(this ICall x, bool error, bool warning, bool critical)
		{
			var str = x.GetFirstArgument();
			return str switch
			{
				"Error" => error,
				"Warning" => warning,
				"Critical" => critical,
				_ => false,
			};
		}

		internal static string GetFirstArgument(this ICall x)
		{
			var first = x.GetOriginalArguments().FirstOrDefault();
			if (first != null)
				return first.ToString() ?? "";
			return "<null>";
		}

		internal static void SomeIssues<T>(ILogger<T> loggerMock)
		{
			var loggingCalls = loggerMock.ReceivedCalls().Where(call => call.IsErrorType(true, true, true)).Should().NotBeEmpty("some calls to errors or warnings are expected");
		}
	}
}
