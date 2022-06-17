using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
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
				"(RAL)*",
				new string[] {"RAL"}, // true
				new string[] {"noRAL"}, // false
			};
			yield return new object[]
			{
				"3[1-2].[0-9][0-9]",
				new string[] {"31.12"}, // true
				new string[] {"31.12RAL"}, // false
			};
			yield return new object[]
			{
				"*(glas)*",
				new string[] {"glass"}, // true
				System.Array.Empty<string>(), // false
			};

		}

		[Fact]
		public void FailLoggerTest()
        {
			var loggerMock = new Mock<ILogger<PatternConstraintTests>>();
			var vc = new ValueConstraint(TypeName.String);
			vc.AddAccepted(new PatternConstraint() { Pattern = "(invalid" });
			vc.IsSatisfiedBy("a", loggerMock.Object).Should().BeFalse();
			var loggingCalls = loggerMock.Invocations.Select(x => x.ToString()).ToArray(); // this creates the array of logging calls
			loggingCalls.Where(x => x.Contains("Error")).Should().NotBeEmpty("we are passing an invalid pattern");
		}


        [Fact]
		public void PatternConstraintSatisfactionTest()
		{
			var vc = new ValueConstraint(TypeName.String);
			vc.AddAccepted(new PatternConstraint() { Pattern = "[a-z]" });
			vc.IsSatisfiedBy("a").Should().BeTrue();
			vc.IsSatisfiedBy("z").Should().BeTrue();
			vc.IsSatisfiedBy("A").Should().BeFalse();
			vc.IsSatisfiedBy("Z").Should().BeFalse();
		}

	}
}
