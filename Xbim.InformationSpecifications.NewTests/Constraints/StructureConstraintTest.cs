using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;


namespace Xbim.InformationSpecifications.NewTests
{
	public class StructureConstraintTests
	{

		[Fact]
		public void StructureConstraintSatisfactionTest()
        {
            var vc = new ValueConstraint(TypeName.Decimal);
            var sc = new StructureConstraint
            {
                FractionDigits = 2,
                TotalDigits = 5
            };
            vc.AddAccepted(sc);
            TestDirectAndAfterPersistence(vc, InitialPassAndFailTests);

            // long cases
            sc.TotalDigits = 14;
            TestDirectAndAfterPersistence(vc, Test14Digits);

            sc.FractionDigits = 0;
            TestDirectAndAfterPersistence(vc, TestNoFraction);


            vc = new ValueConstraint(TypeName.Floating);
            sc = new StructureConstraint();
            vc.AddAccepted(sc);
            sc.FractionDigits = 2;
            sc.TotalDigits = 5;
            TestDirectAndAfterPersistence(vc, FloatingTests);

            vc = new ValueConstraint(TypeName.Double);
            sc = new StructureConstraint();
            vc.AddAccepted(sc);
            sc.FractionDigits = 2;
            sc.TotalDigits = 5;
            TestDirectAndAfterPersistence(vc, DoubleStructureTests);

            vc = new ValueConstraint(TypeName.String);
            sc = new StructureConstraint();
            vc.AddAccepted(sc);
            sc.Length = 4;
            TestDirectAndAfterPersistence(vc, StringTestLenis4);

            sc.Length = null;
            sc.MinLength = 3;
            sc.MaxLength = 5;
            TestDirectAndAfterPersistence(vc, StringTestLenRange);
        }

        private static void StringTestLenRange(ValueConstraint vc)
        {
            vc.IsSatisfiedBy("12").Should().BeFalse();
            vc.IsSatisfiedBy("123").Should().BeTrue();
            vc.IsSatisfiedBy("1234").Should().BeTrue();
            vc.IsSatisfiedBy("12345").Should().BeTrue();
            vc.IsSatisfiedBy("123456").Should().BeFalse();
        }

        private static void StringTestLenis4(ValueConstraint vc)
        {
            vc.IsSatisfiedBy("12345").Should().BeFalse();
            vc.IsSatisfiedBy("1234").Should().BeTrue();
        }

        private static void DoubleStructureTests(ValueConstraint vc)
        {
            vc.IsSatisfiedBy(324.75).Should().BeTrue(); // positive
            vc.IsSatisfiedBy(-324.75).Should().BeTrue(); // negative too

            vc.IsSatisfiedBy(32.754).Should().BeFalse(); // positive
            vc.IsSatisfiedBy(-32.754).Should().BeFalse(); // negative too

            vc.IsSatisfiedBy(324.753).Should().BeFalse(); // positive
            vc.IsSatisfiedBy(-324.753).Should().BeFalse(); // negative too
        }

        private static void FloatingTests(ValueConstraint vc)
        {
            vc.IsSatisfiedBy(324.75f).Should().BeTrue(); // positive
            vc.IsSatisfiedBy(-324.75f).Should().BeTrue(); // negative too

            vc.IsSatisfiedBy(32.754f).Should().BeFalse(); // positive
            vc.IsSatisfiedBy(-32.754f).Should().BeFalse(); // negative too

            vc.IsSatisfiedBy(324.753f).Should().BeFalse(); // positive
            vc.IsSatisfiedBy(-324.753f).Should().BeFalse(); // negative too
        }

        private static void InitialPassAndFailTests(ValueConstraint vc)
        {
            // basic value
            vc.IsSatisfiedBy(123.12m).Should().BeTrue();

            // decimal fails
            vc.IsSatisfiedBy(1234.1m).Should().BeFalse("because too few decimals");
            vc.IsSatisfiedBy(14.133m).Should().BeFalse("because too many decimals");

            // total len fails
            vc.IsSatisfiedBy(1234.21m).Should().BeFalse("because too many digits");
            vc.IsSatisfiedBy(14.13m).Should().BeFalse("because too few digits");
        }

        private static void Test14Digits(ValueConstraint vc)
        {
            vc.IsSatisfiedBy(214748364700.13m).Should().BeTrue();
            vc.IsSatisfiedBy(-214748364700.13m).Should().BeTrue(); // negative too
        }

        private static void TestDirectAndAfterPersistence(ValueConstraint vc, Action<ValueConstraint> testToRun)
        {
            testToRun.Invoke(vc);
            var vc2 = GetAgainAfterPersistence(vc);
            testToRun.Invoke(vc2);
        }

        private static void TestNoFraction(ValueConstraint vc)
        {
            vc.IsSatisfiedBy(21474836470013m).Should().BeTrue(); // positive
            vc.IsSatisfiedBy(-21474836470013m).Should().BeTrue(); // negative too
        }

        private static ValueConstraint GetAgainAfterPersistence(ValueConstraint vc)
        {
            var facet = new MaterialFacet
            {
                Value = vc
            };
            var s = new Xids();
			var t = s.PrepareSpecification(IfcSchemaVersion.IFC2X3);
            if (t.Requirement is not null)
			    t.Requirement.Facets.Add(facet);

			var tfn = Path.GetTempFileName();
			s.ExportBuildingSmartIDS(tfn);

			var unpers = Xids.ImportBuildingSmartIDS(tfn);
			var fg = unpers.FacetGroups(FacetGroup.FacetUse.All).FirstOrDefault();
			var unpersF = fg.Facets.OfType<MaterialFacet>().First();
			return unpersF.Value;
        }
    }
}
