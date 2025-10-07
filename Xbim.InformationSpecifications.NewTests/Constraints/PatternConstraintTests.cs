#pragma warning disable xUnit1042
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Xbim.InformationSpecifications.Tests
{
	public class PatternConstraintTests
	{
		[Theory]
		[MemberData(nameof(GetExamplePatterns))]
		public void CanProcessExamplePatterns(string pattern, string[] expectTrue, string[] expectFalse)
		{
			var vc = ValueConstraint.CreatePattern(pattern);

			foreach (var val in expectTrue)
				vc.IsSatisfiedBy(val).Should().BeTrue();
			foreach (var val in expectFalse)
				vc.IsSatisfiedBy(val).Should().BeFalse();
		}

		public static IEnumerable<object[]> GetExamplePatterns()
		{
			yield return new object[]
			{
				"(RAL).*",
				new string[] {"RAL"}, // true
				new string[] {"noRAL"}, // false
			};
			yield return new object[]
			{
				@"3[1-2]\.[0-9][0-9]",
				new string[] {"31.12"}, // true
				new string[] {"31.12RAL"}, // false
			};
			yield return new object[]
			{
				".*(glas).*",
				new string[] {"glass"}, // true
				System.Array.Empty<string>(), // false
			};

		}

		[Fact]
		public void FailLoggerTest()
		{
			var loggerMock = Substitute.For<ILogger<PatternConstraintTests>>();
			var vc = new ValueConstraint(NetTypeName.String);
			vc.AddAccepted(new PatternConstraint() { Pattern = "(invalid" });
			vc.IsSatisfiedBy("a", loggerMock).Should().BeFalse();
			var errorAndWarnings = loggerMock.ReceivedCalls().Where(call => call.IsErrorType(true, true, false));
			errorAndWarnings.Should().NotBeEmpty("we are passing an invalid pattern");
		}


		[InlineData("123", true)]
		[InlineData("n/a", true)]
		[InlineData("BAD-123", false)]
		[InlineData("B123", false)]
		[InlineData("0001234A", false)]
		[Theory]
		public void PatternImplicitlyIncludesStartEndWhenUsingRegexOrTest(string input, bool expectedResult)
		{
			var vc = new ValueConstraint();
			vc.AddAccepted(new PatternConstraint() { Pattern = @"n\/a|(\d)+" });

			vc.IsSatisfiedBy(input).Should().Be(expectedResult);
		}


		[Fact]
		public void PatternConstraintSatisfactionTest()
		{
			var vc = new ValueConstraint(NetTypeName.String);
			vc.AddAccepted(new PatternConstraint() { Pattern = "[a-z]" });
			vc.IsSatisfiedBy("a").Should().BeTrue();
			vc.IsSatisfiedBy("z").Should().BeTrue();
			vc.IsSatisfiedBy("A").Should().BeFalse();
			vc.IsSatisfiedBy("Z").Should().BeFalse();
		}


		[Fact]
		public void MultiplePatternConstraintsSupported()
		{
			var vc = new ValueConstraint(NetTypeName.String);
			vc.AddAccepted(new PatternConstraint() { Pattern = "[a-d]" });
			vc.AddAccepted(new PatternConstraint() { Pattern = "[d-f]" });
			vc.IsSatisfiedBy("a").Should().BeTrue();
			vc.IsSatisfiedBy("d").Should().BeTrue();
			vc.IsSatisfiedBy("e").Should().BeTrue();
			vc.IsSatisfiedBy("g").Should().BeFalse();
			vc.IsSatisfiedBy("Z").Should().BeFalse();
		}

	}
}
