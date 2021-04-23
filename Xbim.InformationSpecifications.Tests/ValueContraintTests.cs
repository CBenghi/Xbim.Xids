using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.InformationSpecifications.Tests
{
	[TestClass]
	public class ValueContraintTests
	{
		[TestMethod]
		public void ExactContraintSatisfactionTest()
		{
			var vc = new ValueConstraint(TypeName.String, "2");
			Assert.IsTrue(vc.IsSatisfiedBy("2"));
			Assert.IsFalse(vc.IsSatisfiedBy("1"));
			Assert.IsFalse(vc.IsSatisfiedBy(1));
			Assert.IsFalse(vc.IsSatisfiedBy(2)); // conversion ToString is valid match

			vc = new ValueConstraint(375.230);
			Assert.IsTrue(vc.IsSatisfiedBy(375.23));
			Assert.IsTrue(vc.IsSatisfiedBy(375.230000));
			Assert.IsTrue(vc.IsSatisfiedBy(375.230000));

			vc = new ValueConstraint(375.230m);
			Assert.IsTrue(vc.IsSatisfiedBy(375.230m));
			Assert.IsFalse(vc.IsSatisfiedBy(375.23m));


			vc = new ValueConstraint("red");
			Assert.IsTrue(vc.IsSatisfiedBy("red"));
			Assert.IsFalse(vc.IsSatisfiedBy("blue"));

			vc = new ValueConstraint(TypeName.Floating);
			Assert.IsTrue(vc.IsSatisfiedBy(2f));
			Assert.IsFalse(vc.IsSatisfiedBy("blue"));
			Assert.IsFalse(vc.IsSatisfiedBy(2d));
		}

		[TestMethod]
		public void PatternConstraintSatisfactionTest()
		{
			var vc = new ValueConstraint(TypeName.String);
			vc.AddAccepted(new PatternConstraint() { Pattern = "[a-z]" });
			Assert.IsTrue(vc.IsSatisfiedBy("a"));
			Assert.IsTrue(vc.IsSatisfiedBy("z"));
			Assert.IsFalse(vc.IsSatisfiedBy("A"));
			Assert.IsFalse(vc.IsSatisfiedBy("Z"));
		}

		[TestMethod]
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

			Assert.IsFalse(vc.IsSatisfiedBy(1d));
			Assert.IsTrue(vc.IsSatisfiedBy(2d));
			Assert.IsTrue(vc.IsSatisfiedBy(2.01));
			Assert.IsTrue(vc.IsSatisfiedBy(3.99));
			Assert.IsTrue(vc.IsSatisfiedBy(4d));
			Assert.IsFalse(vc.IsSatisfiedBy(4.01));

			t.MinInclusive = false;
			t.MaxInclusive = false;

			Assert.IsFalse(vc.IsSatisfiedBy(1d));
			Assert.IsFalse(vc.IsSatisfiedBy(2d));
			Assert.IsTrue(vc.IsSatisfiedBy(2.01d));
			Assert.IsTrue(vc.IsSatisfiedBy(3.99d));
			Assert.IsFalse(vc.IsSatisfiedBy(4d));
			Assert.IsFalse(vc.IsSatisfiedBy(4.01d));
		}

		[TestMethod]
		public void EnumConstraintSatisfactionTest()
		{
			var vc = new ValueConstraint(TypeName.Integer);
			vc.AddAccepted(new ExactConstraint(30.ToString()));
			vc.AddAccepted(new ExactConstraint(60.ToString()));
			vc.AddAccepted(new ExactConstraint(90.ToString()));

			Assert.IsFalse(vc.IsSatisfiedBy(1d));
			Assert.IsTrue(vc.IsSatisfiedBy(30L));
			Assert.IsTrue(vc.IsSatisfiedBy(60));
			Assert.IsTrue(vc.IsSatisfiedBy(60L));
			
		}
	}
}
