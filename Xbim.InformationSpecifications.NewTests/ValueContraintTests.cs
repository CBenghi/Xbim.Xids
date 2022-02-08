using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Xbim.InformationSpecifications.Tests
{
	
	public class ValueContraintTests
	{

		[Fact]
		public void IPersistValues()
        {
			var str = "2O2Fr$t4X7Zf8NOew3FLOH";
            Ifc2x3.UtilityResource.IfcGloballyUniqueId i = new Ifc2x3.UtilityResource.IfcGloballyUniqueId(str);
			var vc = new ValueConstraint(str);
			vc.IsSatisfiedBy(i).Should().BeTrue();
			

			var str2 = "2O2Fr$t4X7Zf8NOew3FLOh";
			var vc2 = new ValueConstraint(str2);
			vc2.IsSatisfiedBy(i).Should().BeFalse();
			
		}

		[Fact]
		public void ExactContraintSatisfactionTest()
		{
			var vc = new ValueConstraint(TypeName.String, "2");
			vc.IsSatisfiedBy("2").Should().BeTrue();
			vc.IsSatisfiedBy("1").Should().BeFalse();
			vc.IsSatisfiedBy(1).Should().BeFalse();
			vc.IsSatisfiedBy(2).Should().BeTrue(); // conversion ToString is valid match

			vc = new ValueConstraint(375.230);
			vc.IsSatisfiedBy(375.23).Should().BeTrue();
			vc.IsSatisfiedBy(375.230000).Should().BeTrue();
			vc.IsSatisfiedBy(375.230001).Should().BeFalse();

			vc = new ValueConstraint(375.230m);
			vc.IsSatisfiedBy(375.230m).Should().BeTrue();
			vc.IsSatisfiedBy(375.23m).Should().BeFalse();


			vc = new ValueConstraint("red");
			vc.IsSatisfiedBy("red").Should().BeTrue();
			vc.IsSatisfiedBy("blue").Should().BeFalse();

			vc = new ValueConstraint(TypeName.Floating);
			vc.IsSatisfiedBy(2f).Should().BeTrue();
			vc.IsSatisfiedBy("blue").Should().BeFalse();
			vc.IsSatisfiedBy(2d).Should().BeFalse();
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

		[Fact]
		public void RangeConstraintSatisfactionTest()
		{
			var vc = new ValueConstraint(TypeName.Double);
			var t = new RangeConstraint()
			{
				MinValue = 2.ToString(),
				MinInclusive = true,
				MaxValue = 4.ToString(),
				MaxInclusive = true,
			};
			vc.AddAccepted(t);

			vc.IsSatisfiedBy(1d).Should().BeFalse();
			vc.IsSatisfiedBy(2d).Should().BeTrue();
			vc.IsSatisfiedBy(2.01).Should().BeTrue();
			vc.IsSatisfiedBy(3.99).Should().BeTrue();
			vc.IsSatisfiedBy(4d).Should().BeTrue();
			vc.IsSatisfiedBy(4.01).Should().BeFalse();

			t.MinInclusive = false;
			t.MaxInclusive = false;

			vc.IsSatisfiedBy(1d).Should().BeFalse();
			vc.IsSatisfiedBy(2d).Should().BeFalse();
			vc.IsSatisfiedBy(2.01d).Should().BeTrue();
			vc.IsSatisfiedBy(3.99d).Should().BeTrue();
			vc.IsSatisfiedBy(4d).Should().BeFalse();
			vc.IsSatisfiedBy(4.01d).Should().BeFalse();
		}

		[Fact]
		public void EnumConstraintSatisfactionTest()
		{
			var vc = new ValueConstraint(TypeName.Integer);
			vc.AddAccepted(new ExactConstraint(30.ToString()));
			vc.AddAccepted(new ExactConstraint(60.ToString()));
			vc.AddAccepted(new ExactConstraint(90.ToString()));

			vc.IsSatisfiedBy(1d).Should().BeFalse("1d failure");
			vc.IsSatisfiedBy(30L).Should().BeTrue("30L failure");
			vc.IsSatisfiedBy(60).Should().BeTrue("60 failure");
			vc.IsSatisfiedBy(60L).Should().BeTrue("60L failure");
			
		}
	}
}
