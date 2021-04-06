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
			var t = new ExactConstraint("2");
			Assert.IsTrue(t.IsSatisfiedBy("2"));
			Assert.IsFalse(t.IsSatisfiedBy("1"));
			Assert.IsFalse(t.IsSatisfiedBy(1));
			Assert.IsFalse(t.IsSatisfiedBy(2));
		}

		[TestMethod]
		public void PatternConstraintSatisfactionTest()
		{
			var t = new PatternConstraint() { Pattern = "[a-z]" };
			Assert.IsTrue(t.IsSatisfiedBy("a"));
			Assert.IsTrue(t.IsSatisfiedBy("z"));
			Assert.IsFalse(t.IsSatisfiedBy("A"));
			Assert.IsFalse(t.IsSatisfiedBy("Z"));
		}

		[TestMethod]
		public void RangeConstraintSatisfactionTest()
		{
			var t = new RangeConstraint()
			{
				MinValue = 2,
				MinInclusive = true,
				MaxValue = 4,
				MaxInclusive = true,
			};

			Assert.IsFalse(t.IsSatisfiedBy(1));
			Assert.IsTrue(t.IsSatisfiedBy(2));
			Assert.IsTrue(t.IsSatisfiedBy(2.01));
			Assert.IsTrue(t.IsSatisfiedBy(3.99));
			Assert.IsTrue(t.IsSatisfiedBy(4));
			Assert.IsFalse(t.IsSatisfiedBy(4.01));

			t.MinInclusive = false;
			t.MaxInclusive = false;

			Assert.IsFalse(t.IsSatisfiedBy(1));
			Assert.IsFalse(t.IsSatisfiedBy(2));
			Assert.IsTrue(t.IsSatisfiedBy(2.01f));
			Assert.IsTrue(t.IsSatisfiedBy(3.99f));
			Assert.IsFalse(t.IsSatisfiedBy(4m));
			Assert.IsFalse(t.IsSatisfiedBy(4.01));

		}
	}
}
