using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Xids;

namespace Xbim.IDS.Tests
{
	[TestClass]
	public class ValueContraintTests
	{
		[TestMethod]
		public void ExactContraintSatisfactionTest()
		{
			var vc = new ValueConstraint(TypeName.String);
			vc.AddAccepted(new ExactConstraint("2"));
			
			Assert.IsTrue(vc.IsSatisfiedBy("2"));
			Assert.IsFalse(vc.IsSatisfiedBy("1"));
			Assert.IsFalse(vc.IsSatisfiedBy(1));
			Assert.IsFalse(vc.IsSatisfiedBy(2));
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

			Assert.IsFalse(vc.IsSatisfiedBy(1));
			Assert.IsTrue(vc.IsSatisfiedBy(2));
			Assert.IsTrue(vc.IsSatisfiedBy(2.01));
			Assert.IsTrue(vc.IsSatisfiedBy(3.99));
			Assert.IsTrue(vc.IsSatisfiedBy(4));
			Assert.IsFalse(vc.IsSatisfiedBy(4.01));

			t.MinInclusive = false;
			t.MaxInclusive = false;

			Assert.IsFalse(vc.IsSatisfiedBy(1));
			Assert.IsFalse(vc.IsSatisfiedBy(2));
			Assert.IsTrue(vc.IsSatisfiedBy(2.01f));
			Assert.IsTrue(vc.IsSatisfiedBy(3.99f));
			Assert.IsFalse(vc.IsSatisfiedBy(4m));
			Assert.IsFalse(vc.IsSatisfiedBy(4.01));

		}
	}
}
