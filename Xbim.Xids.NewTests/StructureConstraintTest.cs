using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Xbim.Xids.NewTests
{
	public class StructureConstraintTests
	{

		[Fact]
		public void StructureConstraintSatisfactionTest()
		{
			var vc = new ValueConstraint(TypeName.Decimal);
			var sc = new StructureConstraint();
			sc.FractionDigits = 2;
			sc.TotalDigits = 5;
			vc.AddAccepted(sc);

			// basic value
			vc.IsSatisfiedBy(123.12m).Should().BeTrue();

			// decimal fails
			vc.IsSatisfiedBy(1234.1m).Should().BeFalse("because too few decimals");
			vc.IsSatisfiedBy(14.133m).Should().BeFalse("because too many decimals");

			// total len fails
			vc.IsSatisfiedBy(1234.21m).Should().BeFalse("because too many digits");
			vc.IsSatisfiedBy(14.13m).Should().BeFalse("because too few digits");

			// long cases
			sc.TotalDigits = 14;
			vc.IsSatisfiedBy(214748364700.13m).Should().BeTrue();			
			vc.IsSatisfiedBy(-214748364700.13m).Should().BeTrue(); // negative too

			sc.FractionDigits = 0;
			vc.IsSatisfiedBy(21474836470013m).Should().BeTrue(); // positive
			vc.IsSatisfiedBy(-21474836470013m).Should().BeTrue(); // negative too

			vc = new ValueConstraint(TypeName.Floating);
			sc = new StructureConstraint();
			vc.AddAccepted(sc);
			sc.FractionDigits = 2;
			sc.TotalDigits = 5;
			vc.IsSatisfiedBy(324.75f).Should().BeTrue(); // positive
			vc.IsSatisfiedBy(-324.75f).Should().BeTrue(); // negative too

			vc.IsSatisfiedBy(32.754f).Should().BeFalse(); // positive
			vc.IsSatisfiedBy(-32.754f).Should().BeFalse(); // negative too

			vc.IsSatisfiedBy(324.753f).Should().BeFalse(); // positive
			vc.IsSatisfiedBy(-324.753f).Should().BeFalse(); // negative too

			vc = new ValueConstraint(TypeName.Double);
			sc = new StructureConstraint();
			vc.AddAccepted(sc);
			sc.FractionDigits = 2;
			sc.TotalDigits = 5;
			vc.IsSatisfiedBy(324.75).Should().BeTrue(); // positive
			vc.IsSatisfiedBy(-324.75).Should().BeTrue(); // negative too

			vc.IsSatisfiedBy(32.754).Should().BeFalse(); // positive
			vc.IsSatisfiedBy(-32.754).Should().BeFalse(); // negative too

			vc.IsSatisfiedBy(324.753).Should().BeFalse(); // positive
			vc.IsSatisfiedBy(-324.753).Should().BeFalse(); // negative too

			vc = new ValueConstraint(TypeName.String);
			sc = new StructureConstraint();
			vc.AddAccepted(sc);
			sc.Length = 4;
			vc.IsSatisfiedBy("12345").Should().BeFalse(); 
			vc.IsSatisfiedBy("1234").Should().BeTrue();
			sc.Length = null;

			sc.MinLength = 3;
			sc.MaxLength = 5;

			vc.IsSatisfiedBy("12").Should().BeFalse();
			vc.IsSatisfiedBy("123").Should().BeTrue();
			vc.IsSatisfiedBy("1234").Should().BeTrue();
			vc.IsSatisfiedBy("12345").Should().BeTrue();
			vc.IsSatisfiedBy("123456").Should().BeFalse();

		}
	}
}
